using System;

using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;
using E_Lang.interpreter;


namespace E_Lang.operations
{
  // This operation solves a solvable and if it is true executes its code
  public class EWhileOperation : EOperation
  {
    private readonly ESolvable check;
    private readonly EProgram program;

    public EWhileOperation(ESolvable check, EOperation[] operations)
    {
      this.check = check;
      program = new EProgram(operations);
    }

    public override string ToString()
    {
      return "EWhileOperation{\nwhile: " + check + ";\n" + program + "\n}";
    }

    public override EVariable Exec(EScope scope)
    {
      Func<bool> checkVar = () => (
        (EVBoolean)
        check.Solve(scope)
        .Convert(EType.Boolean)
      ).Get();

      EVariable output = new EVVoid();

      while (checkVar())
      {
        EScope subScope = scope.GetChild();
        output = Interpreter.Run(program, subScope);
      }

      return output;
    }
  }

}