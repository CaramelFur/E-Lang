using E_Lang.variables;

namespace E_Lang.solvable
{
  public abstract class ESOperator
  {
    private readonly string op;

    public ESOperator(string op)
    {
      this.op = op;
    }

    public virtual EVariable Solve(EVariable first)
    {
      return new EVVoid();
    }

    public virtual EVariable Solve(EVariable first, EVariable second)
    {
      return new EVVoid();
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool detailed)
    {
      if (detailed)
      {
        return "ESOperator[" + op + "]";
      }
      else return op;
    }
  }

}