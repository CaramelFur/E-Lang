using System;
using System.Linq.Expressions;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public abstract class ESOperator
  {
    private readonly string op;

    protected readonly EType returnType = null;

    public ESOperator(string op, EType returnType)
    {
      this.op = op;
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
      if (detailed)
      {
        if (returnType != null) return "ESOperator(" + returnType + ")[" + op + "]";
        else return "ESOperator[" + op + "]";
      }
      else return op;
    }
  }

}