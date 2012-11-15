using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public abstract class BaseParsingExpression<TProduct> : IParsingExpression<TProduct> {
		private readonly Guid _id;
		private readonly ParsingExpressionKind _kind;

		// Stats
		private readonly System.Diagnostics.Stopwatch _matchExecution;
		private int _matchExecutionCount = 0;

		public BaseParsingExpression(ParsingExpressionKind kind) {
			_id = Guid.NewGuid();
			_kind = kind;
			_matchExecution = new System.Diagnostics.Stopwatch();
		}

		public Guid Id { get { return _id; } }

		public ParsingExpressionKind Kind { get { return _kind; } }


		public IMatchingResult<TProduct> Matches(MatchingContext context) {
			_matchExecutionCount++;
			_matchExecution.Start();

			var result = MatchesCore(context);

			_matchExecution.Stop();
			return result;
		}

		IMatchingResult IParsingExpression.Matches(MatchingContext context) {
			return Matches(context);
		}

		protected abstract IMatchingResult<TProduct> MatchesCore(MatchingContext context);

		public abstract T HandleWith<T>(IParsingExpressionHandler<T> handler);

		public override string ToString() {
			return "[" + _matchExecutionCount + "][" + _matchExecution.Elapsed.ToString() +  "] " + this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
