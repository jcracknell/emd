using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
  public interface INodelike {
    SourceRange SourceRange { get; }
  }
}
