using E_Lang.llvm;
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

    public virtual EVariable Solve(LLVMHolder llvm,  EVariable first)
    {
      throw new ELangException("Cannot solve abstract class operator");
    }

    public virtual EVariable Solve(LLVMHolder llvm, EVariable first, EVariable second)
    {
      throw new ELangException("Cannot solve abstract class operator");
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