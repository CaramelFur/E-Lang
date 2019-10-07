using E_Lang.types;

namespace E_Lang.variables
{
  public class EVBoolean : EVariable
  {
    private bool value;

    public override EVariable Assign(EVariable assign)
    {
      EVBoolean converted = (EVBoolean)assign.Convert(GetEType());
      value = converted.Get();
      return this;
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
      return value ? "true" : "false";
    }
  }
}