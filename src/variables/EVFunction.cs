using E_Lang.types;
using E_Lang.llvm;
using LLVMSharp;
using System;


namespace E_Lang.variables
{
  public class EVFunctionDefinition
  {
    private readonly LLVMValueRef function;
    private readonly EType[] inputTypes;
    private readonly EType returnType;

    public EVFunctionDefinition(LLVMValueRef function, EType[] inputTypes, EType returnType)
    {
      this.function = function;
      this.inputTypes = inputTypes;
      this.returnType = returnType;
    }

    public LLVMValueRef GetFunction()
    {
      return function;
    }

    public EType[] GetInputTypes()
    {
      return inputTypes;
    }

    public EType GetReturnType()
    {
      return returnType;
    }
  }

  public class EVFunction : EVariable
  {
    private EVFunctionDefinition value = null;

    public EVFunction(LLVMHolder holder) : base(holder) { }

    public override LLVMTypeRef GetTypeRef()
    {
      return LLVM.TypeOf(Get());
    }

    public override EVariable Assign(EVariable assign)
    {
      assign.Convert(GetEType());
      return this;
    }

    protected override EVariable ConvertInternal(string to)
    {
      EVariable newvar = New(to, llvm);
      LLVMValueRef convert;
      switch (to)
      {
        case "double":
          convert = LLVM.BuildUIToFP(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
        case "char":
        case "boolean":
          convert = LLVM.BuildIntCast(llvm.GetBuilder(), Get(), newvar.GetTypeRef(), llvm.GetNewName());
          return newvar.Set(convert);
      }

      return null;
    }

    public override LLVMValueRef Get()
    {
      if (value == null) IsUndefined();
      return value.GetFunction();
    }

    public override EVariable Set(dynamic setTo)
    {
      if (setTo is EVFunctionDefinition)
      {
        LLVMValueRef parsedValue = setTo.GetFunction();
        if (LLVM.GetValueKind(parsedValue)
          .Equals(LLVMValueKind.LLVMFunctionValueKind))
        {
          value = setTo;
          return this;
        }
      }
      throw new ELangException("EVFunction did not receive a function");
    }

    public EVariable Call(EVariable[] args){
      LLVMValueRef[] convArgs = new LLVMValueRef[args.Length];
      for(int i = 0; i < args.Length; i++){
        EVariable conv = args[i].Convert(value.GetInputTypes()[i]);
        convArgs[i] = conv.Get();
      }

      LLVMValueRef ret = LLVM.BuildCall(llvm.GetBuilder(), value.GetFunction(), convArgs, llvm.GetNewName());
      return New(value.GetReturnType(), llvm).Set(ret);
    }

  }
}