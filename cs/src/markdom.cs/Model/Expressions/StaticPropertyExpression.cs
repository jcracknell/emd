using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class StaticPropertyExpression : IExpression {
		private readonly IExpression _body;
		private readonly string _memberName;
		private readonly MarkdomSourceRange _sourceRange;

		public StaticPropertyExpression(IExpression body, string memberName, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => memberName, memberName);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);
			CodeContract.ArgumentIsValid(() => memberName, 0 != memberName.Length, "cannot be empty");

			_body = body;
			_memberName = memberName;
			_sourceRange = sourceRange;
		}

		public IExpression Body { get { return _body; } }
		
		public string MemberName { get { return _memberName; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.StaticProperty; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
