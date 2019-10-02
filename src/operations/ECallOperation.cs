using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;
using E_Lang.functions;

namespace E_Lang.operations
{
  // This operation calls a function and return the result
  // It can also assign the result to a variable
  public class ECallOperation : EOperation
  {
    private readonly EWord callFunc;
    private readonly ESolvable[] arguments = { };
    private readonly EWord setVariable;
    private readonly bool alsoSet = false;

    public ECallOperation(EWord callFunc, ESolvable[] arguments)
    {
      this.callFunc = callFunc;
      this.arguments = arguments;
    }

    public ECallOperation(EWord callFunc, ESolvable[] arguments, EWord setVariable) : this(callFunc, arguments)
    {
      this.setVariable = setVariable;
      alsoSet = true;
    }

    public override string ToString()
    {
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }
      if (!alsoSet) return "ECall{function: " + callFunc + ", arguments: " + "(" + argString + ")}";
      else return "ECallAndAssign{function: " + callFunc + ", arguments: " + "(" + argString + "), assign: " + setVariable + "}";
    }

    public override EVariable Exec(EScope scope)
    {
      EFunction toRun = scope.GetFunction(callFunc.ToString());

      EVariable output = toRun.Exec(scope, arguments);

      if (alsoSet)
      {
        EVariable toUpdate = scope.Get(setVariable.ToString());
        toUpdate.Assign(output);
      }

      return output;
    }
  }

}