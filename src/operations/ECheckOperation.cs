using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;
using E_Lang.interpreter;


namespace E_Lang.operations
{
  // This operation solves a solvable and if it is true executes its code
  public class ECheckOperation : EOperation
  {
    private readonly ESolvable check;
    private readonly EProgram program;

    public ECheckOperation(ESolvable check, EOperation[] operations)
    {
      this.check = check;
      program = new EProgram(operations);
    }

    public override string ToString()
    {
      return "ECheckOperation{\ncheck: " + check + ";\n" + program + "\n}";
    }

    public override EVariable Exec(EScope scope)
    {
      EVBoolean solved = (EVBoolean) check.Solve(scope).Convert(new EType("boolean"));

      if (solved.Get())
      {
        EScope subScope = scope.GetChild();
        return Interpreter.Run(program, subScope);
      }
      return new EVVoid();
    }
  }

}