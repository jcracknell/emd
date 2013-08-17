using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
  public class TextNode : IPlainInlineNode {
    private readonly string _text;
    private readonly SourceRange _sourceRange;

    public TextNode(string text, SourceRange sourceRange) {
      if(null == text) throw Xception.Because.ArgumentNull(() => text);
      if(string.IsNullOrEmpty(text)) throw Xception.Because.Argument(() => text, "cannot be empty");

      _text = text;
      _sourceRange = sourceRange;
    }

    public string Text { get { return _text; } }

    public SourceRange SourceRange { get { return _sourceRange; } }

    public void HandleWith(INodeHandler handler) {
      handler.Handle(this);
    }

    public T HandleWith<T>(INodeHandler<T> handler) {
      return handler.Handle(this);
    }

    public override string ToString() {
      return "(" + _text + ")";
    }
  }
}
