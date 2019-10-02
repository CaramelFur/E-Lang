using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;

namespace E_Lang.operations
{
  // This operation creates a new variable in the current scope to be used later
  public class ECreateOperation : EOperation
  {
    private readonly EWord name;
    private readonly EType type;

    public ECreateOperation(EWord name, EType type)
    {
      this.name = name;
      this.type = type;
    }

    public override string ToString()
    {
      return "ECreateOperation{" + type + ": '" + name + "'}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVariable newVar = EVariable.New(type);
      scope.Set(name.ToString(), newVar);
      return newVar;
    }
  }

}