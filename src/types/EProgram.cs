using System.Linq;

using E_Lang.operations;

namespace E_Lang.types
{
  public class EProgram
  {
    private readonly EOperation[] operations;

    public EProgram(EOperation[] operations)
    {
      this.operations = operations;
    }

    public override string ToString()
    {
      return operations.Aggregate("operations:", (prev, next) => prev + "\n" + next.ToString());
    }

    public EOperation[] GetOperations(){
      return operations;
    }
  }
}
