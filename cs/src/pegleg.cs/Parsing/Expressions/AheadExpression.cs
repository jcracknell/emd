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
		private readonly Func<IMatch<object>, TProduct> _matchAction;

		public AheadExpression(IParsingExpression body, Func<IMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
		
			_body = body;	
			_matchAction = matchAction;
		}

		public override IParsingExpression Body { get { return _body; } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			
			var bodyMatchingResult = _body.Matches(bodyMatchingContext);

			if(null == _matchAction || !bodyMatchingResult.Succeeded)
				return bodyMatchingResult;

			var product = _matchAction(context.StartMatch().CompleteMatch(this, bodyMatchingResult.Product));
			return new SuccessfulMatchingResult(product);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
