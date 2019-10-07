namespace E_Lang.types
{
  public enum EType
  {
    Void,
    Int,
    Double,
    Char,
    Boolean
  }

  public class ETypeWord
  {
    private readonly EType type;

    public ETypeWord(EType type)
    {
      this.type = type;
    }

    public EType Get()
    {
      return type;
    }

    public override string ToString()
    {
      return type.ToString().ToLower();
    }
  }
}
