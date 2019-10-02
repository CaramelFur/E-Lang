using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.solvable
{
  public class ESVariable : ESExpression
  {
    private readonly string name;

    public ESVariable(string name)
    {
      this.name = name;
    }

    public override EVariable Solve(EScope scope)
    {
      return scope.Get(name);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESVariable[" + name + "]";
      else return name;
    }
  }

}