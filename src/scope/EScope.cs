using System.Linq;
using System.Collections.Generic;

using LLVMSharp;
using E_Lang.variables;
using E_Lang.llvm;


namespace E_Lang.scope
{

  // This a scope that holds variables and function, it can also create nested scopes
  public class EScope
  {
    private readonly Dictionary<string, EVariable> variables = new Dictionary<string, EVariable>();

    private readonly EScope parent;
    private readonly int depth;

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
      try
      {
        variables.Add(name, variable);
      }
      catch 
      {
        throw new ELangException("The variable " + name + " has already been declared");
      }
    }

    public EVariable Get(string name)
    {
      if (!variables.ContainsKey(name))
      {
        if (depth == 0) throw new ELangException("Could not find the variable " + name);
        return parent.Get(name);
      }
      return variables[name];
    }

    public EScope GetChild()
    {
      return new EScope(this);
    }
  }

}