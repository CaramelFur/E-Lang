using E_Lang.variables;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.solvable
{
  public class ESolvable
  {
    private readonly ESExpression expression;

    public ESolvable(ESExpression expression)
    {
      this.expression = expression;
    }

    public EVariable Solve(LLVMHolder llvm)
    {
      return new EVInt(llvm).Assign(expression.Solve(llvm));
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool detailed)
    {
      if (detailed) return "ESolvable[" + expression.ToString(detailed) + "]";
      else return expression.ToString(detailed);
    }
  }
}