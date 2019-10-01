using System;
using System.Linq.Expressions;

namespace E_Lang.src
{
  public class ESolvable
  {
    public Expression contents;

    public override string ToString()
    {
        return contents.ToString();
    }

    public string SolveString()
    {
        return contents.ToString();
    }

    public int SolveInt()
    {
        try
        {
        int result = Int32.Parse(contents.ToString());
        return result;
        }
        catch (FormatException)
        {
        throw new Exception("Could not parse int: " + contents);
        }
    }

    public bool SolveBool()
    {
        if (SolveInt() != 0) return true;
        else return false;
    }
  }

  public class ESPart {

  }

  public class ESolvableNumber : ESPart {
    public double number;
  }

  public class ESOperation : ESPart {
    public ExpressionType type;
  }
}