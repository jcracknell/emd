using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes {
	public class ExpressionBlockNode : IBlockNode {
		private readonly IExpression _expression;
		private readonly SourceRange _sourceRange;

		public ExpressionBlockNode(IExpression expression, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => expression, expression);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_expression = expression;
			_sourceRange = sourceRange;
		}

		public NodeKind Kind { get { return NodeKind.ExpressionBlock; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
