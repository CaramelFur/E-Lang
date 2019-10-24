using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public class ESBoolean : ESExpression
  {
    private readonly bool boolean;

    public ESBoolean(bool boolean)
    {
      this.boolean = boolean;
    }

    public override EVariable Solve(LLVMHolder llvm)
    {
      return new EVBoolean(llvm).Set(boolean);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESBoolean[" + boolean.ToString() + "]";
      else return boolean.ToString();
    }
  }

}