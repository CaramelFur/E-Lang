using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using Sprache;

using E_Lang.src;

namespace E_Lang
{
  class Program
  {
    static void Main()
    {
      string input = File.ReadAllText("./testPrograms/shouldwork.elg", Encoding.UTF8);
      try
      {
        //var test = EParser.Test();

        /* EProgram test = EParser.EProgram.Parse(input);
        Interpreter interpreter = new Interpreter();
        interpreter.Run(test);*/
        Console.WriteLine("a");
        var expr = ESolvableParser.ESolvable.End().Parse("3 +3 + a").contents;
        Console.WriteLine("a");
        var lamb = Expression.Lambda<Func<decimal>>(expr, new ParameterExpression[] {
          Expression.Parameter(typeof(decimal), "a")
        });
        var compiled = lamb.Compile();
        Console.WriteLine(lamb);
        Console.WriteLine(expr);
      }
      catch (ParseException e)
      {
        Console.WriteLine(e.ToString());
      }

    }
  }

}
