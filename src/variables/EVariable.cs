using System;
using System.Linq;
using System.Collections.Generic;

using E_Lang.types;
using E_Lang.llvm;

using LLVMSharp;


namespace E_Lang.variables
{

  // All the static parts of EVariable
  public abstract class __EVariableStatic
  {

    private static readonly Dictionary<string, Type> types = new Dictionary<string, Type> {
      { "int", typeof(EVInt) },
      { "double", typeof(EVDouble) },
      { "boolean", typeof(EVBoolean) },
      { "char", typeof(EVChar)},
      { "void", typeof(EVVoid) },
      //{ "array", typeof(EVPointer) }
    };

    public static string[] GetTypes()
    {
      return types.Keys.ToArray();
    }

    public static EVariable New(EType type, LLVMHolder llvm)
    {
      return New(type.Get(), llvm);
    }

    public static EVariable New(string type, LLVMHolder llvm)
    {
      if (!types.ContainsKey(type)) throw new ELangException("Variable type " + type + " is unknown");
      Type createType = types[type];
      return (EVariable)Activator.CreateInstance(createType, llvm);
    }

    public static EType GetEType(Type t)
    {
      string type = types.Where((pair) => pair.Value == t).First().Key;
      return new EType(type);
    }
  }

  public abstract class __EVariableEmpty : __EVariableStatic
  {
    protected virtual LLVMValueRef? ConvertThisInternallyTo(string to)
    {
      return null;
    }

    protected virtual LLVMValueRef? ParseInternallyFromToThis(LLVMValueRef from, EType type)
    {
      return null;
    }

    public EType GetEType()
    {
      return GetEType(GetType());
    }

    protected dynamic CannotConvert(string type)
    {
      throw new ELangException("Cannot convert " + GetEType() + " to " + type);
    }

    protected dynamic CannotConvertFrom(EType type)
    {
      throw new ELangException("Cannot convert from " + type + " to " + GetEType());
    }

    protected dynamic IsUndefined()
    {
      throw new ELangException("Variable is undefined");
    }
  }

  public abstract class EVariable : __EVariableEmpty
  {
    protected LLVMHolder llvm = null;
    protected LLVMTypeRef type;
    protected EType etype = null;
    protected LLVMValueRef valuePtr;

    // Initialize a new variable by allocating some space for the variable
    protected EVariable(LLVMHolder holder, LLVMTypeRef typeRef, string typeId, bool skipAlloc = false)
    {
      if (typeId == null || typeId == "") throw new ELangException("The typeid cannot be empty");

      llvm = holder;
      type = typeRef;
      etype = new EType(typeId);

      if (!skipAlloc)
      {
        valuePtr = LLVM.BuildAlloca(llvm.GetBuilder(), type, llvm.GetNewName());
      }
    }

    // Get the LLVM type of this variable
    public LLVMTypeRef GetTypeRef()
    {
      return type;
    }

    // Convert this variable to another type
    private LLVMValueRef Convert(EType toType)
    {
      string to = toType.Get();
      LLVMValueRef? tryConvert = ConvertThisInternallyTo(to);
      if (tryConvert.HasValue) return tryConvert.Value;
      if (GetEType().Get() == to) return LLVM.BuildLoad(llvm.GetBuilder(), valuePtr, llvm.GetNewName());
      return CannotConvert(to);
    }

    private LLVMValueRef Parse(LLVMValueRef valueRef, EType type)
    {
      LLVMValueRef? tryInternalParse = ParseInternallyFromToThis(valueRef, type);
      if (tryInternalParse.HasValue)
        return tryInternalParse.Value;
      if (GetEType().Get() == type.Get()) return valueRef;
      return CannotConvertFrom(type);
    }

    // Set this variable to the parsed variable
    public EVariable Assign(EVariable assign)
    {
      LLVMValueRef converted = Parse(assign.Get(), assign.GetEType());
      return Set(converted);
    }

    protected virtual EVariable Set(LLVMValueRef value)
    {
      if (value.TypeOf().ToString() != GetTypeRef().ToString())
      {
        throw new ELangException("You tried to set a variable of type " + GetTypeRef() +
        " with a value of type " + value.TypeOf());
      }

      LLVM.BuildStore(llvm.GetBuilder(), value, valuePtr);
      return this;
    }

    public EVariable Clone()
    {
      return New(GetEType(), llvm).Assign(this);
    }

    protected virtual LLVMValueRef Get()
    {
      return LLVM.BuildLoad(llvm.GetBuilder(), valuePtr, llvm.GetNewName());
    }

    public static LLVMValueRef GetRawValueFromVariable(EVariable variable)
    {
      return variable.Get();
    }

    public static EVariable PutRawValueIntVariable(EVariable variable, LLVMValueRef value)
    {
      variable.Set(value);
      return variable;
    }
  }
}