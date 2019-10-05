using System;
using System.Linq.Expressions;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public class ESOLinq : ESOperator
  {
    private readonly ExpressionType type;

    public ESOLinq(string op, ExpressionType type, EType returnType) :
      base(op, returnType)
    {
      this.type = type;
    }

    public override EVariable Solve(EVariable first, EVariable second)
    {
      // Solve equations with high precision
      if (first is EVInt) first = first.Convert("double");
      if (second is EVInt) second = second.Convert("double");

      Expression toSolve = Expression.MakeBinary(
        type,
        Expression.Constant(first.Get()),
        Expression.Constant(second.Get())
      );

      dynamic solve = Expression.Lambda(toSolve).Compile();
      return EVariable.New(returnType).Set(solve());
    }
  }

}