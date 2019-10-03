using E_Lang.types;
using E_Lang.scope;
using E_Lang.operations;
using E_Lang.variables;

namespace E_Lang.interpreter
{
  public class Interpreter
  {
    private readonly EScope scope = new EScope();

    // Insert global functions like log into the scope when a new interpreter is created
    public Interpreter()
    {
      GlobalFunctions.Add(scope);
    }

    public EVariable Run(EProgram program)
    {
      return Run(program, scope);
    }

    // Run a program
    public static EVariable Run(EProgram program, EScope scope)
    {
      // Create a variable to assign the output to
      EVariable output = new EVVoid();

      // Loop over every instruction and return the last value
      EOperation[] operations = program.GetOperations();
      foreach (EOperation operation in operations)
      {
        output = operation.Exec(scope);
      }
      return output;
    }
  }
}