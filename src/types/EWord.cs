namespace E_Lang.types
{
  public class EWord
  {
    private readonly string word;

    public EWord(string word)
    {
      this.word = word;
    }

    public string Get(){
      if(word == null) throw new ELangException("Tried to access empty word");
      return word;
    }

    public override string ToString()
    {
      return Get();
    }
  }
}