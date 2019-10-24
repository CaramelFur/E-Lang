using System;
using System.Linq;
using System.Collections.Generic;

using E_Lang.variables;
using E_Lang.llvm;
using LLVMSharp;

namespace E_Lang.functions
{
  using SysCallGenFunc = Func<LLVMHolder, EVariable[], EVariable>;

  class EFunctions
  {
    private static string[] functions = { "syscall" };

    public static bool HasFunction(string name)
    {
      return functions.Contains(name);
    }

    public static EVariable ExecFunc(LLVMHolder llvm, string name, EVariable[] arguments)
    {
      if (name == "syscall")
      {
        LLVMTypeRef[] syscallArgTypes = arguments.Select(a => a.GetTypeRef()).ToArray();
        SysCallGenFunc GeneratedSysCall = GenSyscall(syscallArgTypes);

        return GeneratedSysCall(llvm, arguments);
      }
      else
      {
        throw new ELangException("There was no function to try");
      }
    }

    public static SysCallGenFunc GenSyscall(LLVMTypeRef[] syscallArgTypes)
    {
      string[] regs = { "rdi", "rsi", "rdx", "r10", "r8", "r9" };

      if (syscallArgTypes.Length > regs.Length) throw new ELangException("Can't generate syscall with moe than 6 parameters");

      LLVMTypeRef sycallFunctionType = LLVM.FunctionType(LLVM.Int64Type(), syscallArgTypes, false);

      string genContraints = "";
      for (int i = 0; i < syscallArgTypes.Length; i++)
      {
        genContraints += "{" + regs[i] + "},";
      }
      string constraints = "=r,{rax}," + genContraints + "~{dirflag},~{fpsr},~{flags}";


      return (LLVMHolder llvm, EVariable[] input) =>
      {
        if (input.Length != syscallArgTypes.Length) throw new ELangException("Invalid syscall generated (amount)");

        for (int i = 0; i < syscallArgTypes.Length; i++)
        {
          if (!syscallArgTypes[i].Equals(input[i].GetTypeRef()))
            throw new ELangException("Invalid syscall generated (types)");
        }

        LLVMValueRef asmSyscall =
          LLVM.ConstInlineAsm(sycallFunctionType, "syscall", constraints, true, false);

        LLVMValueRef[] llvmArguments = input.Select(i => i.Get()).ToArray();

        LLVMValueRef result = LLVM.BuildCall(llvm.GetBuilder(), asmSyscall, llvmArguments, llvm.GetNewName());

        LLVMValueRef result32 = LLVM.BuildIntCast(llvm.GetBuilder(), result, LLVM.Int32Type(), llvm.GetNewName());

        EVInt varResult = new EVInt(llvm);
        return varResult.Set(result32);
      };
    }
  }
}