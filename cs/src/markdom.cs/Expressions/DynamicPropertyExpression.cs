using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class DynamicPropertyExpression : IExpression {
		private readonly IExpression _body;
		private readonly IExpression _memberName;
		private readonly SourceRange _sourceRange;

		public DynamicPropertyExpression(IExpression body, IExpression memberName, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => memberName, memberName);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_body = body;
			_memberName = memberName;
			_sourceRange = sourceRange;
		}

		public IExpression Body { get { return _body; } }

		public IExpression MemberName { get { return _memberName; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
