using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
  public abstract class UnaryExpression : Expression {
    private readonly IExpression _body;

    public UnaryExpression(IExpression body, SourceRange sourceRange)
      : base(sourceRange)
    {
      if(null == body) throw Xception.Because.ArgumentNull(() => body);

      _body = body;
    }

    public IExpression Body { get { return _body; } }
  }
}
