using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
  public class StaticPropertyExpression : IExpression {
    private readonly IExpression _body;
    private readonly string _memberName;
    private readonly SourceRange _sourceRange;

    public StaticPropertyExpression(IExpression body, string memberName, SourceRange sourceRange) {
      if(null == body) throw Xception.Because.ArgumentNull(() => body);
      if(null == memberName) throw Xception.Because.ArgumentNull(() => memberName);
      if(null == sourceRange) throw Xception.Because.ArgumentNull(() => sourceRange);
      if(!(0 != memberName.Length)) throw Xception.Because.Argument(() => memberName, "cannot be empty");

      _body = body;
      _memberName = memberName;
      _sourceRange = sourceRange;
    }

    public IExpression Body { get { return _body; } }
    
    public string MemberName { get { return _memberName; } }

    public SourceRange SourceRange { get { return _sourceRange; } }

    public void HandleWith(IExpressionHandler handler) {
      handler.Handle(this);
    }

    public T HandleWith<T>(IExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
