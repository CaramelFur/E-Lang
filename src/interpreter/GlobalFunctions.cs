using System;
using System.Linq;

using E_Lang.scope;
using E_Lang.functions;
using E_Lang.types;
using E_Lang.solvable;
using E_Lang.variables;


namespace E_Lang.interpreter
{
  public class GlobalFunctions
  {
    public static void Add(EScope scope)
    {
      scope.SetFunction("log",
        new EGlobalFunction(
          new EWord("log"),
          new EType("void"),
          (EScope subScope, ESolvable[] args) => {
            EVariable[] variables = args.Select(arg => arg.Solve(subScope)).ToArray();
            string toLog = variables.Aggregate("", (prev, next) => {
              if(prev.Length == 0) return prev + next;
              return prev + " " + next;
            });
            Console.WriteLine(toLog);
            return new EVVoid();
          }
        )
      );
    }
  }
}