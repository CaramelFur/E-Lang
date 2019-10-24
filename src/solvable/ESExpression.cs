using E_Lang.llvm;
using E_Lang.variables;

namespace E_Lang.solvable
{
  public abstract class ESExpression
  {
    public static ESExpression CombineExpression(ESOperator op, ESExpression first, ESExpression second)
    {
      return new ESDoubleExpression(first, second, op);
    }

    public virtual EVariable Solve(LLVMHolder scope)
    {
      throw new ELangException("Cannot solve and abstract expression class");
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public virtual string ToString(bool detailed)
    {
      return "";
    }
  }

}