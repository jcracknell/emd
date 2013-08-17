using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remd {
  public class Options {
    public Options() {
      InputFilename = null;
      OutputFilename = null;
    }

    public string InputFilename { get; set; }
    public string OutputFilename { get; set; }
    public bool StdInput { get { return "-".Equals(InputFilename); } }
    public bool StdOutput { get { return "-".Equals(OutputFilename); } }
  }
}
