using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
  public class LineInfo {
    public readonly string LineString;
    public readonly SourceRange SourceRange;

    public LineInfo(string lineString, SourceRange sourceRange) {
      LineString = lineString;
      SourceRange = sourceRange;
    }

    public static LineInfo FromMatch(IMatch match) {
      return new LineInfo(match.String, match.SourceRange);
    }
  }
}
