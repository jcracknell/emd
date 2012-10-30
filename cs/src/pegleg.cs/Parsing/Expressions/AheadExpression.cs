using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class AheadExpression : BaseParsingExpression {
		public AheadExpression() : base(ParsingExpressionKind.Lookahead) { }

		public abstract IParsingExpression Body { get; }
	}

	public class AheadExpression<TProduct> : AheadExpression, IParsingExpression<TProduct> {
		private readonly IParsingExpression _body;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;

		public AheadExpression(IParsingExpression body, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
		
			_body = body;	
			_matchAction = matchAction;
		}

		public override IParsingExpression Body { get { return _body; } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				var product = _matchAction(context.StartMatch().CompleteMatch(this, bodyMatchingResult.Product));

				return new SuccessfulMatchingResult(product);
			} else {
				return new UnsuccessfulMatchingResult();
			}
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
