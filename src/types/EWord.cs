namespace E_Lang.types
{
  public class EWord
  {
    private readonly string word;

    public EWord(string word)
    {
      this.word = word;
    }

    public override string ToString()
    {
      return word;
    }
  }
}
