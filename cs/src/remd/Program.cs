using emd.cs;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remd {
  class Program {
    static void Main(string[] args) {
      var options = new Options();
      for(var i = 0; i < args.Length; i++) {
        if("-?".Equals(args[i]) || "--help".Equals(args[i])) {
          ShowUsage();
          return;
        }

        if(null == options.InputFilename) {
          options.InputFilename = args[i];
          continue;
        }
        if(null == options.OutputFilename) {
          options.OutputFilename = args[i];
          continue;
        }

        Console.Error.Write("Too many arguments.");
        ShowUsage();
        return;
      }

      if(null == options.InputFilename) {
        Console.Error.WriteLine("No input specified.");
        ShowUsage();
        return;
      }

      if(null == options.OutputFilename) {
        Console.Error.WriteLine("No output specified.");
        ShowUsage();
        return;
      }

      string input;
      using(var reader = OpenInput(options))
        input = reader.ReadToEnd();

      var document = Emd.ParseDocument(input);

      var renderer = new emd.cs.Conversion.StructuralRenderer();
      using(var ostream = OpenOutput(options))
        renderer.Render(document, ostream);
    }

    private static TextReader OpenInput(Options options) {
      if(options.StdInput)
        return new StreamReader(Console.OpenStandardInput(), Console.InputEncoding, true);
      else
        return new StreamReader(FormatPath(options.InputFilename), Encoding.UTF8, true);
    }

    private static Stream OpenOutput(Options options) {
      if(options.StdOutput)
        return Console.OpenStandardOutput();
      else 
        return File.Open(FormatPath(options.OutputFilename), FileMode.OpenOrCreate, FileAccess.Write);
    }

    private static string FormatPath(string path) {
      return path
        .Replace('/', Path.DirectorySeparatorChar)
        .Replace('\\', Path.DirectorySeparatorChar);
    }

    private static void ShowUsage() {
      Console.WriteLine();
      Console.WriteLine("Usage: remd.exe input output");
      Console.WriteLine();
      Console.WriteLine("If input is - input is read from standard input.");
      Console.WriteLine("If output is - output is read from standard input.");
    }
  }
}
