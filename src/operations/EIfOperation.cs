using System;

using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;
using E_Lang.interpreter;

namespace E_Lang.operations
{
  // This operation solves a solvable and if it is true executes its code
  public class EIfOperation : EOperation
  {
    private readonly ESolvable check;
    private readonly EProgram program;
    private readonly EProgram elseProgram = null;

    public EIfOperation(ESolvable check, EOperation[] operations)
    {
      this.check = check;
      program = new EProgram(operations);
    }

    public EIfOperation(ESolvable check, EOperation[] operations, EOperation[] elseOperations) :
    this(check, operations)
    {
      elseProgram = new EProgram(elseOperations);
    }

    public override string ToString()
    {
      return "EIfOperation{\ncheck: " + check + ";\n" + program + "\n}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVBoolean solved = (EVBoolean)check.Solve(scope).Convert(new EType("boolean"));

      if (solved.Get())
      {
        EScope subScope = scope.GetChild();
        return Interpreter.Run(program, subScope);
      }
      if (elseProgram != null)
      {
        EScope subScope = scope.GetChild();
        return Interpreter.Run(elseProgram, subScope);
      }
      return new EVVoid();

    }
  }

}