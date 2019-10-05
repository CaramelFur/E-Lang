using System.Linq;
using Sprache;

using E_Lang.types;
using E_Lang.operations;
using E_Lang.functions;
using E_Lang.solvable;

namespace E_Lang.lexer
{
  public class EParser
  {
    // Base characters
    public static readonly Parser<string> Space = Parse.Text(Parse.WhiteSpace.AtLeastOnce()).Named("Space");
    public static readonly Parser<char> Colon = Parse.Token(Parse.Char(':')).Named("Colon");
    public static readonly Parser<char> Comma = Parse.Token(Parse.Char(',')).Named("Comma");
    public static readonly Parser<char> EndLine = Parse.Token(Parse.Char(';')).Named("Semicolon");
    public static readonly Parser<char> Exclamation = Parse.Token(Parse.Char('!')).Named("Exclamation");

    // Opening and closing characters
    public static readonly Parser<char> BraceOpen = Parse.Token(Parse.Char('{')).Named("Opening Brace");
    public static readonly Parser<char> BraceClose = Parse.Token(Parse.Char('}')).Named("Closing Brace");
    public static readonly Parser<char> ParenthesesOpen = Parse.Token(Parse.Char('(')).Named("Opening Parentheses");
    public static readonly Parser<char> ParenthesesClose = Parse.Token(Parse.Char(')')).Named("Closing Parentheses");
    public static readonly Parser<char> BracketOpen = Parse.Token(Parse.Char('[')).Named("Opening Bracket");
    public static readonly Parser<char> BracketClose = Parse.Token(Parse.Char(']')).Named("Closing Bracket");

    // Parse a simple word, these consist of: `A-Za-z0-9` and `-_`
    public static readonly Parser<string> SimpleWord = Parse
      .LetterOrDigit
      .XOr(Parse.Char('-'))
      .XOr(Parse.Char('_'))
      .AtLeastOnce()
      .Named("Word")
      .Text();

    // Comment parser
    static readonly CommentParser Comment = new CommentParser("#", "###", "###", "\n");

    // Simple word parser
    static readonly Parser<EWord> Word =
      (
        from word in SimpleWord
        select new EWord(word)
      ).Named("EWord");

    // Arrow parser
    static readonly Parser<EToken> ArrowLeft =
      (
        from arrow in Parse.String("<-").Token().Text()
        select new EToken(arrow)
      ).Named("Left Arrow");
    static readonly Parser<EToken> ArrowRight =
      (
        from arrow in Parse.String("->").Token().Text()
        select new EToken(arrow)
      ).Named("Right Arrow");

    // Parser for types that you assign and create
    static readonly Parser<EType> CreateableType =
      (
        from typeName in
          Parse.String("int")
          .Or(Parse.String("double"))
          .Or(Parse.String("boolean"))
          .Or(Parse.String("void"))
          .Text()
        select new EType(typeName)
      ).Named("Createable Type");
    // Parser for the other types (Disabled for now)
    static readonly Parser<EType> UsableType =
      (
        from typeName in Parse.String("Function").Text()
        select new EType(typeName)
      ).Named("Usable Type");
    // Parser for all types
    static readonly Parser<EType> Type = CreateableType.Named("EType"); //.Or(UsableType);

    // A reference to the solvable in ESolvableParser.cs
    static readonly Parser<ESolvable> Solvable = Parse.Ref(() => ESolvableParser.ESolvable);

    // Parse a subprogram a.k.a. a function
    static readonly Parser<EOperation[]> SubProgram =
      (
        from operations in
          Parse.Ref(() => Operations)
            .Select(o => new EOperation[] {o})
          .Or(
            Parse.Ref(() => Operations)
            .Many()
            .Contained(BraceOpen, BraceClose)
          )
        select operations.ToArray()
      ).Named("Sub Program");

    // Parse a function argument, this is a type with a colon and then a name
    static readonly Parser<EFunctionArgument> FunctionArgument =
      (
        from type in Type
        from colon in Colon
        from name in Word
        select new EFunctionArgument(name, type)
      ).Named("Function Argument");

    // Parse multiple function arguments
    // these are single function arguments seperated by commas and surrounded by braces
    static readonly Parser<EFunctionArgument[]> FunctionArguments =
      (
        from arguments in FunctionArgument
          .DelimitedBy(Comma)
          .Optional()
          .Contained(ParenthesesOpen, ParenthesesClose)
        select arguments.IsDefined ? arguments.Get().ToArray() : new EFunctionArgument[] { }
      ).Named("Function Arguments");

