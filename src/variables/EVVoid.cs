using System;

using E_Lang.types;

namespace E_Lang.variables
{
  public class EVVoid : EVariable
  {
    public override EVariable Assign(EVariable assign)
    {
      assign.Convert(GetEType());
      throw new Exception("Shoulnt have thrown this");
    }

    public override string ToString()
    {
      return "void";
    }
  }
}