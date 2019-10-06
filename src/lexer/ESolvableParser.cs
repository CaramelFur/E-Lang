using System;
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
    static Parser<ESOperator> SimpleOperator(string op, string not, ExpressionType opType, string returnType)
    {
      return
        from begin in
          SimpleOperator(op, opType, returnType)
        from end in
          Parse.String(not).Not()
        select begin;
    }

    static Parser<ESOperator> SimpleOperator(string op, string not, ExpressionType opType)
    {
      return SimpleOperator(op, not, opType, "double");
    }

    static Parser<ESOperator> SimpleOperator(string op, ExpressionType opType, string returnType)
    {
      return
        Parse
        .String(op)
        .Token()
        .Named("Solvable Operator")
        .Return(new ESOLinq(op, opType, new EType(returnType)));
    }

    static Parser<ESOperator> SimpleOperator(string op, ExpressionType opType)
    {
      return SimpleOperator(op, opType, "double");
    }

    static Parser<ESOperator> AssignOperator(string op, AssignType opType)
    {
      return
        Parse
        .String(op)
        .Token()
        .Named("Solvable Operator")
        .Return(new ESOAssign(op, opType));
    }

    // ==== Start words

    // Simple integer operations
    static readonly Parser<ESOperator> Add = SimpleOperator("+", ExpressionType.AddChecked);
    static readonly Parser<ESOperator> Subtract = SimpleOperator("-", ExpressionType.SubtractChecked);
    static readonly Parser<ESOperator> Multiply = SimpleOperator("*", ExpressionType.MultiplyChecked);
    static readonly Parser<ESOperator> Divide = SimpleOperator("/", ExpressionType.Divide);
    static readonly Parser<ESOperator> Power = SimpleOperator("^", ExpressionType.Power);
    static readonly Parser<ESOperator> Modulo = SimpleOperator("%", ExpressionType.Modulo);

    // Condidtional operations
    static readonly Parser<ESOperator> And = SimpleOperator("&&", ExpressionType.AndAlso, "boolean");
    static readonly Parser<ESOperator> Or = SimpleOperator("||", ExpressionType.OrElse, "boolean");

    // Comparison operations
    static readonly Parser<ESOperator> Equal = SimpleOperator("==", ExpressionType.Equal, "boolean");
    static readonly Parser<ESOperator> NotEqual = SimpleOperator("!=", ExpressionType.NotEqual, "boolean");
    static readonly Parser<ESOperator> GreaterThanOrEqual = SimpleOperator(">=", ExpressionType.GreaterThanOrEqual, "boolean");
    static readonly Parser<ESOperator> LessThanOrEqual = SimpleOperator("<=", ExpressionType.LessThanOrEqual, "boolean");
    static readonly Parser<ESOperator> GreaterThan = SimpleOperator(">", ExpressionType.GreaterThan, "boolean");
    static readonly Parser<ESOperator> LessThan = SimpleOperator("<", "-", ExpressionType.LessThan, "boolean");

    // Assign operations
    static readonly Parser<ESOperator> Assign = AssignOperator("=", AssignType.Assign);
    static readonly Parser<ESOperator> MoveOp = AssignOperator("<-", AssignType.Move);

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

    // These are the arguments passed while calling a function
    // They consist of zero or more solvables seperated by commas
    // And they are contained by braces
    static readonly Parser<ESExpression[]> CallArguments =
      (
        from open in EParser.BraceOpen
        from expressionArguments in
          BeginExpression()
          .Named("Call Argument")
          .DelimitedBy(EParser.Comma)
          .Optional()
        from close in EParser.BraceClose
        select expressionArguments.IsDefined ? expressionArguments.Get().ToArray() : new ESExpression[] { }
      ).Named("Call Arguments");

    // Parses a call operation, this operation solves its given arguments and then call the specified function
    // It can also immediately assign its result to a new variable
    public static readonly Parser<ESExpression> FuncCall =
      (
        from arguments in CallArguments
        from arrow in EParser.ArrowRight
        from word in EParser.Word
        select new ESCall(word, arguments)
      ).Named("Call Operation");

    // === Start operations

    // Parse an expression, this can be:
    // - Another expression surrounded by parentheses
    // - A number
    // - A boolean
    // - A variable
    static Parser<ESExpression> SubExpression(Parser<ESExpression> input) =>
      (
        (
          from lparen in EParser.ParenthesesOpen
          from expr in Parse.Ref(() => BeginExpression())
          from rparen in EParser.ParenthesesClose
          select expr
        )
        .Or(FuncCall)
        .Or(Number)
        .Or(Boolean)
        .Or(Variable)
      )
      .Token()
      .Named("Solvable Expression");

    // Parse all comparison operations
    static Parser<ESExpression> Comparisons(Parser<ESExpression> input) => Parse.ChainOperator(
      Equal
        .Or(NotEqual)
        .Or(GreaterThanOrEqual)
        .Or(LessThanOrEqual)
        .Or(GreaterThan)
        .Or(LessThan),
      input,
      ESExpression.CombineExpression
    );

    // Parse all conditional operations
    static Parser<ESExpression> Conditionals(Parser<ESExpression> input) =>
      Parse.ChainOperator(And.Or(Or), input, ESExpression.CombineExpression);

    // Parse things like power and root
    static Parser<ESExpression> PowerRoot(Parser<ESExpression> input) =>
      Parse.ChainOperator(Power, input, ESExpression.CombineExpression);

    // Parse things like multiply and divide
    static Parser<ESExpression> MultiplyDivideModulo(Parser<ESExpression> input) =>
      Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), input, ESExpression.CombineExpression);

    // Parse the simple math operations
    static Parser<ESExpression> AddSubtract(Parser<ESExpression> input) =>
      Parse.ChainOperator(Add.Or(Subtract), input, ESExpression.CombineExpression);

    // Parse assing operations
    // This operation is right associative
    static Parser<ESExpression> AssignMove(Parser<ESExpression> input) =>
      Parse.ChainRightOperator(MoveOp.Or(Assign), input, ESExpression.CombineExpression);

    static Parser<ESExpression> BeginExpression()
    {
      Func<Parser<ESExpression>, Parser<ESExpression>>[] functions =
        new Func<Parser<ESExpression>, Parser<ESExpression>>[] {
          SubExpression,
          PowerRoot,
          MultiplyDivideModulo,
          AddSubtract,
          Comparisons,
          Conditionals,
          AssignMove
        };

      Parser<ESExpression> accumulated = functions.Aggregate((Parser<ESExpression>)null, (prev, next) => next(prev));

      return accumulated;
    }

    // Fully parse a solvable
    public static Parser<ESolvable> ESolvable =
      BeginExpression()
      .Named("Solvable")
      .Select(solvable => new ESolvable(solvable));
  }
}
