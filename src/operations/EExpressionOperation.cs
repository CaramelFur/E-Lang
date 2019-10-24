using E_Lang.variables;
using E_Lang.llvm;
using E_Lang.solvable;
using LLVMSharp;

namespace E_Lang.operations
{
  // This is an expression operation, it solves a solvable and return the value
  public class EExpressionOperation : EOperation
  {
    private readonly ESolvable expression;

    public EExpressionOperation(ESolvable solvable)
    {
      expression = solvable;
    }

    public override EVariable Exec(LLVMHolder llvm)
    {
      return expression.Solve(llvm);
    }

    public override string ToString()
    {
      return "EExpressionOperation{" + expression + "}";
    }
  }

}