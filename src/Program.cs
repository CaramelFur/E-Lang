using System;
using System.IO;
using System.Text;
using Sprache;

using E_Lang.src;

namespace E_Lang {
  class Program {
    static void Main (string[] args) {
      string input = File.ReadAllText ("./testPrograms/onlyCreate.elg", Encoding.UTF8);

      var test = EParser.EProgram.Parse(input);

      Console.WriteLine (test);
    }
  }

}
