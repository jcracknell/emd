﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
  public class IdentifierExpression : IExpression {
    private readonly string _name;
    private readonly SourceRange _sourceRange;

    public IdentifierExpression(string name, SourceRange sourceRange) {
      if(null == name) throw Xception.Because.ArgumentNull(() => name);
      if(null == sourceRange) throw Xception.Because.ArgumentNull(() => sourceRange);

      _name = name;
      _sourceRange = sourceRange;
    }

    public string Name { get { return _name; } }

    public SourceRange SourceRange { get { return _sourceRange; } }

    public void HandleWith(IExpressionHandler handler) {
      handler.Handle(this);
    }

    public T HandleWith<T>(IExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
