using System;
using System.Collections.Generic;

namespace E_Lang.src
{
  public class EScope
  {
    public Dictionary<string, EVariable> variables = new Dictionary<string, EVariable>();

    private EScope parent;
    private int depth;

    public EScope()
    {
      depth = 0;
    }

    public EScope(EScope parent)
    {
      this.parent = parent;
      depth = this.parent.depth + 1;
    }

    public void Set(string name, EVariable variable)
    {
      variables[name] = variable;
    }

    public EVariable Get(string name)
    {
      if (!variables.ContainsKey(name))
      {
        if (depth == 0) throw new Exception();
        return parent.Get(name);
      }
      return variables[name];
    }

    public string[] GetVariables() {
      string[] buffer;
      if(depth == 0) buffer = new string[] {};
      else buffer = parent.GetVariables();

      foreach(KeyValuePair<string, EVariable> pair in variables){
        buffer[buffer.Length] = pair.Key;
      }

      return buffer;
    }
  }

  public class EVariable
  {

  }

  public class EInt : EVariable
  {
    public int value;

  }

  public class EBoolean : EVariable
  {
    public bool value;
  }

  public class EString : EVariable
  {
    public string value;
  }
}