using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sprache;

namespace E_Lang.src
{
  public class EParser
  {
    // Base characters
    static readonly Parser<string> Space = Parse.Text(Parse.WhiteSpace.AtLeastOnce());
    static readonly Parser<char> Colon = Parse.Token(Parse.Char(':'));
    static readonly Parser<char> Comma = Parse.Token(Parse.Char(','));
    static readonly Parser<char> EndLine = Parse.Token(Parse.Char(';'));

    // Comment parser
    static readonly CommentParser Comment = new CommentParser("#", "###", "###", "\n");

    // Simple word parser
    static readonly Parser<string> Word = Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many().Text();
    static readonly Parser<EWord> EWord =
      from word in Word
      select new EWord { word = word };

    // Arrow parser
    static readonly Parser<EToken> ArrowLeft =
      from arrow in Parse.String("<-").Token().Text()
      select new EToken { token = arrow };
    static readonly Parser<EToken> ArrowRight =
      from arrow in Parse.String("->").Token().Text()
      select new EToken { token = arrow };

    // Parser for types that you assign and create
    static readonly Parser<EType> CreateableType =
      from typeName in Parse.String("int")
      .Or(Parse.String("boolean"))
      .Or(Parse.String("string"))
      .Or(Parse.String("void"))
      .Text()
      select new EType { type = typeName };
    // Parser for the other types
    static readonly Parser<EType> UsableType =
      from typeName in Parse.String("Function").Text()
      select new EType { type = typeName };
    // Parser for all types
    static readonly Parser<EType> Type = CreateableType.Or(UsableType);

    // Expression parser
    static readonly Parser<string> PartExpression = Word.Or(
      Parse.String("+").Token()
    ).Or(
      Parse.String("/").Token()
    ).Or(
      Parse.String("*").Token()
    ).Or(
      Parse.String("-").Token()
    ).Text();
    // Parses the () around the expression
    static readonly Parser<string> FullExpression =
      from open in Parse.Char('(').Token()
      from expression in PartExpression.Many()
      from close in Parse.Char(')').Token()
      select new String(expression.Aggregate("", (a, b) => a + b));

    // Parses a solvable 
    static readonly Parser<ESolvable> ESolvable =
      from expr in Parse.Or(Word, FullExpression)
      select new ESolvable { contents = expr };

    // Parse a subprogram a.k.a. a function
    static readonly Parser<EOperation[]> ESubProgram =
      from open in Parse.Char('{').Token()
      from operations in EOperation.Many()
      from close in Parse.Char('}').Token()
      select operations.ToArray();

    static readonly Parser<EFunctionArgument> EFunctionArgument =
      from type in Type
      from colon in Colon
      from name in EWord
      select new EFunctionArgument { type = type, variable = name };

    static readonly Parser<EFunctionArgument> EFunctionArgumentComma =
      from comma in Comma
      from argument in EFunctionArgument
      select argument;

    static readonly Parser<EFunctionArgument[]> EFunctionArguments =
      from open in Parse.Char('(').Token()
      from firstArgument in EFunctionArgument
      from otherArguments in EFunctionArgumentComma.Many()
      from close in Parse.Char(')').Token()
      select new[] { firstArgument }.Concat(otherArguments).ToArray();

    // Parses a create variable operation
    static readonly Parser<ECreateOperation> ECreateOperation =
      from keyword in Parse.String("create")
      from space in Space
      from type in CreateableType
      from colon in Parse.Token(Parse.Char(':'))
      from name in EWord
      from semicolon in EndLine
      select new ECreateOperation { type = type, name = name };

    // Parses a assign variable operation
    static readonly Parser<EAssignOperation> EAssignOperation =
      from variable in EWord
      from arrow in ArrowLeft
      from content in ESolvable
      from semicolon in EndLine
      select new EAssignOperation { variable = variable, content = content };

    // Parses a check statement, comparable to an if statment without an else
    static readonly Parser<ECheckOperation> ECheckOperation =
      from keyword in Parse.String("check")
      from solvable in ESolvable
      from arrow in ArrowRight
      from operations in ESubProgram
      from semicolon in EndLine
      select new ECheckOperation { check = solvable, operations = operations };

    static readonly Parser<EFunctionOperation> EFunctionOperation =
      from keyword in Parse.String("function")
      from arguments in EFunctionArguments
      from rightarrow in ArrowRight
      from type in CreateableType
      from colon in Colon
      from name in EWord
      from leftarrow in ArrowLeft
      from operations in ESubProgram
      from semicolon in EndLine
      select new EFunctionOperation { name = name, type = type, arguments = arguments, operations = operations };

    // Parses different types of operations
    static readonly Parser<EOperation> EOperation =
        ECreateOperation
         .Or<EOperation>(EAssignOperation)
         .Or<EOperation>(ECheckOperation)
         .Or<EOperation>(EFunctionOperation);

    static readonly Parser<EOperation> EComment =
      from comment in Comment.MultiLineComment.Or(Comment.SingleLineComment).Token()
      select new EOperation { };

    static readonly Parser<EOperation> EOperations = EOperation.Or(EComment);

    // Parses the whole program
    public static Parser<EProgram> EProgram =
      from space in Parse.WhiteSpace.Many()
      from operations in EOperations.Many().End()
      select new EProgram { operations = operations.ToArray() };

  }

  class ExpressionParser
  {

  }
}
