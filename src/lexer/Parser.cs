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
    public static readonly Parser<string> Space = Parse.Text(Parse.WhiteSpace.AtLeastOnce());
    public static readonly Parser<char> Colon = Parse.Token(Parse.Char(':'));
    public static readonly Parser<char> Comma = Parse.Token(Parse.Char(','));
    public static readonly Parser<char> EndLine = Parse.Token(Parse.Char(';'));

    public static readonly Parser<char> BraceOpen = Parse.Token(Parse.Char('{'));
    public static readonly Parser<char> BraceClose = Parse.Token(Parse.Char('}'));
    public static readonly Parser<char> RoundBraceOpen = Parse.Token(Parse.Char('('));
    public static readonly Parser<char> RoundBraceClose = Parse.Token(Parse.Char(')'));
    public static readonly Parser<char> BracketOpen = Parse.Token(Parse.Char('['));
    public static readonly Parser<char> BracketClose = Parse.Token(Parse.Char(']'));

    public static readonly Parser<string> SimpleWord = Parse
      .LetterOrDigit
      .XOr(
        Parse.Char('-')
      ).XOr(
        Parse.Char('_')
      ).AtLeastOnce().Text();

    // Comment parser
    static readonly CommentParser Comment = new CommentParser("#", "###", "###", "\n");

    // Simple word parser
    static readonly Parser<EWord> Word =
      from word in SimpleWord
      select new EWord(word);

    // Arrow parser
    static readonly Parser<EToken> ArrowLeft =
      from arrow in Parse.String("<-").Token().Text()
      select new EToken(arrow);
    static readonly Parser<EToken> ArrowRight =
      from arrow in Parse.String("->").Token().Text()
      select new EToken(arrow);

    // Parser for types that you assign and create
    static readonly Parser<EType> CreateableType =
      from typeName in Parse.String("int")
      .Or(Parse.String("double"))
      .Or(Parse.String("boolean"))
      .Or(Parse.String("void"))
      .Text()
      select new EType(typeName);
    // Parser for the other types
    static readonly Parser<EType> UsableType =
      from typeName in Parse.String("Function").Text()
      select new EType(typeName);
    // Parser for all types
    static readonly Parser<EType> Type = CreateableType.Or(UsableType);

    public static Parser<ESolvable> Solvable = Parse.Ref(() => ESolvableParser.ESolvable);

    // Parse a subprogram a.k.a. a function
    static readonly Parser<EOperation[]> SubProgram =
      from operations in Parse.Ref(() => EOperation).Many().Contained(BraceOpen, BraceClose)
      select operations.ToArray();

    static readonly Parser<EFunctionArgument> FunctionArgument =
      from type in Type
      from colon in Colon
      from name in Word
      select new EFunctionArgument { type = type, variable = name };

    static readonly Parser<EFunctionArgument[]> FunctionArguments =
      from arguments in FunctionArgument
        .DelimitedBy(Comma)
        .Optional()
        .Contained(RoundBraceOpen, RoundBraceClose)
      select arguments.IsDefined ? arguments.Get().ToArray() : new EFunctionArgument[] { };

    static readonly Parser<ESolvable[]> CallArguments =
      from open in BraceOpen
      from solvable in
        Solvable
        .DelimitedBy(Comma)
        .Optional()
      from close in BraceClose
      select solvable.IsDefined ? solvable.Get().ToArray() : new ESolvable[] { };


    // Parses a create variable operation
    static readonly Parser<ECreateOperation> CreateOperation =
      from keyword in Parse.String("create")
      from space in Space
      from type in CreateableType
      from colon in Parse.Token(Parse.Char(':'))
      from name in Word
      from semicolon in EndLine
      select new ECreateOperation(name, type);

    // Parses a assign variable operation
    static readonly Parser<EAssignOperation> AssignOperation =
      from variable in Word
      from arrow in ArrowLeft
      from content in Solvable
      from semicolon in EndLine
      select new EAssignOperation(variable, content);

    // Parses a check statement, comparable to an if statment without an else
    static readonly Parser<ECheckOperation> CheckOperation =
      from keyword in Parse.String("check")
      from solvable in Solvable.Contained(BraceOpen, BraceClose)
      from arrow in ArrowRight
      from operations in SubProgram
      from semicolon in EndLine
      select new ECheckOperation(solvable, operations);

    static readonly Parser<EFunctionOperation> FunctionOperation =
      from keyword in Parse.String("function")
      from arguments in FunctionArguments
      from rightarrow in ArrowRight
      from type in CreateableType
      from colon in Colon
      from name in Word
      from leftarrow in ArrowLeft
      from operations in SubProgram
      from semicolon in EndLine
      select new EFunctionOperation(name, type, arguments, operations);

    public static readonly Parser<ECallOperation> CallOperation =
      from arguments in CallArguments
      from arrow in ArrowRight
      from word in Word
      from assign in (
        from secondArrow in ArrowRight
        from variable in Word
        select variable
      ).Optional()
      from semicolon in EndLine
      select assign.IsDefined ?
        new ECallOperation(word, arguments, assign.Get()) :
        new ECallOperation(word, arguments);

    static readonly Parser<EExpressionOperation> ExpressionOperation =
      from solvable in Solvable
      from semicolon in EndLine
      select new EExpressionOperation(solvable);

    // Parses different types of operations
    static readonly Parser<EOperation> EOperation =
        CreateOperation
         .Or<EOperation>(AssignOperation)
         .Or(CallOperation)
         .Or(CheckOperation)
         .Or(FunctionOperation)
         .Or(ExpressionOperation);

    static readonly Parser<EOperation> CommentOperation =
      from comment in Comment.MultiLineComment.Or(Comment.SingleLineComment).Token()
      select new ENoOperation();

    static readonly Parser<EOperation> Operations =
      from comment in CommentOperation.Many().Optional()
      from operation in EOperation
      select operation;

    // Parses the whole program
    public static Parser<EProgram> Program =
      from space in Parse.WhiteSpace.Many()
      from operations in Operations.Many().End()
      select new EProgram(operations.ToArray());

    public static string Test()
    {
      //return ESolvable.End().Parse("4").ToString();
      //return EProgram.Parse("poop();").ToString();
      return CheckOperation.End().Parse("check (itWorks + true) -> {};").ToString();
    }

  }
}
