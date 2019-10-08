using System;
using System.Linq.Expressions;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{

  public class ESOConvert : ESOperator
  {

    public ESOConvert(string op) :
      base(op, null)
    {
    }

    public override EVariable Solve(EVariable first, EVariable second)
    {
      return first.Convert(second.GetEType());
    }
  }

}