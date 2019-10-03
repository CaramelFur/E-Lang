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
      if (args.Length > 1)
      {
        throw new Exception("Please do not supply more than 1 argument");
      }

      Interpreter interpreter = new Interpreter();
      if (args.Length == 0)
      {
        while (true) { 
          Console.Write("> "); 
          string next = Console.ReadLine();
          if(next == "quit") break;

          try{
            EProgram prog = EParser.Program.Parse(next);
            interpreter.Run(prog);
          }catch(Exception e){
            Console.WriteLine(e.Message);
          }
        }
      }
      else if (args.Length == 1)
      {
        string input = File.ReadAllText(args[0], Encoding.UTF8);
        EProgram test = EParser.Program.Parse(input);

        interpreter.Run(test);
      }
      /* */
    }
  }

}
