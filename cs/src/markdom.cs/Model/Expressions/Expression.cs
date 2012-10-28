using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public abstract class Expression {
		private readonly MarkdomSourceRange _sourceRange;

		public Expression(MarkdomSourceRange sourceRange) {
			_sourceRange = sourceRange;
		}

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public abstract ExpressionType Type { get; }
	}
}
