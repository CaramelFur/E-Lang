using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;

namespace E_Lang.operations
{
  // This operation solves a solvable and assigns it to a variable
  public class EAssignOperation : EOperation
  {
    private readonly EWord variable;

    private readonly ESolvable value;

    public EAssignOperation(EWord variable, ESolvable value)
    {
      this.variable = variable;
      this.value = value;
    }

    public override string ToString()
    {
      return "EAssignOperation{" + variable + " = " + value.ToString() + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVariable toUpdate = scope.Get(variable.ToString());
      toUpdate.Assign(value.Solve(scope));
      return toUpdate;
    }
  }

}