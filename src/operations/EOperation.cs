using E_Lang.variables;
using E_Lang.scope;


namespace E_Lang.operations
{
  // The base class of an operation
  public abstract class EOperation
  {
    public override string ToString()
    {
      return "";
    }

    public virtual EVariable Exec(EScope scope)
    {
      return new EVVoid();
    }
  }

}