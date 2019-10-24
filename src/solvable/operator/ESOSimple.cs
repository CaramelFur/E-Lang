using System;
using System.Collections.Generic;

using E_Lang.types;
using E_Lang.variables;

using LLVMSharp;
using E_Lang.llvm;

namespace E_Lang.solvable
{
  using CalcFunction = Func<LLVMHolder, dynamic, dynamic, LLVMValueRef>;

  public enum ESOSimpleType
  {
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Modulo,

    AndAlso,
    OrElse,

    Equal,
    NotEqual,

    GreaterThanOrEqual,
    LessThanOrEqual,
    GreaterThan,
    LessThan,

    Point
  }

  public class ESOSimpleTypeObject
  {
    private readonly ESOSimpleType type;
    private readonly EType? input = null;
    private readonly EType output;
    private readonly CalcFunction calc;

    private ESOSimpleTypeObject(ESOSimpleType type, EType? input, EType output, CalcFunction calc)
    {
      this.type = type;
      this.calc = calc;
      this.input = input;
      this.output = output;
    }

    private EVariable Calc(LLVMHolder llvm, EVariable a, EVariable b)
    {
      EVariable aConv = a;
      EVariable bConv = b;
      if (input != null)
      {
        aConv = a.Convert(input.Value);
        bConv = b.Convert(input.Value);
      }

      LLVMValueRef calculated = calc(llvm, aConv.Get(), bConv.Get());

      return EVariable.New(output, llvm).Set(calculated);
    }

    private EVariable Calc(LLVMHolder llvm, EVariable a)
    {
      EVariable aConv = a;
      if (input != null)
      {
        aConv = a.Convert(input.Value);
      }

      LLVMValueRef calculated = calc(llvm, aConv.Get(), null);

      return EVariable.New(output, llvm).Set(calculated);
    }

    private static readonly Dictionary<ESOSimpleType, ESOSimpleTypeObject> dict = Populate();

    private static Dictionary<ESOSimpleType, ESOSimpleTypeObject> Populate()
    {
      Dictionary<ESOSimpleType, ESOSimpleTypeObject> temp = new Dictionary<ESOSimpleType, ESOSimpleTypeObject>();

      Add(temp, ESOSimpleType.Add, EType.Double, EType.Double, (llvm, a, b) => LLVM.BuildAdd(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.Subtract, EType.Double, EType.Double, (llvm, a, b) => a - b);
      Add(temp, ESOSimpleType.Multiply, EType.Double, EType.Double, (llvm, a, b) => a * b);
      Add(temp, ESOSimpleType.Divide, EType.Double, EType.Double, (llvm, a, b) => a / b);
      Add(temp, ESOSimpleType.Power, EType.Double, EType.Double, (llvm, a, b) => a ^ b);
      Add(temp, ESOSimpleType.Modulo, EType.Double, EType.Double, (llvm, a, b) => a % b);

      Add(temp, ESOSimpleType.AndAlso, null, EType.Boolean, (llvm, a, b) => a && b);
      Add(temp, ESOSimpleType.OrElse, null, EType.Boolean, (llvm, a, b) => a || b);

      Add(temp, ESOSimpleType.Equal, null, EType.Boolean, (llvm, a, b) => a == b);
      Add(temp, ESOSimpleType.NotEqual, null, EType.Boolean, (llvm, a, b) => a != b);

      Add(temp, ESOSimpleType.GreaterThanOrEqual, EType.Double, EType.Boolean, (llvm, a, b) => a >= b);
      Add(temp, ESOSimpleType.LessThanOrEqual, EType.Double, EType.Boolean, (llvm, a, b) => a <= b);
      Add(temp, ESOSimpleType.GreaterThan, EType.Double, EType.Boolean, (llvm, a, b) => a > b);
      Add(temp, ESOSimpleType.LessThan, EType.Double, EType.Boolean, (llvm, a, b) => a < b);

      return temp;
    }

    private static void Add(Dictionary<ESOSimpleType, ESOSimpleTypeObject> temp,
      ESOSimpleType type, EType? input, EType output, CalcFunction calc)
    {
      temp.Add(type, new ESOSimpleTypeObject(type, input, output, calc));
    }

    public static EVariable Calc(LLVMHolder llvm, ESOSimpleType type, EVariable a)
    {
      return Calc(llvm, type, a, null);
    }

    public static EVariable Calc(LLVMHolder llvm, ESOSimpleType type, EVariable a, EVariable b)
    {
      ESOSimpleTypeObject selected = dict[type];
      if (selected == null) throw new ELangException("Invalid operation: " + type.ToString());
      return selected.Calc(llvm, a, b);
    }
  }

  public class ESOSimple : ESOperator
  {
    private readonly ESOSimpleType type;

    public ESOSimple(string op, ESOSimpleType type) :
      base(op)
    {
      this.type = type;
    }

    public override EVariable Solve(LLVMHolder llvm, EVariable first)
    {
      return ESOSimpleTypeObject.Calc(llvm, type, first);
    }

    public override EVariable Solve(LLVMHolder llvm, EVariable first, EVariable second)
    {
      return ESOSimpleTypeObject.Calc(llvm, type, first, second);
    }
  }

}