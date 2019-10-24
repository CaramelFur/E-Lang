using E_Lang.llvm;
using E_Lang.variables;

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

    public override EVariable Solve(LLVMHolder llvm)
    {
      EVariable firstS = first.Solve(llvm);
      EVariable secondS = second.Solve(llvm);
      return op.Solve(llvm, firstS, secondS);
    }
    
    public override string ToString(bool detailed)
    {
      if (detailed) return "ESDoubleExpression[" + first.ToString(detailed) + " " + op.ToString(detailed) + " " + second.ToString(detailed) + "]";
      else return first.ToString(detailed) + " " + op.ToString(detailed) + " " + second.ToString(detailed);
    }
  }

}