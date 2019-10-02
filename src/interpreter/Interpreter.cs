using E_Lang.types;
using E_Lang.scope;
using E_Lang.functions;
using E_Lang.operations;
using E_Lang.variables;

namespace E_Lang.interpreter
{
  public class Interpreter
  {
    private readonly EScope scope = new EScope();

    public Interpreter()
    {
      GlobalFunctions.Add(scope);
    }

    public EVariable Run(EProgram program)
    {
      return Run(program, scope);
    }

    public static EVariable Run(EProgram program, EScope scope)
    {
      EVariable output = new EVVoid();
      EOperation[] operations = program.GetOperations();
      foreach (EOperation operation in operations)
      {
        output = operation.Exec(scope);
      }
      return output;
    }
  }
}