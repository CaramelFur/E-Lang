using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public class ESNumber : ESExpression
  {
    private readonly decimal number;

    public ESNumber(decimal number)
    {
      this.number = (int)number;
    }

    public override EVariable Solve(LLVMHolder llvm)
    {
      /*if (true || IsInteger())
      {
        return new EVInt(llvm).Set((int)number);
      }
      else
      {
        return new EVDouble(llvm).Set(number);
      }*/

      return new EVInt(llvm).InsertRaw((int)number);
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