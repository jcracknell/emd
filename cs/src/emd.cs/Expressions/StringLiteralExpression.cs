﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
  public class StringLiteralExpression : IExpression {
    private readonly string _value;
    private readonly SourceRange _sourceRange;

    public StringLiteralExpression(string value, SourceRange sourceRange) {
      _value = value;
      _sourceRange = sourceRange;
    }

    public string Value { get { return _value; } }

    public SourceRange SourceRange { get { return _sourceRange; } }

    public void HandleWith(IExpressionHandler handler) {
      handler.Handle(this);
    }

    public T HandleWith<T>(IExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
