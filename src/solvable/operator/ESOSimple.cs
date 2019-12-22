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
  }

  public class ESOSimpleTypeObject
  {
    // This class has an instance for every simple operation (See ESOSimpleType)
    private readonly ESOSimpleType type;
    private readonly EType input = null;
    private readonly EType output;
    private readonly CalcFunction calc;

    private ESOSimpleTypeObject(ESOSimpleType type, EType input, EType output, CalcFunction calc)
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
        aConv = EVariable.New(input, llvm).Assign(a);
        bConv = EVariable.New(input, llvm).Assign(b);
      }

      var aa = EVariable.GetRawValueFromVariable(aConv);
      Console.WriteLine("aaa " + aa.PrintValueToString());

      LLVMValueRef calculated = calc(
        llvm,
        aa,
        EVariable.GetRawValueFromVariable(bConv)
      );


      EVariable outvar = EVariable.New(output, llvm);
      return EVariable.PutRawValueIntVariable(outvar, calculated);
    }

    private EVariable Calc(LLVMHolder llvm, EVariable a)
    {
      EVariable aConv = a;
      if (input != null)
      {
        aConv = EVariable.New(input, llvm).Assign(a);
      }

      LLVMValueRef calculated = calc(llvm, EVariable.GetRawValueFromVariable(aConv), null);

      EVariable outvar = EVariable.New(output, llvm);
      return EVariable.PutRawValueIntVariable(outvar, calculated);
    }

    private static readonly Dictionary<ESOSimpleType, ESOSimpleTypeObject> dict = Populate();

    private static Dictionary<ESOSimpleType, ESOSimpleTypeObject> Populate()
    {
      Dictionary<ESOSimpleType, ESOSimpleTypeObject> temp = new Dictionary<ESOSimpleType, ESOSimpleTypeObject>();

      Add(temp, ESOSimpleType.Add, "double", "double", (llvm, a, b)
        => LLVM.BuildFAdd(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.Subtract, "double", "double", (llvm, a, b)
        => LLVM.BuildFSub(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.Multiply, "double", "double", (llvm, a, b)
        => LLVM.BuildFMul(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.Divide, "double", "double", (llvm, a, b)
        => LLVM.BuildFDiv(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.Power, "double", "double", (llvm, a, b)
        => LLVM.BuildFMul(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.Modulo, "double", "double", (llvm, a, b)
        => LLVM.BuildFMul(llvm.GetBuilder(), a, b, llvm.GetNewName()));

      Add(temp, ESOSimpleType.AndAlso, null, "boolean", (llvm, a, b)
        => LLVM.BuildAnd(llvm.GetBuilder(), a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.OrElse, null, "boolean", (llvm, a, b)
        => LLVM.BuildOr(llvm.GetBuilder(), a, b, llvm.GetNewName()));

      Add(temp, ESOSimpleType.Equal, "double", "boolean", (llvm, a, b)
        => LLVM.BuildFCmp(llvm.GetBuilder(), LLVMRealPredicate.LLVMRealOEQ, a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.NotEqual, "double", "boolean", (llvm, a, b)
        => LLVM.BuildFCmp(llvm.GetBuilder(), LLVMRealPredicate.LLVMRealONE, a, b, llvm.GetNewName()));

      Add(temp, ESOSimpleType.GreaterThanOrEqual, "double", "boolean", (llvm, a, b)
        => LLVM.BuildFCmp(llvm.GetBuilder(), LLVMRealPredicate.LLVMRealOGE, a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.LessThanOrEqual, "double", "boolean", (llvm, a, b)
        => LLVM.BuildFCmp(llvm.GetBuilder(), LLVMRealPredicate.LLVMRealOLE, a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.GreaterThan, "double", "boolean", (llvm, a, b)
        => LLVM.BuildFCmp(llvm.GetBuilder(), LLVMRealPredicate.LLVMRealOGT, a, b, llvm.GetNewName()));
      Add(temp, ESOSimpleType.LessThan, "double", "boolean", (llvm, a, b)
        => LLVM.BuildFCmp(llvm.GetBuilder(), LLVMRealPredicate.LLVMRealOLE, a, b, llvm.GetNewName()));

      return temp;
    }

    // Add a esosimpletypeobject to a dictionary
    private static void Add(Dictionary<ESOSimpleType, ESOSimpleTypeObject> temp,
      ESOSimpleType type, string input, string output, CalcFunction calc)
    {
      temp.Add(type, new ESOSimpleTypeObject(type, input != null ? new EType(input) : null, new EType(output), calc));
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