using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;

using Sprache;
using LLVMSharp;

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
        throw new ELangException("Please do not supply more than 1 argument");
      }


      if (args.Length == 0)
      {
        Interpreter interpreter = new Interpreter();
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
        }
      }
      else if (args.Length == 1)
      {
        string input;
        EProgram program;
        try
        {
          input = File.ReadAllText(args[0], Encoding.UTF8);
          program = EParser.Program.Parse(input);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
          return;
        }

        Console.WriteLine(program);

        LLVMModuleRef module = LLVM.ModuleCreateWithName("E-Lang");
        LLVMBuilderRef builder = LLVM.CreateBuilder();

        LLVM.LinkInMCJIT();
        LLVM.InitializeX86TargetInfo();
        LLVM.InitializeX86Target();
        LLVM.InitializeX86TargetMC();

        if (LLVM.CreateExecutionEngineForModule(out LLVMExecutionEngineRef engine, module, out String errorMessage).Value == 1)
        {
          Console.WriteLine(errorMessage);
          return;
        }

        LLVMPassManagerRef passManager = GenPassManager(module);

        // Do things

        LLVM.DumpModule(module);

        LLVM.DisposeModule(module);
        LLVM.DisposePassManager(passManager);

      }
      /* */
    }

    static LLVMPassManagerRef GenPassManager(LLVMModuleRef module)
    {
      LLVMPassManagerRef passManager = LLVM.CreateFunctionPassManagerForModule(module);
      // Provide basic AliasAnalysis support for GVN.
      LLVM.AddBasicAliasAnalysisPass(passManager);

      // Promote allocas to registers.
      LLVM.AddPromoteMemoryToRegisterPass(passManager);

      // Do simple "peephole" optimizations and bit-twiddling optzns.
      LLVM.AddInstructionCombiningPass(passManager);

      // Reassociate expressions.
      LLVM.AddReassociatePass(passManager);

      // Eliminate Common SubExpressions.
      LLVM.AddGVNPass(passManager);

      // Simplify the control flow graph (deleting unreachable blocks, etc).
      LLVM.AddCFGSimplificationPass(passManager);

      LLVM.InitializeFunctionPassManager(passManager);

      return passManager;
    }
  }

}
