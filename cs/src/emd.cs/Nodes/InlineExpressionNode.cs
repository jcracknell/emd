using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
	public class InlineExpressionNode : IRichInlineNode {
		private readonly IExpression _expression;
		private readonly SourceRange _sourceRange;

		public InlineExpressionNode(IExpression expression, SourceRange sourceRange) {
			if(null == expression) throw Xception.Because.ArgumentNull(() => expression);

			_expression = expression;
			_sourceRange = sourceRange;
		}

		public IExpression Expression { get { return _expression; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
