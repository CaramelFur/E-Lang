using System;

using E_Lang.llvm;

namespace E_Lang.variables
{
  public class EVVoid : EVariable
  {
    public EVVoid(LLVMHolder holder) : base(holder) { }

    public override EVariable Assign(EVariable assign)
    {
      throw new ELangException("Shoulnt have thrown this");
    }
  }
}