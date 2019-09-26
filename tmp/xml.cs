using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Sprache;
using System.IO;
using System.Xml;

namespace XmlExample
{
  public class Document
  {
    public Node Root;

    public override string ToString()
    {
      return Root.ToString();
    }
  }

  public class Item { }

  public class Content : Item
  {
    public string Text;

    public override string ToString()
    {
      return Text;
    }
  }

  public class Node : Item
  {
    public string Name;
    public IEnumerable<Item> Children;

    public override string ToString()
    {
      if (Children != null)
        return string.Format("<{0}>", Name) +
            Children.Aggregate("", (s, c) => s + c) +
            string.Format("</{0}>", Name);
      return string.Format("<{0}/>", Name);
    }
  }

  public static class XmlParser
  {
    static CommentParser Comment = new CommentParser("<!--", "-->", "\r\n");

    static readonly Parser<string> Identifier =
        from first in Parse.Letter.Once()
        from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
        select new string(first.Concat(rest).ToArray());

    static Parser<T> Tag<T>(Parser<T> content)
    {
      return from lt in Parse.Char('<')
             from t in content
             from gt in Parse.Char('>').Token()
             select t;
    }

    static readonly Parser<string> BeginTag = Tag(Identifier);

    static Parser<string> EndTag(string name)
    {
      return Tag(from slash in Parse.Char('/')
                 from id in Identifier
                 where id == name
                 select id).Named("closing tag for " + name);
    }

    static readonly Parser<Content> Content =
        from chars in Parse.CharExcept('<').Many()
        select new Content { Text = new string(chars.ToArray()) };

    static readonly Parser<Node> FullNode =
        from tag in BeginTag
        from nodes in Parse.Ref(() => Item).Many()
        from end in EndTag(tag)
        select new Node { Name = tag, Children = nodes };

    static readonly Parser<Node> ShortNode = Tag(from id in Identifier
                                                 from slash in Parse.Char('/')
                                                 select new Node
                                                 {
                                                   Name = id
                                                 });

    static readonly Parser<Node> Node = ShortNode.Or(FullNode);

    static readonly Parser<Item> Item =
        from leading in Comment.MultiLineComment.Many()
        from item in Node.Select(n => (Item)n).XOr(Content)
        from trailing in Comment.MultiLineComment.Many()
        select item;

    public static readonly Parser<Document> Document =
        from leading in Parse.WhiteSpace.Many()
        from doc in Node.Select(n => new Document { Root = n }).End()
        select doc;
  }

  class Program
  {
    static void MMain()
    {
      string input = File.ReadAllText("TestFile.xml", Encoding.UTF8);
      var parsed = XmlParser.Document.Parse(input);
      Console.WriteLine(parsed);
      Console.ReadKey(true);
    }

  }
  static class ExpressionParser
  {
    public static Expression<Func<decimal>> ParseExpression(string text)
    {
      return Lambda.Parse(text);
    }

    static Parser<ExpressionType> Operator(string op, ExpressionType opType)
    {
      return Parse.String(op).Token().Return(opType);
    }

    static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
    static readonly Parser<ExpressionType> Subtract = Operator("-", ExpressionType.SubtractChecked);
    static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);
    static readonly Parser<ExpressionType> Divide = Operator("/", ExpressionType.Divide);

    static readonly Parser<Expression> Constant =
        (from d in Parse.Decimal.Token()
         select (Expression)Expression.Constant(decimal.Parse(d))).Named("number");

    static readonly Parser<Expression> Factor =
        ((from lparen in Parse.Char('(')
          from expr in Parse.Ref(() => Expr)
          from rparen in Parse.Char(')')
          select expr).Named("expression")
         .XOr(Constant)).Token();

    static readonly Parser<Expression> Term = Parse.ChainOperator(Multiply.Or(Divide), Factor, Expression.MakeBinary);

    static readonly Parser<Expression> Expr = Parse.ChainOperator(Add.Or(Subtract), Term, Expression.MakeBinary);

    static readonly Parser<Expression<Func<decimal>>> Lambda =
        Expr.End().Select(body => Expression.Lambda<Func<decimal>>(body));
  }
}

