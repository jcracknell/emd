using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public abstract class BaseParsingExpression : IParsingExpression {
		private readonly Guid _id;
		private readonly ParsingExpressionKind _kind;

		public BaseParsingExpression(ParsingExpressionKind kind) {
			_id = Guid.NewGuid();
			_kind = kind;
		}

		public Guid Id { get { return _id; } }

		public ParsingExpressionKind Kind { get { return _kind; } }

		public abstract T HandleWith<T>(IParsingExpressionHandler<T> handler);

		public IMatchingResult Matches(IMatchingContext context) {
			return MatchesCore(context);
		}

		protected abstract IMatchingResult MatchesCore(IMatchingContext context);

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
