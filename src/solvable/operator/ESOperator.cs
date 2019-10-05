using System;
using System.Linq.Expressions;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public abstract class ESOperator
  {
    private readonly string op;

    protected readonly ExpressionType type;
    protected readonly EType returnType;

    public ESOperator(string op, ExpressionType type, EType returnType)
    {
      this.op = op;
      this.type = type;
      this.returnType = returnType;
    }

    public virtual EVariable Solve(EVariable first)
    {
      return new EVVoid();
    }

    public virtual EVariable Solve(EVariable first, EVariable second)
    {
      return new EVVoid();
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