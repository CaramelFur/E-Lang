using System;
using System.Linq.Expressions;

namespace E_Lang.src
{
  public class ESolvable
  {
    public ESExpression expression;

    public decimal Solve(EScope scope)
    {
      return expression.Solve(scope);
    }

    public override string ToString()
    {
      return expression.ToString();
    }
  }


  public class ESOperator
  {
    public ExpressionType type;
    public string op;

    public decimal Solve(decimal first, decimal second)
    {
      Expression toSolve = Expression.MakeBinary(
        type,
        Expression.Constant(first),
        Expression.Constant(second)
      );

      return Expression.Lambda<Func<decimal>>(toSolve).Compile()();
    }

    public override string ToString()
    {
      return op;
    }
  }

  public abstract class ESExpression
  {
    public static ESExpression CombineExpression(ESOperator op, ESExpression first, ESExpression second)
    {
      return new ESDoubleExpression(first, second, op);
    }

    public virtual decimal Solve(EScope scope)
    {
      return 0;
    }
  }

  public class ESDoubleExpression : ESExpression
  {
    private readonly ESExpression first;
    private readonly ESExpression second;
    private readonly ESOperator op;

    public ESDoubleExpression(ESExpression first, ESExpression second, ESOperator op)
    {
      this.first = first;
      this.second = second;
      this.op = op;
    }

    public override decimal Solve(EScope scope)
    {
      decimal firstS = first.Solve(scope);
      decimal secondS = second.Solve(scope);
      return op.Solve(firstS, secondS);
    }

    public override string ToString()
    {
      return first.ToString() + " " + op.ToString() + " " + second.ToString();
    }
  }

  public class ESNumber : ESExpression
  {
    private readonly decimal number;

    public ESNumber(decimal number)
    {
      this.number = number;
    }

    public override decimal Solve(EScope scope)
    {
      return number;
    }

    public override string ToString()
    {
      return number.ToString();
    }
  }

  public class ESVariable : ESExpression
  {
    private readonly string name;

    public ESVariable(string name)
    {
      this.name = name;
    }

    public override decimal Solve(EScope scope)
    {
      return scope.Get(name).value;
    }

    public override string ToString()
    {
      return name;
    }
  }

}