using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.solvable
{
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

    public override EVariable Solve(EScope scope)
    {
      EVariable firstS = first.Solve(scope);
      EVariable secondS = second.Solve(scope);
      return op.Solve(firstS, secondS);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESDoubleExpression[" + first.ToString(detailed) + " " + op.ToString(detailed) + " " + second.ToString(detailed) + "]";
      else return first.ToString(detailed) + " " + op.ToString(detailed) + " " + second.ToString(detailed);
    }
  }

}