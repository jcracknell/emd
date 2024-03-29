﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
  public class CodeNode : IFormattedInlineNode {
    private readonly string _text;
    private readonly SourceRange _sourceRange;
    
    public CodeNode(string text, SourceRange sourceRange) {
      if(null == text) throw Xception.Because.ArgumentNull(() => text);
      if(null == sourceRange) throw Xception.Because.ArgumentNull(() => sourceRange);

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
  }
}
