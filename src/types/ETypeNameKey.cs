namespace E_Lang.types
{
  public class ETypeNameKey
  {
    private readonly EType type;
    private readonly EWord variable;

    public ETypeNameKey(EWord name, EType type)
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