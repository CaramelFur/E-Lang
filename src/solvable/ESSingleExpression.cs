using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public class ESSingleExpression : ESExpression
  {
    private readonly ESExpression first;
    private readonly ESOperator op;

    public ESSingleExpression(ESExpression first, ESOperator op)
    {
      this.first = first;
      this.op = op;
    }

    public override EVariable Solve(LLVMHolder llvm)
    {
      EVariable firstS = first.Solve(llvm);
      return op.Solve(llvm, firstS);
    }
    
    public override string ToString(bool detailed)
    {
      if (detailed) return "ESDoubleExpression[" + first.ToString(detailed) + " " + op.ToString(detailed) + "]";
      else return first.ToString(detailed) + " " + op.ToString(detailed);
    }
  }

}