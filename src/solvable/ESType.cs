using E_Lang.variables;
using E_Lang.scope;
using E_Lang.types;

namespace E_Lang.solvable
{
  public class ESType : ESExpression
  {
    private readonly EType type;

    public ESType(EType type)
    {
      this.type = type;
    }

    public override EVariable Solve(EScope scope)
    {
      return EVariable.New(type);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESVariable[" + type + "]";
      else return type.ToString();
    }
  }

}