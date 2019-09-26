using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sprache;

namespace E_Lang.src {
  public class EParser {
    // Base characters
    static readonly Parser<string> Space = Parse.Text(Parse.WhiteSpace.AtLeastOnce());
    static readonly Parser<char> EndLine = Parse.Token(Parse.Char(';'));

    // Comment parser
    static CommentParser Comment = new CommentParser("# ", "###", "###", "\n");

    // Simple word parser
    static readonly Parser<EWord> Word =
      from word in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many().Text()
    select new EWord { word = word };

    // Arrow parser
    static readonly Parser<EToken> ArrowLeft =
      from arrow in Parse.String("<-").Token().Text()
    select new EToken { token = arrow };
    static readonly Parser<EToken> ArrowRight =
      from arrow in Parse.String("->").Token().Text()
    select new EToken { token = arrow };

    // Parser for types that you assign and create
    static readonly Parser<Etype> CreateableType =
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
    static readonly Parser<EType> Type = Parse.Or(CreateableType, UsableType);

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

    static readonly Parser<string> FullExpression =
      from open in Parse.Char('(').Token()
    from expression in PartExpression.Many()
    from close in Parse.Char(')').Token()
    select new String(expression.Aggregate("", (a, b) => a + b));

    static readonly Parser<ESolvable> ESolvable =
      from expr in Parse.Or(Word, FullExpression)
    select new ESolvable { contents = expr };

    static readonly Parser<ECreateOperation> ECreateOperation =
      from keyword in Parse.String("create")
    from space in Space
    from type in CreateableType
    from colon in Parse.Token(Parse.Char(':'))
    from name in Word
    from semicolon in EndLine
    select new ECreateOperation { type = type, name = name };

    static readonly Parser<EAssignOperation> EAssignOperation =
      from variable in Word
    from arrow in ArrowLeft
    from content in ESolvable
    from semicolon in EndLine
    select new EAssignOperation { variable = variable, content = content };

    static readonly Parser<EOperation> EOperation = Parse.Or<EOperation>(ECreateOperation, EAssignOperation);

    public static Parser<EProgram> EProgram =
      from space in Parse.WhiteSpace.Many()
    from operations in EOperation.Many().End()
    select new EProgram { operations = operations.ToArray() };

  }

  class ExpressionParser {
    
  }
}
