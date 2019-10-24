using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.operations
{
  // Empty operation that does nothing
  public class ENoOperation : EOperation
  {
    public override string ToString()
    {
      return "NoOp";
    }

    public override EVariable Exec(LLVMHolder llvm)
    {
      return new EVVoid(llvm);
    }
  }

}