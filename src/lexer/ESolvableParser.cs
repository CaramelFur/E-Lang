using System.Linq;
using System.Linq.Expressions;
using Sprache;

using E_Lang.solvable;
using E_Lang.types;

namespace E_Lang.lexer
{
  class ESolvableParser
  {
    // Parse a math token and return the appropiate operator
    static Parser<ESOperator> Operator(string op, ExpressionType opType, string returnType)
    {
      return Parse.String(op).Token().Named("Solvable Operator").Return(new ESOperator(op, opType, new EType(returnType)));
    }

    static Parser<ESOperator> Operator(string op, ExpressionType opType)
    {
      return Operator(op, opType, "double");
    }

    // Simple integer operations
    static readonly Parser<ESOperator> Add = Operator("+", ExpressionType.AddChecked);
    static readonly Parser<ESOperator> Subtract = Operator("-", ExpressionType.SubtractChecked);
    static readonly Parser<ESOperator> Multiply = Operator("*", ExpressionType.MultiplyChecked);
    static readonly Parser<ESOperator> Divide = Operator("/", ExpressionType.Divide);
    static readonly Parser<ESOperator> Power = Operator("^", ExpressionType.Power);
    static readonly Parser<ESOperator> Modulo = Operator("%", ExpressionType.Modulo);

    // Condidtional operations
    static readonly Parser<ESOperator> And = Operator("&&", ExpressionType.AndAlso, "boolean");
    static readonly Parser<ESOperator> Or = Operator("||", ExpressionType.OrElse, "boolean");

    // Comparison operations
    static readonly Parser<ESOperator> Equal = Operator("==", ExpressionType.Equal, "boolean");
    static readonly Parser<ESOperator> NotEqual = Operator("!=", ExpressionType.NotEqual, "boolean");
    static readonly Parser<ESOperator> GreaterThanOrEqual = Operator(">=", ExpressionType.GreaterThanOrEqual, "boolean");
    static readonly Parser<ESOperator> LessThanOrEqual = Operator("<=", ExpressionType.LessThanOrEqual, "boolean");
    static readonly Parser<ESOperator> GreaterThan = Operator(">", ExpressionType.GreaterThan, "boolean");
    static readonly Parser<ESOperator> LessThan = Operator("<", ExpressionType.LessThan, "boolean");

    // Parse a decimal number
    static readonly Parser<ESNumber> Number =
      (
        from dec in Parse.Decimal.Token()
        select new ESNumber(decimal.Parse(dec))
      ).Named("Solvable Number");

    // Parse a boolean
    static readonly Parser<ESBoolean> Boolean =
      (
        from boolean in Parse.String("true").Or(Parse.String("false")).Text()
        select new ESBoolean(boolean == "true")
      ).Named("Solvable Boolean");

    // Parse a variable, this is a simple word
    static readonly Parser<ESVariable> Variable =
      (
        from word in EParser.SimpleWord
        select new ESVariable(word)
      ).Named("Solvable Variable");

    // Parse an expression, this can be:
    // - Another expression surrounded by parentheses
    // - A number
    // - A boolean
    // - A variable
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
      )
      .Token()
      .Named("Solvable Expression");

    // Parse all comparison operations
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

    // Parse all conditional operations
    static readonly Parser<ESExpression> Conditionals = Parse.ChainOperator(And.Or(Or), Comparisons, ESExpression.CombineExpression);

    // Parse things like power and root
    static readonly Parser<ESExpression> PowerRoot = Parse.ChainOperator(Power, Conditionals, ESExpression.CombineExpression);

    // Parse things like multiply and divide
    static readonly Parser<ESExpression> MultiplyDivideModulo = Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), PowerRoot, ESExpression.CombineExpression);

    // Parse the simple math operations
    static readonly Parser<ESExpression> AddSubtract = Parse.ChainOperator(Add.Or(Subtract), MultiplyDivideModulo, ESExpression.CombineExpression);

    // Fully parse a solvable
    public static Parser<ESolvable> ESolvable = 
      AddSubtract
      .Named("Solvable")
      .Select(solvable => new ESolvable(solvable));
  }
}
