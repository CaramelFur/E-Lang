using E_Lang.types;
using LLVMSharp;
using E_Lang.llvm;
using E_Lang.solvable;
using E_Lang.variables;

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

    public override EVariable Exec(LLVMHolder llvm)
    {
      EVariable toUpdate = llvm.GetScope().Get(variable.ToString());
      toUpdate.Assign(value.Solve(llvm));
      return toUpdate;
    }

    public override string ToString()
    {
      return "EAssignOperation{" + variable + " = " + value.ToString() + "}";
    }
  }

}