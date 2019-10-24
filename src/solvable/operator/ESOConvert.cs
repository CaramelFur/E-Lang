using E_Lang.variables;
using LLVMSharp;
using E_Lang.llvm;

namespace E_Lang.solvable
{

  public class ESOConvert : ESOperator
  {

    public ESOConvert(string op) : base(op) { }

    public override EVariable Solve(LLVMHolder llvm, EVariable first, EVariable second)
    {
      return first.Convert(second.GetEType());
    }
  }

}