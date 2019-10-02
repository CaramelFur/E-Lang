using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.operations
{
  // Empty operation that does nothing
  public class ENoOperation : EOperation
  {
    public override string ToString()
    {
      return "NoOp";
    }

    public override EVariable Exec(EScope scope)
    {
      return new EVVoid();
    }
  }

}