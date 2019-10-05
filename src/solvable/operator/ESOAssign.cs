using System;
using System.Linq.Expressions;

using E_Lang.types;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public enum AssignType
  {
    Assign,
    Move
  }

  public class ESOAssign : ESOperator
  {
    private readonly AssignType type;

    public ESOAssign(string op, AssignType type) :
      base(op, null)
    {
      this.type = type;
    }

    public override EVariable Solve(EVariable first, EVariable second)
    {

      switch (type)
      {
        case AssignType.Assign:
          first.Assign(second);
          return first;
        case AssignType.Move:
          EVariable temp = first.Clone();
          first.Assign(second);
          return temp;
        default:
          throw new Exception("Invalid Assigntype: " + type);
      }
    }
  }

}