using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.solvable
{
  public class ESNumber : ESExpression
  {
    private readonly decimal number;

    public ESNumber(decimal number)
    {
      this.number = number;
    }

    public override EVariable Solve(EScope scope)
    {
      if (IsInteger()) return new EVInt().Set((int)number);
      else return new EVDouble().Set(number);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESNumber[" + number.ToString() + "]";
      else return number.ToString();
    }

    public bool IsInteger()
    {
      return (number % 1) == 0;
    }
  }

}