using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
  public class UnorderedListItemNode : INodelike {
    private readonly INode[] _children;
    private readonly SourceRange _sourceRange;

    public UnorderedListItemNode(INode[] children, SourceRange sourceRange) {
      if(null == children) throw Xception.Because.ArgumentNull(() => children);

      _children = children;
      _sourceRange = sourceRange;
    }

    public IEnumerable<INode> Children { get { return _children; } }

    public SourceRange SourceRange { get { return _sourceRange; } }
  }
}
