
using System;
using System.Linq;
using LLVMSharp;

using Sprache;

public class Parser
{
  public static Solvable ParseExpression(string text)
  {
    return Expr.Parse(text);
  }

  static Parser<TOperator> Operator(string op)
  {
    return Parse.String(op).Token().Return(new TOperator(op));
  }

  static readonly Parser<TOperator> Add = Operator("+");
  static readonly Parser<TOperator> Subtract = Operator("-");

  static readonly Parser<Solvable> Constant =
       Parse.Number
       .Select(x => new TNumber(x))
       .Named("number");

  static readonly Parser<Solvable> Factor =
      (from lparen in Parse.Char('(')
       from expr in Parse.Ref(() => Expr)
       from rparen in Parse.Char(')')
       select expr).Named("expression")
       .XOr(Constant);


  static readonly Parser<Solvable> Expr = Parse.ChainOperator(Add.Or(Subtract), Factor, (TOperator op, Solvable a, Solvable b) => new TOp(a, b, op));

}

public abstract class Solvable
{
  public virtual LLVMValueRef Solve(LLVMBuilderRef builder)
  {
    return default(LLVMValueRef);
  }
}

public class TNumber : Solvable
{
  private readonly int num;

  public TNumber(string num)
  {
    this.num = Int32.Parse(num);
  }

  public override LLVMValueRef Solve(LLVMBuilderRef builder)
  {
    return LLVM.ConstInt(LLVM.Int32Type(), (ulong)num, new LLVMBool(0));
  }

  public override string ToString()
  {
    return num.ToString();
  }
}

public class TOperator
{
  private readonly string op;
  public TOperator(string op)
  {
    this.op = op;
  }

  public string Get()
  {
    return op;
  }

  public override string ToString()
  {
    return op;
  }
}

public class TOp : Solvable
{
  private readonly Solvable a;
  private readonly Solvable b;
  private readonly TOperator op;

  public TOp(Solvable a, Solvable b, TOperator op)
  {
    this.a = a;
    this.b = b;
    this.op = op;
  }

  public override LLVMValueRef Solve(LLVMBuilderRef builder)
  {
    if (op.Get() == "+")
    {
      return LLVM.BuildAdd(builder, a.Solve(builder), b.Solve(builder), "tmp");
    }
    else if (op.Get() == "-")
    {
      return LLVM.BuildSub(builder, a.Solve(builder), b.Solve(builder), "tmp");
    }
    else
    {
      return default(LLVMValueRef);
    }
  }

  public override string ToString()
  {
    return a.ToString() + op.ToString() + b.ToString();
  }
}
