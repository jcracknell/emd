using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes {
	public class InlineExpressionNode : IRichInlineNode {
		private readonly IExpression _expression;
		private readonly MarkdomSourceRange _sourceRange;

		public InlineExpressionNode(IExpression expression, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => expression, expression);

			_expression = expression;
			_sourceRange = sourceRange;
		}

		public IExpression Expression { get { return _expression; } }

		public NodeKind Kind { get { return NodeKind.InlineExpression; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
