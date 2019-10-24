using LLVMSharp;
using E_Lang.llvm;
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

    public ESOAssign(string op, AssignType type) : base(op)
    {
      this.type = type;
    }

    public override EVariable Solve(LLVMHolder llvm, EVariable first, EVariable second)
    {

      switch (type)
      {
        case AssignType.Assign:
          second.Assign(first);
          return second;
        case AssignType.Move:
          EVariable temp = second.Clone();
          second.Assign(first);
          return temp;
        default:
          throw new ELangException("Invalid Assigntype: " + type);
      }
    }
  }

}