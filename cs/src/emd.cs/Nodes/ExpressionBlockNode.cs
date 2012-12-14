using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
	public class ExpressionBlockNode : IBlockNode {
		private readonly IExpression _expression;
		private readonly SourceRange _sourceRange;

		public ExpressionBlockNode(IExpression expression, SourceRange sourceRange) {
			if(null == expression) throw ExceptionBecause.ArgumentNull(() => expression);
			if(null == sourceRange) throw ExceptionBecause.ArgumentNull(() => sourceRange);

			_expression = expression;
			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public IExpression Expression { get { return _expression; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
