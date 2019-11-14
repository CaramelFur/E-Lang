namespace E_Lang.types
{
  public class EType
  {
    private readonly string word;

    public EType(string word)
    {
      this.word = word;
    }

    public string Get()
    {
      if (word == null) throw new ELangException("Tried to access empty type");
      return word;
    }

    public override string ToString()
    {
      return Get();
    }
  }
}