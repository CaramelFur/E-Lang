using System;
using System.Linq;
using System.Collections.Generic;

using E_Lang.types;


namespace E_Lang.variables
{
  public abstract class EVariable
  {
    private static readonly Dictionary<EType, Type> types = new Dictionary<EType, Type> {
      { EType.Int, typeof(EVInt) },
      { EType.Double, typeof(EVDouble) },
      { EType.Boolean, typeof(EVBoolean) },
      { EType.Char, typeof(EVChar)},
      { EType.Void, typeof(EVVoid) }
    };

    public static EVariable New(ETypeWord type)
    {
      return New(type.Get());
    }

    public static EVariable New(EType type)
    {
      if (!types.ContainsKey(type)) throw new Exception("Variable type " + type + " is unknown");
      Type createType = types[type];
      return (EVariable)Activator.CreateInstance(createType);
    }

    public ETypeWord GetEType()
    {
      EType type = types.Where((pair) => pair.Value == GetType()).First().Key;
      return new ETypeWord(type);
    }

    public static ETypeWord GetEType(Type t)
    {
      EType type = types.Where((pair) => pair.Value == t).First().Key;
      return new ETypeWord(type);
    }

    public virtual EVariable Assign(EVariable assign)
    {
      throw new Exception("Cannot assign to abstract class");
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

    protected virtual EVariable ConvertInternal(ETypeWord to)
    {
      return null;
    }

    public virtual dynamic Get()
    {
      return "";
    }

    public virtual EVariable Set(dynamic setto)
    {
      return this;
    }

    public EVariable Clone()
    {
      return New(GetEType()).Assign(this);
    }

    protected EVariable CannotConvert(ETypeWord type)
    {
      throw new Exception("Cannot convert " + type + " to " + GetEType());
    }
  }
}