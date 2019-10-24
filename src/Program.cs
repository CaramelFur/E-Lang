using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;

using Sprache;
using LLVMSharp;

using E_Lang.types;
using E_Lang.lexer;
using E_Lang.compiler;
using E_Lang.llvm;

namespace E_Lang
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 1)
      {
        throw new ELangException("Please do not supply more than 1 argument");
      }


      if (args.Length == 0)
      {
        /*Interpreter interpreter = new Interpreter();
        while (true)
        {
          Console.Write("> ");
          string next = Console.ReadLine();
          if (next == "quit") break;

          try
          {
            EProgram prog = EParser.Program.Parse(next);
            string result = interpreter.Run(prog).ToString();
            Console.WriteLine(result);
          }
          catch (Exception e)
          {
            Console.WriteLine(e.Message);
          }
        }*/
      }
      else if (args.Length == 1)
      {

        EProgram program = readProgramAST(args[0]);

        Console.WriteLine(program);

        LLVMHolder llvm = LLVMHolder.Create("E-Lang");
        Compiler compiler = new Compiler(llvm);

        compiler.Compile(program);

        llvm.Verify();
        llvm.Print();
        llvm.Destroy();
      }
      /* */
    }

    static EProgram readProgramAST(string file)
    {
      string input;
      EProgram program;
      try
      {
        input = File.ReadAllText(file, Encoding.UTF8);
        program = EParser.Program.Parse(input);
        return program;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        Environment.Exit(1);
        throw new Exception();
      }
    }
  }

}
