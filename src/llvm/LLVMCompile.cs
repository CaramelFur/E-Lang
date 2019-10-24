using System;
using System.IO;
using System.Diagnostics;

namespace E_Lang.llvm
{
  class LLVMCompile
  {
    private readonly string outPath = null;
    private string bitCode = null;
    private string binCode = null;

    public LLVMCompile(string outpath = null)
    {
      if (outpath == null) outpath = "./out/";
      Directory.CreateDirectory(outpath);
      string fullOutPath = new FileInfo(outpath).Directory.FullName;
      outPath = fullOutPath;
    }

    public string GetOutPath(string FileName = null, string FileExt = null)
    {
      if (FileName == null && FileExt == null)
        return outPath;

      return outPath + "/" + FileName + ((FileExt == null) ? "" : ("." + FileExt));
    }

    public void SetBitCodePath(string path)
    {
      bitCode = path;
    }

    public string CompileToBin(string outputPath)
    {
      Process p = new Process();
      p.StartInfo.FileName = "clang";
      p.StartInfo.Arguments = bitCode + " -o " + outputPath;
      p.StartInfo.UseShellExecute = false;
      p.StartInfo.RedirectStandardOutput = true;
      p.Start();

      string output = p.StandardOutput.ReadToEnd();
      p.WaitForExit();

      p.Dispose();

      binCode = outputPath;

      return output;
    }

    public void Delete()
    {
      if (bitCode != null) File.Delete(bitCode);
    }
  }
}