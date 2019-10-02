namespace E_Lang.lexer
{
  public class EToken
  {
    private readonly string token;

    public EToken(string token)
    {
      this.token = token;
    }

    public override string ToString()
    {
      return token;
    }
  }
}
