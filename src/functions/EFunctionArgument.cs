using E_Lang.types;

namespace E_Lang.functions
{
  public class EFunctionArgument
  {
    public EType type;
    public EWord variable;

    public override string ToString()
    {
      return type + ": " + variable;
    }
  }
}