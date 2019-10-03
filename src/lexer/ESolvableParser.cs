using System.Linq;
using System.Linq.Expressions;
using Sprache;

using E_Lang.solvable;
using E_Lang.types;

namespace E_Lang.lexer
{
  class ESolvableParser
  {

    static Parser<ESOperator> Operator(string op, ExpressionType opType)
    {
      return Operator(op, opType, "double");
    }

    static Parser<ESOperator> Operator(string op, ExpressionType opType, string returnType)
    {
      return Parse.String(op).Token().Return(new ESOperator(op, opType, new EType(returnType)));
    }

    static readonly Parser<ESOperator> Add = Operator("+", ExpressionType.AddChecked);
    static readonly Parser<ESOperator> Subtract = Operator("-", ExpressionType.SubtractChecked);
    static readonly Parser<ESOperator> Multiply = Operator("*", ExpressionType.MultiplyChecked);
    static readonly Parser<ESOperator> Divide = Operator("/", ExpressionType.Divide);
    static readonly Parser<ESOperator> Power = Operator("^", ExpressionType.Power);
    static readonly Parser<ESOperator> Modulo = Operator("%", ExpressionType.Modulo);

    static readonly Parser<ESOperator> And = Operator("&&", ExpressionType.AndAlso, "boolean");
    static readonly Parser<ESOperator> Or = Operator("||", ExpressionType.OrElse, "boolean");

    static readonly Parser<ESOperator> Equal = Operator("==", ExpressionType.Equal, "boolean");
    static readonly Parser<ESOperator> NotEqual = Operator("!=", ExpressionType.NotEqual, "boolean");
    static readonly Parser<ESOperator> GreaterThanOrEqual = Operator(">=", ExpressionType.GreaterThanOrEqual, "boolean");
    static readonly Parser<ESOperator> LessThanOrEqual = Operator("<=", ExpressionType.LessThanOrEqual, "boolean");
    static readonly Parser<ESOperator> GreaterThan = Operator(">", ExpressionType.GreaterThan, "boolean");
    static readonly Parser<ESOperator> LessThan = Operator("<", ExpressionType.LessThan, "boolean");

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
          from lparen in EParser.ParenthesesOpen
          from expr in Parse.Ref(() => AddSubtract)
          from rparen in EParser.ParenthesesClose
          select expr
        )
        .XOr(Number)
        .XOr(Boolean)
        .XOr(Variable)
      ).Token();

    static readonly Parser<ESExpression> Comparisons = Parse.ChainOperator(
      Equal
        .Or(NotEqual)
        .Or(GreaterThanOrEqual)
        .Or(LessThanOrEqual)
        .Or(GreaterThan)
        .Or(LessThan), 
      Expression, 
      ESExpression.CombineExpression
    );

    static readonly Parser<ESExpression> Conditionals = Parse.ChainOperator(And.Or(Or), Comparisons, ESExpression.CombineExpression);

    static readonly Parser<ESExpression> PowerRoot = Parse.ChainOperator(Power, Conditionals, ESExpression.CombineExpression);

    static readonly Parser<ESExpression> MultiplyDivideModulo = Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), PowerRoot, ESExpression.CombineExpression);

    static readonly Parser<ESExpression> AddSubtract = Parse.ChainOperator(Add.Or(Subtract), MultiplyDivideModulo, ESExpression.CombineExpression);

    public static Parser<ESolvable> ESolvable = AddSubtract.Select(solvable => new ESolvable(solvable));

    public static readonly Parser<ESExpression> Lambda =
        AddSubtract.End();

  }
}
