using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
  public class PostfixDecrementExpression : UnaryExpression {
    public PostfixDecrementExpression(IExpression body, SourceRange sourceRange)
      : base(body, sourceRange) { }

    public override void HandleWith(IExpressionHandler handler) {
      handler.Handle(this);
    }

    public override T HandleWith<T>(IExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
