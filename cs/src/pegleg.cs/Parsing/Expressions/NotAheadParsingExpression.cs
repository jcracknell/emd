using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class NotAheadParsingExpression : BaseParsingExpression {
		public NotAheadParsingExpression() : base(ParsingExpressionKind.NegativeLookahead) { }
		public abstract IParsingExpression Body { get; }
	}

	public class NotAheadParsingExpression<TProduct> : NotAheadParsingExpression, IParsingExpression<TProduct> {
		private readonly IParsingExpression _body;
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public NotAheadParsingExpression(IParsingExpression body, Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
			_matchAction = matchAction;
		}

		public override IParsingExpression Body { get { return _body; } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				return new UnsuccessfulMatchingResult();
			} else {
				if(null == _matchAction)
					return new SuccessfulMatchingResult(Nil.Value);

				var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));
				return new SuccessfulMatchingResult(product);
			}
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
