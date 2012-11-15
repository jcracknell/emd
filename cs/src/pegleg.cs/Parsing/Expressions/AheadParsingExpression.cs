using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class AheadParsingExpression<TBody, TProduct> : BaseParsingExpression<TProduct> {
		protected readonly IParsingExpression<TBody> _body;

		public AheadParsingExpression(IParsingExpression<TBody> body)
			: base(ParsingExpressionKind.Lookahead)
		{
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
		}

		public IParsingExpression<TBody> Body { get { return _body; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingAheadParsingExpression<TBody> : AheadParsingExpression<TBody, TBody> {
		public NonCapturingAheadParsingExpression(IParsingExpression<TBody> body)
			: base(body)
		{ }

		protected override IMatchingResult<TBody> MatchesCore(MatchingContext context) {
			return _body.Matches(context.Clone());
		}
	}

	public class CapturingAheadParsingExpression<TBody, TProduct> : AheadParsingExpression<TBody, TProduct> {
		private readonly Func<IMatch<TBody>, TProduct> _matchAction;

		public CapturingAheadParsingExpression(IParsingExpression<TBody> body, Func<IMatch<TBody>, TProduct> matchAction)
			: base(body)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
			
			_matchAction = matchAction;
		}

		protected override IMatchingResult<TProduct> MatchesCore(MatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				var product = _matchAction(context.GetMatchBuilderFor(this).CompleteMatch(bodyMatchingResult.Product));
				return SuccessfulMatchingResult.Create(product);
			}

			return UnsuccessfulMatchingResult.Create(this);
		}
	}
}
