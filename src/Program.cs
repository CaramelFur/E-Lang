using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using Sprache;

using E_Lang.types;
using E_Lang.lexer;
using E_Lang.interpreter;

namespace E_Lang
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        Prog();
        //Test();
      }
      catch (ParseException e)
      {
        Console.WriteLine(e.ToString());
      }
    }

    static void Prog()
    {
      string input = File.ReadAllText("./testPrograms/shouldwork.elg", Encoding.UTF8);
      EProgram test = EParser.Program.Parse(input);
      //Console.WriteLine(test);
      Interpreter interpreter = new Interpreter();
      interpreter.Run(test);
      //
    }

    static void Test()
    {
      var test = EParser.CallOperation.End().Parse("a -> hi;");

      Console.WriteLine(test);

    }
  }

}
