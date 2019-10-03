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
    private readonly EType returnType;

    public ESOperator(string op, ExpressionType type, EType returnType)
    {
      this.op = op;
      this.type = type;
      this.returnType = returnType;
    }

    public EVariable Solve(EVariable first, EVariable second)
    {
      Expression toSolve = Expression.MakeBinary(
        type,
        Expression.Constant(first.Get()),
        Expression.Constant(second.Get())
      );

      dynamic solve = Expression.Lambda(toSolve).Compile();
      return EVariable.New(returnType).Set(solve());
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool detailed)
    {
      if (detailed) return "ESOperator(" + returnType + ")[" + op + "]";
      else return op;
    }
  }

}