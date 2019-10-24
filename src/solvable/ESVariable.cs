using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public class ESVariable : ESExpression
  {
    private readonly string name;

    public ESVariable(string name)
    {
      this.name = name;
    }

    public override EVariable Solve(LLVMHolder llvm)
    {
      return llvm.GetScope().Get(name);
    }

    public override string ToString(bool detailed)
    {
      if (detailed) return "ESVariable[" + name + "]";
      else return name;
    }
  }

}