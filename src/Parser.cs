using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sprache;

namespace E_Lang.src {
  public class EParser {
    static CommentParser Comment = new CommentParser("# ", "###", "###", "\n");

    static readonly Parser<string> Word = Parse.Text(
      Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
    );
    static readonly Parser<string> Space = Parse.Text(Parse.WhiteSpace.AtLeastOnce());
    static readonly Parser<char> EndLine = Parse.Token(Parse.Char(';'));
    static readonly Parser<string> ArrowLeft = Parse.String("<-").Token().Text();
    static readonly Parser<string> ArrowRight = Parse.String("->").Token().Text();

    static readonly Parser<string> CreateableType = Parse.Text(
      Parse.String("int")
      .Or(Parse.String("boolean"))
      .Or(Parse.String("string"))
      .Or(Parse.String("void"))
    );
    static readonly Parser<string> UsableType = Parse.Text(Parse.String("Function"));
    static readonly Parser<string> Type = CreateableType.Or(UsableType);

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
}
