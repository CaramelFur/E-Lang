using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;

namespace E_Lang.operations
{
  // This operation creates a new variable in the current scope to be used later
  public class ECreateOperation : EOperation
  {
    private readonly EWord name;
    private readonly EType type;

    private readonly EAssignOperation alsoAssign = null;

    public ECreateOperation(EWord name, EType type)
    {
      this.name = name;
      this.type = type;
    }

    public ECreateOperation(EWord name, EType type, ESolvable assign) : this(name, type)
    {
      alsoAssign = new EAssignOperation(name, assign);
    }

    public override string ToString()
    {
      if (alsoAssign != null)
      {
        return "ECreateOperation{\n" + type + ": '" + name + "'\nsuboperation: " + alsoAssign + "\n}";
      }
      return "ECreateOperation{" + type + ": '" + name + "'}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVariable newVar = EVariable.New(type);
      scope.Set(name.ToString(), newVar);

      if (alsoAssign != null)
      {
        alsoAssign.Exec(scope);
      }

      return newVar;
    }
  }

}