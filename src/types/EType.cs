namespace E_Lang.types
{
  public class EType
  {
    private readonly string type;

    public EType(string type)
    {
      this.type = type;
    }

    public override string ToString()
    {
      return type;
    }
  }
}
