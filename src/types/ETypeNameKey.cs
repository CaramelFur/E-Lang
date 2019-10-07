namespace E_Lang.types
{
  public class ETypeNameKey
  {
    private readonly ETypeWord type;
    private readonly EWord variable;

    public ETypeNameKey(EWord name, ETypeWord type)
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

    public ETypeWord GetEType()
    {
      return type;
    }
  }
}