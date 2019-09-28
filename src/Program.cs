using System;
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

        var test = EParser.EProgram.Parse(input);
        Console.WriteLine(test);
      }
      catch (ParseException e)
      {
        Console.WriteLine(e.ToString());
      }

    }
  }

}