    // These are the arguments passed while calling a function
    // They consist of zero or more solvables seperated by commas
    // And they are contained by braces
    static readonly Parser<ESolvable[]> CallArguments =
      (
        from open in BraceOpen
        from solvable in
          Solvable
          .Named("Call Argument")
          .DelimitedBy(Comma)
          .Optional()
        from close in BraceClose
        select solvable.IsDefined ? solvable.Get().ToArray() : new ESolvable[] { }
      ).Named("Call Arguments");

    // Parses a create variable operation
    static readonly Parser<ECreateOperation> CreateOperation =
      (
        from keyword in Parse.String("create")
        from space in Space
        from type in CreateableType
        from colon in Parse.Token(Parse.Char(':'))
        from name in Word
        from assign in (
          from arrow in ArrowLeft
          from solvable in Solvable
          select solvable
        ).Optional()
        from semicolon in EndLine
        select
          assign.IsDefined ?
          new ECreateOperation(name, type, assign.Get()) :
          new ECreateOperation(name, type)
      ).Named("Create Operation");

    // Parses an if statement, it does not have an else though
    static readonly Parser<EIfOperation> IfOperation =
      (
        from keyword in Parse.String("if")
        from solvable in Solvable
        from arrow in ArrowRight
        from operations in SubProgram
        from elseOperations in (
          from elseWord in Parse.String("else").Token()
          from elseOps in SubProgram
          select elseOps
        ).Named("Else Operations").Optional()
        select
          elseOperations.IsDefined ?
          new EIfOperation(solvable, operations, elseOperations.Get()) :
          new EIfOperation(solvable, operations)
      ).Named("If Operation");

    // Parses a while statement, this is a looping if statement
    static readonly Parser<EWhileOperation> WhileOperation =
      (
        from keyword in Parse.String("while")
        from solvable in Solvable
        from arrow in ArrowRight
        from operations in SubProgram
        select new EWhileOperation(solvable, operations)
      ).Named("While Operation");

    // Parses a function operation, this creates a new function in the current scope
    static readonly Parser<EFunctionOperation> FunctionOperation =
      (
        from keyword in Parse.String("function")
        from arguments in FunctionArguments
        from rightarrow in ArrowRight
        from type in CreateableType
        from colon in Colon
        from name in Word
        from leftarrow in ArrowLeft
        from operations in SubProgram
        select new EFunctionOperation(name, type, arguments, operations)
      ).Named("Function Operation");

    // Parses a call operation, this operation solves its given arguments and then call the specified function
    // It can also immediately assign its result to a new variable
    public static readonly Parser<ECallOperation> CallOperation =
      (
        from arguments in CallArguments
        from arrow in ArrowRight
        from word in Word
        from assign in
          (
            from secondArrow in ArrowRight
            from variable in Word
            select variable
          )
          .Named("Call Assign Operation")
          .Optional()
        from semicolon in EndLine
        select
          assign.IsDefined ?
          new ECallOperation(word, arguments, assign.Get()) :
          new ECallOperation(word, arguments)
      ).Named("Call Operation");

    // Parses an expression operation, this is just a blank expression followed by a semicolon
    // It returns its calculated result
    static readonly Parser<EExpressionOperation> ExpressionOperation =
      (
        from solvable in Solvable
        from semicolon in EndLine
        select new EExpressionOperation(solvable)
      ).Named("Expression Operation");

    // Parses different types of operations
    static readonly Parser<EOperation> EOperation =
        CreateOperation
         .Or<EOperation>(CallOperation)
         .Or(IfOperation)
         .Or(WhileOperation)
         .Or(FunctionOperation)
         .Or(ExpressionOperation);

    // Parses comments
    static readonly Parser<EOperation> CommentOperation =
      (
        from comment in Comment.MultiLineComment.Or(Comment.SingleLineComment).Token()
        select new ENoOperation()
      ).Named("Comment Operation");

    // Parses
    static readonly Parser<EOperation> Operations =
      from comment in CommentOperation.Many().Optional()
      from operation in EOperation
      select operation;

    // Parses the whole program
    public static Parser<EProgram> Program =
      (
        from space in Parse.WhiteSpace.Many()
        from operations in
          Operations
          .Many()
          .Named("Operations")
        from comment in
          CommentOperation
          .Many()
          .Optional()
          .End()
        select new EProgram(operations.ToArray())
      ).Named("Program");
  }
}
