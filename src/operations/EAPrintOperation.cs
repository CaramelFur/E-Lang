using System;

using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.operations
{
  public class EAPrintOperation : EOperation
  {
    private readonly EWord variable;

    public EAPrintOperation(EWord variable)
    {
      this.variable = variable;
    }

    public override string ToString()
    {
      return "EAPrint{variable: " + variable + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      Console.WriteLine(scope.Get(variable.ToString()));
      return new EVVoid();
    }
  }

}