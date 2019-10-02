using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.solvable
{
  public abstract class ESExpression
  {
    public static ESExpression CombineExpression(ESOperator op, ESExpression first, ESExpression second)
    {
      return new ESDoubleExpression(first, second, op);
    }

    public virtual EVariable Solve(EScope scope)
    {
      return new EVVoid();
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public virtual string ToString(bool detailed)
    {
      return "";
    }
  }

}