using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;

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

    public override string ToString()
    {
      return "EExpressionOperation{" + expression + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      return expression.Solve(scope);
    }
  }

}