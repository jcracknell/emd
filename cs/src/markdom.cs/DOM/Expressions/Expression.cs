using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM.Expressions {
	public abstract class Expression {
		private readonly SourceRange _sourceRange;

		public Expression(SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public abstract ExpressionType Type { get; }
	}
}
