using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class OptionalParsingExpression<TBody, TProduct> : BaseParsingExpression<TProduct> {
		protected readonly IParsingExpression<TBody> _body;

		public OptionalParsingExpression(IParsingExpression<TBody> body)
			: base(ParsingExpressionKind.Optional)
		{
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
		}

		public IParsingExpression<TBody> Body { get { return _body; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingOptionalParsingExpression<TBody> : OptionalParsingExpression<TBody, TBody> {
		public NonCapturingOptionalParsingExpression(IParsingExpression<TBody> body) : base(body) { }

		protected override IMatchingResult<TBody> MatchesCore(MatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchResult.Succeeded) {
				context.Assimilate(bodyMatchingContext);
				return bodyMatchResult;
			}

			return SuccessfulMatchingResult.Create(default(TBody));
		}
	}

	public class CapturingOptionalParsingExpression<TBody, TProduct> : OptionalParsingExpression<TBody, TProduct> {
		private readonly Func<IMatch<TBody>, TProduct> _matchAction;
		private readonly Func<IMatch<Nil>, TProduct> _noMatchAction;

		public CapturingOptionalParsingExpression(IParsingExpression<TBody> body, Func<IMatch<TBody>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction)
			: base(body)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
			CodeContract.ArgumentIsNotNull(() => noMatchAction, noMatchAction);

			_matchAction = matchAction;
			_noMatchAction = noMatchAction;
		}

		protected override IMatchingResult<TProduct> MatchesCore(MatchingContext context) {
			var matchBuilder = context.StartMatch();

			var bodyMatchingContext = context.Clone();
			var bodyMatchResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchResult.Succeeded) {
				context.Assimilate(bodyMatchingContext);
				
				var product = _matchAction(matchBuilder.CompleteMatch(this, bodyMatchResult.Product));	
				return SuccessfulMatchingResult.Create(product);
			} else {
				return SuccessfulMatchingResult.Create(_noMatchAction(matchBuilder.CompleteMatch(this, Nil.Value)));
			}
		}
	}
}
