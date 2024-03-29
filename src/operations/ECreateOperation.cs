using System.Linq;

using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;

namespace E_Lang.operations
{
  // This operation creates a new variable in the current scope to be used later
  public class ECreateOperation : EOperation
  {
    private readonly EWord[] variables;
    private readonly ETypeWord type;

    private readonly EAssignOperation[] assignOperations = null;

    public ECreateOperation(EWord[] variables, ETypeWord type)
    {
      this.variables = variables;
      this.type = type;
    }

    public ECreateOperation(EWord[] variables, ETypeWord type, ESolvable assign) : this(variables, type)
    {
      assignOperations = variables.Select(variable => new EAssignOperation(variable, assign)).ToArray();
    }

    public override string ToString()
    {
      string varstring = "";
      for (int i = 0; i < variables.Length; i++)
      {
        if (i != 0) varstring += "\n";
        varstring += variables[i].ToString();
      }

      if (assignOperations != null)
      {
        string opstring = "";
        for (int i = 0; i < assignOperations.Length; i++)
        {
          if (i != 0) opstring += "\n";
          opstring += assignOperations[i].ToString();
        }

        return "ECreateOperation{\n" + type + ": '" + varstring + "'\nsuboperations: (\n" + opstring + "\n)\n}";
      }
      return "ECreateOperation{" + type + ": '" + varstring + "'}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVariable newVar = new EVVoid();

      foreach (EWord variable in variables)
      {
        newVar = EVariable.New(type);
        scope.Set(variable.ToString(), newVar);
      }

      if (assignOperations != null)
      {
        foreach (EAssignOperation assignOp in assignOperations)
        {
          assignOp.Exec(scope);
        }
      }

      return newVar;
    }
  }

}