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

    static Parser<ESOperator> SimpleOperator(string op, string not, ESOSimpleType opType)
    {
      return
        from begin in
          SimpleOperator(op, opType)
        from end in
          Parse.Chars(not).Not()
        select begin;
    }

    static Parser<ESOperator> SimpleOperator(string op, ESOSimpleType opType)
    {
      return
        Parse
        .String(op)
        .Token()
        .Named("Solvable Operator")
        .Return(new ESOSimple(op, opType));
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

    static Parser<ESOperator> ConvertOperator(string op)
    {
      return
        Parse
        .String(op)
        .Token()
        .Named("Solvable Operator")
        .Return(new ESOConvert(op));
    }

    static Parser<ESOperator> OtherOperator<T>(string op)
    {
      return
        Parse
        .String(op)
        .Token()
        .Named("Solvable Operator")
        .Return((ESOperator)(Activator.CreateInstance(typeof(T), op)));
    }

    // ==== Start words

    // Simple integer operations
    static readonly Parser<ESOperator> Add = SimpleOperator("+", ESOSimpleType.Add);
    static readonly Parser<ESOperator> Subtract = SimpleOperator("-", ">", ESOSimpleType.Subtract);
    static readonly Parser<ESOperator> Multiply = SimpleOperator("*", ESOSimpleType.Multiply);
    static readonly Parser<ESOperator> Divide = SimpleOperator("/", ESOSimpleType.Divide);
    static readonly Parser<ESOperator> Power = SimpleOperator("^", ESOSimpleType.Power);
    static readonly Parser<ESOperator> Modulo = SimpleOperator("%", ESOSimpleType.Modulo);

    // Condidtional operations
    static readonly Parser<ESOperator> And = SimpleOperator("&&", ESOSimpleType.AndAlso);
    static readonly Parser<ESOperator> Or = SimpleOperator("||", ESOSimpleType.OrElse);

    // Comparison operations
    static readonly Parser<ESOperator> Equal = SimpleOperator("==", ESOSimpleType.Equal);
    static readonly Parser<ESOperator> NotEqual = SimpleOperator("!=", ESOSimpleType.NotEqual);
    static readonly Parser<ESOperator> GreaterThanOrEqual = SimpleOperator(">=", ESOSimpleType.GreaterThanOrEqual);
    static readonly Parser<ESOperator> LessThanOrEqual = SimpleOperator("<=", ESOSimpleType.LessThanOrEqual);
    static readonly Parser<ESOperator> GreaterThan = SimpleOperator(">", ESOSimpleType.GreaterThan);
    static readonly Parser<ESOperator> LessThan = SimpleOperator("<", "-", ESOSimpleType.LessThan);

    // Unary
    static readonly Parser<ESOperator> Pound = OtherOperator<ESOPointer>("#");

    // Assign operations
    static readonly Parser<ESOperator> Assign = AssignOperator("=>", AssignType.Assign);
    static readonly Parser<ESOperator> MoveOp = AssignOperator("->", AssignType.Move);

    // Convert operations
    static readonly Parser<ESOperator> CastOp = ConvertOperator("@");

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

    // Parse a type, this is a simple word
    static readonly Parser<ESType> Type =
      (
        from type in EParser.ParseEType
        select new ESType(type)
      ).Named("Type");

    // These are the arguments passed while calling a function
    // They consist of zero or more solvables seperated by commas
    // And they are contained by braces
    static readonly Parser<ESExpression[]> CallArguments =
      Parse.Or(
        Parse.Ref(() => BeginExpression)
          .Select(e => new ESExpression[] { e })
          .Named("Call Argument"),
        (
          from open in EParser.BraceOpen
          from expressionArgs in
            Parse.Ref(() => BeginExpression)
            .Named("Call Argument")
            .DelimitedBy(EParser.Comma)
            .Optional()
          from close in EParser.BraceClose
          select expressionArgs.IsDefined ? expressionArgs.Get().ToArray() : new ESExpression[] { }
        )
      ).Named("Call Arguments");

    // Parses a call operation, this operation solves its given arguments and then call the specified function
    // It can also immediately assign its result to a new variable
    public static readonly Parser<ESExpression> FuncCall =
      (
        from word in EParser.Word
        from arrow in EParser.ArrowLeft
        from arguments in CallArguments
        select new ESCall(word, arguments)
      ).Named("Call Operation");

    // === Start operations

    static readonly Parser<ESExpression> SimpleSubExpression =
      FuncCall
      .Or(Number)
      .Or(Boolean)
      .Or(Type)
      .Or(Variable)
      .Token()
      .Named("Simple Expression");

    static readonly Parser<ESExpression> UnarySubExpression =
      (
        from op in Pound
        from expr in SimpleSubExpression
        select new ESSingleExpression(expr, op)
      ).Named("Unary Expression");



    // Parse an expression, this can be:
    // - Another expression surrounded by parentheses
    // - A number
    // - A boolean
    // - A variable
    static Parser<ESExpression> SubExpression(Parser<ESExpression> input) =>
      (
        (
          from lparen in EParser.ParenthesesOpen
          from expr in Parse.Ref(() => BeginExpression)
          from rparen in EParser.ParenthesesClose
          select expr
        )
        .Or(UnarySubExpression)
        .Or(SimpleSubExpression)
      )
      .Token()
      .Named("Solvable Expression");

    // Parse convert operations
    static Parser<ESExpression> Casts(Parser<ESExpression> input) => Parse.ChainOperator(
      CastOp,
      input,
      ESExpression.CombineExpression
    );

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
      Parse.ChainOperator(MoveOp.Or(Assign), input, ESExpression.CombineExpression);

    static Parser<ESExpression> BeginExpressionFactory()
    {
      Func<Parser<ESExpression>, Parser<ESExpression>>[] functions =
        new Func<Parser<ESExpression>, Parser<ESExpression>>[] {
          SubExpression,
          Casts,
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

    static readonly Parser<ESExpression> BeginExpression = BeginExpressionFactory();

    // Fully parse a solvable
    public static Parser<ESolvable> ESolvable =
      BeginExpression
      .Named("Solvable")
      .Select(solvable => new ESolvable(solvable));
  }
}
