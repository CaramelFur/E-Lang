using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.solvable
{
  public class ESBoolean : ESExpression
  {
    private readonly bool boolean;

    public ESBoolean(bool boolean)
    {
      this.boolean = boolean;
    }

    public override EVariable Solve(EScope scope)
    {
      return new EVBoolean().Set(boolean);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESBoolean[" + boolean.ToString() + "]";
      else return boolean.ToString();
    }
  }

}