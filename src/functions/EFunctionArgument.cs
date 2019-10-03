using E_Lang.types;

namespace E_Lang.functions
{
  public class EFunctionArgument
  {
    private readonly EType type;
    private readonly EWord variable;

    public EFunctionArgument(EWord name, EType type)
    {
      variable = name;
      this.type = type;
    }

    public override string ToString()
    {
      return type + ": " + variable;
    }

    public EWord GetVariable()
    {
      return variable;
    }

    public EType GetEType()
    {
      return type;
    }
  }
}