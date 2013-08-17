using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
  public class TableRowNode : INodelike {
    private readonly TableCellNode[] _cells;
    private readonly SourceRange _sourceRange;

    public TableRowNode(TableCellNode[] cells, SourceRange sourceRange) {
      if(null == cells) throw Xception.Because.ArgumentNull(() => cells);

      _cells = cells;
      _sourceRange = sourceRange;
    }

    public IEnumerable<TableCellNode> Cells { get { return _cells; } }

    public SourceRange SourceRange { get { return _sourceRange; } }
  }
}
