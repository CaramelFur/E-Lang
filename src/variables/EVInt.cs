using E_Lang.types;

namespace E_Lang.variables
{
  public class EVInt : EVariable
  {
    private int value;

    public override EVariable Assign(EVariable assign)
    {
      EVInt converted = (EVInt)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    protected override EVariable ConvertInternal(ETypeWord to)
    {
      switch (to.Get())
      {
        case EType.Double:
          return ((EVDouble)New(to)).Set(value);
        case EType.Char:
          return ((EVChar)New(to)).Set((char)value);
        case EType.Boolean:
          return ((EVBoolean)New(to)).Set(value != 0);
      }

      return null;
    }

    public override dynamic Get()
    {
      return value;
    }

    public override EVariable Set(dynamic setto)
    {
      value = setto;
      return this;
    }

    public override string ToString()
    {
      return value.ToString();
    }
  }
}