using System.Linq;

using E_Lang.types;
using E_Lang.llvm;
using E_Lang.variables;
using E_Lang.functions;

namespace E_Lang.solvable
{
  public class ESCall : ESExpression
  {
    private readonly EWord callFunc;
    private readonly ESolvable[] arguments = { };

    public ESCall(EWord callFunc, ESExpression[] arguments)
    {
      this.callFunc = callFunc;
      this.arguments = arguments.Select(arg => new ESolvable(arg)).ToArray();
    }

    public override EVariable Solve(LLVMHolder llvm)
    {
      EVariable[] solved = arguments.Select(a => a.Solve(llvm)).ToArray();
      return EFunctions.ExecFunc(llvm, callFunc.ToString(), solved);
    }

    public override string ToString(bool detailed)
    {
      string argString = "";
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i != 0) argString += ", ";
        argString += arguments[i].ToString();
      }

      if (detailed) return "ESCall[" + callFunc + ", (" + argString + ")]";
      else return "{" + argString + "} -> " + callFunc;
    }

  }

}