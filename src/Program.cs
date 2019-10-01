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

      try
      {
        //var test = EParser.Test();

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
      EProgram test = EParser.EProgram.Parse(input);
      Interpreter interpreter = new Interpreter();
      interpreter.Run(test);
      //Console.WriteLine(test);
    }

    static void Test()
    {
      var test = EParser.ECallOperation.End().Parse("a -> hi;");

      Console.WriteLine(test);

    }
  }

}
