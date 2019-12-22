using E_Lang.variables;
using LLVMSharp;
using E_Lang.llvm;

namespace E_Lang.solvable
{

  public class ESOPointer : ESOperator
  {

    public ESOPointer(string op) : base(op) { }

    public override EVariable Solve(LLVMHolder llvm, EVariable first)
    {
      throw new ELangException("fuck u");
    }
  }

}