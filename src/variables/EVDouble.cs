using E_Lang.types;

namespace E_Lang.variables
{
  public class EVDouble : EVariable
  {
    private decimal value;

    public override EVariable Assign(EVariable assign)
    {
      EVDouble converted = (EVDouble)assign.Convert(GetEType());
      value = converted.Get();
      return this;
    }

    public override EVariable Convert(EType to)
    {
      switch (to.ToString())
      {
        case "int":
          return ((EVInt)New(to)).Set((int)value);
        case "boolean":
          return ((EVBoolean)New(to)).Set(value != 0);
      }

      if (GetEType().ToString() == to.ToString()) return this;
      return CannotConvert(to);
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