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

    public override EVariable Convert(EType to)
    {
      switch (to.ToString())
      {
        case "double":
          return ((EVDouble)New(to)).Set(value);
        case "boolean":
          return ((EVBoolean)New(to)).Set(value != 0);
      }

      if (GetEType().ToString() == to.ToString()) return this;
      return CannotConvert(to);
    }

    public override dynamic Get()
    {
      return (decimal) value;
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