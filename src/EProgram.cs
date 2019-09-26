using System.Linq;
namespace E_Lang.src {
  public class EProgram {
    public EOperation[] operations;

    public override string ToString() {
      return operations.Aggregate("Operations:", (prev, next) => prev + " " + next.ToString());
    }

  }

  public class EFunction {

  }

  public class ESolvable {
    public string contents;

    public override string ToString() {
      return contents;
    }
  }

  public class EOperation { }

  public class ECreateOperation : EOperation {
    public string name;
    public string type;

    public override string ToString() {
      return "ECreateOperation{" + type + ": '" + name + "'}";
    }

  }

  public class EAssignOperation : EOperation {
    public string variable;

    public ESolvable content;

    public override string ToString() {
      return "EAssignOperation{" + variable + " = " + content.ToString() + "}";
    }
  }
}
