using System.Linq;
using System.Linq.Expressions;
using Sprache;

using E_Lang.solvable;

namespace E_Lang.lexer
{
  class ESolvableParser
  {
    static Parser<ESOperator> Operator(string op, ExpressionType opType)
    {
      return Parse.String(op).Token().Return(new ESOperator(op, opType));
    }

    static readonly Parser<ESOperator> Add = Operator("+", ExpressionType.AddChecked);
    static readonly Parser<ESOperator> Subtract = Operator("-", ExpressionType.SubtractChecked);
    static readonly Parser<ESOperator> Multiply = Operator("*", ExpressionType.MultiplyChecked);
    static readonly Parser<ESOperator> Divide = Operator("/", ExpressionType.Divide);

    static readonly Parser<ESNumber> Number =
      (
        from dec in Parse.Decimal.Token()
        select new ESNumber(decimal.Parse(dec))
      ).Named("number");

    static readonly Parser<ESBoolean> Boolean =
      (
        from boolean in Parse.String("true").Or(Parse.String("false")).Text()
        select new ESBoolean(boolean == "true")
      ).Named("boolean");

    static readonly Parser<ESVariable> Variable =
      from word in EParser.SimpleWord
      select new ESVariable(word);

    static readonly Parser<ESExpression> Expression =
      (
        (
          from lparen in EParser.BraceOpen
          from expr in Parse.Ref(() => AddSubtract)
          from rparen in EParser.BraceClose
          select expr
        )
        .XOr(Number)
        .XOr(Boolean)
        .XOr(Variable)
      ).Token();


    static readonly Parser<ESExpression> MultiplyDivide = Parse.ChainOperator(Multiply.Or(Divide), Expression, ESExpression.CombineExpression);

    static readonly Parser<ESExpression> AddSubtract = Parse.ChainOperator(Add.Or(Subtract), MultiplyDivide, ESExpression.CombineExpression);

    public static Parser<ESolvable> ESolvable = AddSubtract.Select(solvable => new ESolvable(solvable));

    public static readonly Parser<ESExpression> Lambda =
        AddSubtract.End();

  }
}
