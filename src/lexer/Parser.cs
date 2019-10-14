using System;
using System.Linq;
using Sprache;

using E_Lang.types;
using E_Lang.operations;
using E_Lang.solvable;

namespace E_Lang.lexer
{
  public class EParser
  {
    // Base characters
    public static readonly Parser<string> Space = Parse.WhiteSpace.AtLeastOnce().Text().Named("Space");
    public static readonly Parser<string> ColonEquals = Parse.Token(Parse.String(":=")).Text().Named("SetColon");
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

    // Simple word parser
    public static readonly Parser<EWord> Word =
      (
        from word in SimpleWord
        select new EWord(word)
      ).Named("EWord");

    // Arrow parser
    public static readonly Parser<EToken> ArrowLeft =
      (
        from arrow in Parse.String("<-").Token().Text()
        select new EToken(arrow)
      ).Named("Left Arrow");
    public static readonly Parser<EToken> ArrowRight =
      (
        from arrow in Parse.String("->").Token().Text()
        select new EToken(arrow)
      ).Named("Right Arrow");

    // Comment parser
    static readonly CommentParser Comment = new CommentParser("//", "/*", "*/", "\n");

    static Parser<EType> ParseETypeFactory()
    {
      EType[] types = Enum.GetValues(typeof(EType)).Cast<EType>().ToArray();

      Parser<EType> buffer = null;

      foreach (EType type in types)
      {
        string stringType = type.ToString().ToLower();

        if (buffer == null)
        {
          buffer = Parse.String(stringType).Select((s) => type);
        }
        else
        {
          buffer = buffer.Or(
            Parse.String(stringType).Select((s) => type)
          );
        }
      }

      return buffer;
    }

    public static readonly Parser<EType> ParseEType = ParseETypeFactory();

    // Parser for types that you assign and create
    static readonly Parser<ETypeWord> CreateableType =
      (
        from typeName in ParseEType
        select new ETypeWord(typeName)
      ).Named("Createable Type");

    // Parser for all types
    static readonly Parser<ETypeWord> Type = CreateableType.Named("EType"); //.Or(UsableType);

    // A reference to the solvable in ESolvableParser.cs
    static readonly Parser<ESolvable> Solvable = Parse.Ref(() => ESolvableParser.ESolvable);

    // Parse a subprogram a.k.a. a function
    static readonly Parser<EOperation[]> SubProgram =
      (
        from operations in
          Parse.Ref(() => Operations)
            .Select(o => new EOperation[] { o })
          .Or(
            Parse.Ref(() => Operations)
            .Many()
            .Contained(BraceOpen, BraceClose)
          )
        select operations.ToArray()
      ).Named("Sub Program");

    // Parse a type name key, this is a type with a colon and then a name
    static readonly Parser<ETypeNameKey> TypeNameKey =
      (
        from type in Type
        from colon in Colon
        from name in Word
        select new ETypeNameKey(name, type)
      ).Named("Function Argument");

    // Parse multiple type name keys
    // these are single type name keys seperated by commas and surrounded by braces
    static readonly Parser<ETypeNameKey[]> TypeNameKeys =
      (
        (
          from arguments in TypeNameKey
          .DelimitedBy(Comma)
          .Optional()
          .Contained(ParenthesesOpen, ParenthesesClose)
          select arguments.IsDefined ? arguments.Get().ToArray() : new ETypeNameKey[] { }
        )
        .Or(
          from argument in TypeNameKey.Token()
          select new ETypeNameKey[] { argument }
        )
      ).Named("Function Arguments");

    // Parses a create variable operation
    static readonly Parser<ECreateOperation> CreateOperation =
      (
        from keyword in Parse.String("create")
        from space in Space
        from type in CreateableType
        from colon in Parse.Token(Parse.Char(':'))
        from names in Word.DelimitedBy(Comma)
        from assign in (
          from set in ColonEquals.Token()
          from solvable in Solvable
          select solvable
        ).Optional()
        from semicolon in EndLine
        select
          assign.IsDefined ?
          new ECreateOperation(names.ToArray(), type, assign.Get()) :
          new ECreateOperation(names.ToArray(), type)
      ).Named("Create Operation");

    // Parses an if statement, it does not have an else though
    static readonly Parser<EIfOperation> IfOperation =
      (
        from keyword in Parse.String("if")
        from solvable in Solvable
        from set in ColonEquals
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
    public static readonly Parser<EWhileOperation> WhileOperation =
      (
        from keyword in Parse.String("while")
        from solvable in Solvable
        from set in ColonEquals
        from operations in SubProgram
        select new EWhileOperation(solvable, operations)
      ).Named("While Operation");

    // Parses a function operation, this creates a new function in the current scope
    static readonly Parser<EFunctionOperation> FunctionOperation =
      (
        from keyword in Parse.String("function")
        from arguments in TypeNameKeys
        from rightarrow in ArrowRight
        from type in CreateableType
        from colon in Colon
        from name in Word
        from set in ColonEquals
        from operations in SubProgram
        select new EFunctionOperation(name, type, arguments, operations)
      ).Named("Function Operation");

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
         .Or<EOperation>(IfOperation)
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
