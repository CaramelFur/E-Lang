using System;
using System.Linq.Expressions;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public class ESOperator
  {
    private readonly ExpressionType type;
    private readonly string op;

    public ESOperator(string op, ExpressionType type)
    {
      this.op = op;
      this.type = type;
    }

    public EVariable Solve(EVariable first, EVariable second)
    {
      decimal dfirst =
      (
        (EVDouble)
        first.Convert(new EType("double"))
      ).Get();

      decimal dsecond =
      (
        (EVDouble)
        second.Convert(new EType("double"))
      ).Get();

      Expression toSolve = Expression.MakeBinary(
        type,
        Expression.Constant(dfirst),
        Expression.Constant(dsecond)
      );

      decimal solved = Expression.Lambda<Func<decimal>>(toSolve).Compile()();
      return new EVDouble().Set(solved);
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool detailed)
    {
      if (detailed) return "ESOperator[" + op + "]";
      else return op;
    }
  }

}