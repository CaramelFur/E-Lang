using System;
using System.Collections.Generic;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{
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
    LessThan
  }

  public class ESOSimpleTypeObject
  {
    private readonly ESOSimpleType type;
    private readonly EType? input = null;
    private readonly EType output;
    private readonly Func<dynamic, dynamic, dynamic> calc;

    private ESOSimpleTypeObject(ESOSimpleType type, EType? input, EType output, Func<dynamic, dynamic, dynamic> calc)
    {
      this.type = type;
      this.calc = calc;
      this.input = input;
      this.output = output;
    }

    private EVariable Calc(EVariable a, EVariable b)
    {
      EVariable convA = a;
      EVariable convB = b;
      if (input.HasValue)
      {
        EType inputType = input.Value;
        convA = a.Convert(inputType);
        convB = b.Convert(inputType);
      }

      dynamic convC = calc(convA.Get(), convB.Get());

      return EVariable.New(output).Set(convC);
    }

    private static readonly Dictionary<ESOSimpleType, ESOSimpleTypeObject> dict = Populate();

    private static void Add(Dictionary<ESOSimpleType, ESOSimpleTypeObject> temp,
      ESOSimpleType type, EType? input, EType output,
      Func<dynamic, dynamic, dynamic> calc)
    {
      temp.Add(type, new ESOSimpleTypeObject(type, input, output, calc));
    }

    private static Dictionary<ESOSimpleType, ESOSimpleTypeObject> Populate()
    {
      Dictionary<ESOSimpleType, ESOSimpleTypeObject> temp = new Dictionary<ESOSimpleType, ESOSimpleTypeObject>();

      Add(temp, ESOSimpleType.Add, EType.Double, EType.Double, (a, b) => a + b);
      Add(temp, ESOSimpleType.Subtract, EType.Double, EType.Double, (a, b) => a - b);
      Add(temp, ESOSimpleType.Multiply, EType.Double, EType.Double, (a, b) => a * b);
      Add(temp, ESOSimpleType.Divide, EType.Double, EType.Double, (a, b) => a / b);
      Add(temp, ESOSimpleType.Power, EType.Double, EType.Double, (a, b) => a ^ b);
      Add(temp, ESOSimpleType.Modulo, EType.Double, EType.Double, (a, b) => a % b);

      Add(temp, ESOSimpleType.AndAlso, null, EType.Boolean, (a, b) => a && b);
      Add(temp, ESOSimpleType.OrElse, null, EType.Boolean, (a, b) => a || b);

      Add(temp, ESOSimpleType.Equal, null, EType.Boolean, (a, b) => a == b);
      Add(temp, ESOSimpleType.NotEqual, null, EType.Boolean, (a, b) => a != b);

      Add(temp, ESOSimpleType.GreaterThanOrEqual, EType.Double, EType.Boolean, (a, b) => a >= b);
      Add(temp, ESOSimpleType.LessThanOrEqual, EType.Double, EType.Boolean, (a, b) => a <= b);
      Add(temp, ESOSimpleType.GreaterThan, EType.Double, EType.Boolean, (a, b) => a > b);
      Add(temp, ESOSimpleType.LessThan, EType.Double, EType.Boolean, (a, b) => a < b);

      return temp;
    }

    public static EVariable Calc(ESOSimpleType type, EVariable a, EVariable b)
    {
      ESOSimpleTypeObject selected = dict[type];
      if (selected == null) throw new Exception("Invalid operation: " + type.ToString());
      return selected.Calc(a, b);
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

    public override EVariable Solve(EVariable first, EVariable second)
    {
      return ESOSimpleTypeObject.Calc(type, first, second);
    }
  }

}