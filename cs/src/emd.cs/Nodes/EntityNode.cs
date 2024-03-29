﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
  public class EntityNode : IPlainInlineNode {
    private readonly string _value;
    private readonly SourceRange _sourceRange;

    public EntityNode(string value, SourceRange sourceRange) {
      if(null == value) throw Xception.Because.ArgumentNull(() => value);
      if(null == sourceRange) throw Xception.Because.ArgumentNull(() => sourceRange);

      _value = value;
      _sourceRange = sourceRange;
    }

    public string Value { get { return _value; } }

    public SourceRange SourceRange { get { return _sourceRange; } }

    public void HandleWith(INodeHandler handler) {
      handler.Handle(this);
    }

    public T HandleWith<T>(INodeHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}