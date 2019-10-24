using System;
using System.Linq;
using System.Collections.Generic;

using E_Lang.types;
using E_Lang.llvm;

using LLVMSharp;


namespace E_Lang.variables
{
  public abstract class EVariable
  {
    protected LLVMHolder llvm;

    public EVariable(LLVMHolder holder)
    {
      llvm = holder;
    }

    public virtual LLVMTypeRef GetTypeRef()
    {
      throw new ELangException("Cannot get type from abstract class");
    }

    private static readonly Dictionary<EType, Type> types = new Dictionary<EType, Type> {
      { EType.Int, typeof(EVInt) },
      { EType.Double, typeof(EVDouble) },
      { EType.Boolean, typeof(EVBoolean) },
      { EType.Char, typeof(EVChar)},
      { EType.Void, typeof(EVVoid) },
      { EType.Pointer, typeof(EVPointer) }
    };

    public static EVariable New(ETypeWord type, LLVMHolder llvm)
    {
      return New(type.Get(), llvm);
    }

    public static EVariable New(EType type, LLVMHolder llvm)
    {
      if (!types.ContainsKey(type)) throw new ELangException("Variable type " + type + " is unknown");
      Type createType = types[type];
      return (EVariable)Activator.CreateInstance(createType, llvm);
    }

    public static ETypeWord GetEType(Type t)
    {
      EType type = types.Where((pair) => pair.Value == t).First().Key;
      return new ETypeWord(type);
    }

    public ETypeWord GetEType()
    {
      EType type = types.Where((pair) => pair.Value == GetType()).First().Key;
      return new ETypeWord(type);
    }

    public EVariable Convert(EType to)
    {
      return Convert(new ETypeWord(to));
    }

    public EVariable Convert(ETypeWord to)
    { 
      EVariable tryConvert = ConvertInternal(to);
      if (tryConvert != null) return tryConvert;
      if (GetEType().Get() == to.Get()) return this;
      return CannotConvert(to);
    }

    public EVariable Clone()
    {
      return New(GetEType(), llvm).Assign(this);
    }

    public virtual EVariable Assign(EVariable assign)
    {
      throw new ELangException("Cannot assign to abstract class");
    }

    protected virtual EVariable ConvertInternal(ETypeWord to)
    {
      return null;
    }

    public virtual LLVMValueRef Get()
    {
      throw new ELangException("Cannot get from abstract class");
    }

    public virtual EVariable Set(dynamic setto)
    {
      throw new ELangException("Cannot set in abstract class");
    }

    protected EVariable CannotConvert(ETypeWord type)
    {
      throw new ELangException("Cannot convert " + GetEType() + " to " + type);
    }

    protected EVariable IsUndefined()
    {
      throw new ELangException("Variable is undefined");
    }
  }
}