using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;

namespace E_Lang.functions
{
  public abstract class EFunction
  {
    public virtual EVariable Exec(EScope scope, ESolvable[] args)
    {
      return new EVVoid();
    }
  }
}