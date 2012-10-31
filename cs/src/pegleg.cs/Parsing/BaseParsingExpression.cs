using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public abstract class BaseParsingExpression : IParsingExpression {
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

		public abstract T HandleWith<T>(IParsingExpressionHandler<T> handler);

		public IMatchingResult Matches(IMatchingContext context) {
			_matchExecutionCount++;
			_matchExecution.Start();

			var result = MatchesCore(context);

			_matchExecution.Stop();
			return result;
		}

		protected abstract IMatchingResult MatchesCore(IMatchingContext context);

		public override string ToString() {
			return "[" + _matchExecutionCount + "][" + _matchExecution.Elapsed.ToString() +  "] " + this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
