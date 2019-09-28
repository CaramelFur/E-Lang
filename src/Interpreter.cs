namespace E_Lang.src
{
  public class Interpreter
  {
    private readonly EScope scope = new EScope();

    public Interpreter()
    {
      AddBasicFunctions();
    }

    private void AddBasicFunctions()
    {
      scope.SetFunction("log", new EFunction
      {
        type = new EType
        {
          type = "void"
        },
        arguments = new EFunctionArgument[] {
          new EFunctionArgument {
            type = new EType {
              type = "int"
            },
            variable = new EWord {
              word = "toLog"
            }
          }
        },
        operations = new EOperation[] {
          new EAPrintOperation{
            variable = new EWord {
              word = "toLog"
            }
          }
        }
      });
    }

    public EVariable Run(EProgram program)
    {
      return Run(program, scope);
    }

    public static EVariable Run(EProgram program, EScope scope)
    {
      EVariable output = new EVariable();
      EOperation[] operations = program.operations;
      foreach (EOperation operation in operations)
      {
        output = operation.Exec(scope);
      }
      return output;
    }
  }
}