using System;

using E_Lang.types;
using E_Lang.variables;
using E_Lang.scope;
using E_Lang.solvable;

namespace E_Lang.functions
{
  public class EGlobalFunction : EFunction
  {
    private readonly EType type;
    private readonly EWord name;
    private readonly Func<EScope, ESolvable[], EVariable> func;

    public EGlobalFunction(EWord name, EType type, Func<EScope, ESolvable[], EVariable> func)
    {
      this.name = name;
      this.type = type;
      this.func = func;
    }

    public override EVariable Exec(EScope scope, ESolvable[] args)
    {

      EScope lowerScope = scope.GetChild();

      return func(lowerScope, args);
    }

    public override string ToString()
    {
      return "EGlobalFunction{name: " + name + ", type: " + type + "}";
    }
  }
}