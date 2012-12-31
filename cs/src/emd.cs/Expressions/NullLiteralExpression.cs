using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
	public class NullLiteralExpression : IExpression {
		private readonly SourceRange _sourceRange;

		public NullLiteralExpression(SourceRange sourceRange) {
			if(null == sourceRange) throw Xception.Because.ArgumentNull(() => sourceRange);

			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
