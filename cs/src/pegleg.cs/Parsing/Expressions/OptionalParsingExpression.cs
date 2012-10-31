using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class OptionalParsingExpression : BaseParsingExpression {
		public OptionalParsingExpression() : base(ParsingExpressionKind.Optional) { }
		public abstract IParsingExpression Body { get; }
	}

	public class OptionalParsingExpression<TProduct> : OptionalParsingExpression, IParsingExpression<TProduct> {
		private readonly IParsingExpression _body;
		private readonly Func<IMatch<object>, TProduct> _matchAction;
		private readonly Func<IMatch<Nil>, TProduct> _noMatchAction;

		public OptionalParsingExpression(IParsingExpression body, Func<IMatch<object>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
			_matchAction = matchAction;
			_noMatchAction = noMatchAction;
		}

		public override IParsingExpression Body { get { return _body; } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var bodyApplicationContext = context.Clone();
			var bodyMatchResult = _body.Matches(bodyApplicationContext);

			if(!bodyMatchResult.Succeeded)
				return new SuccessfulMatchingResult(
					null != _noMatchAction
						? _noMatchAction(matchBuilder.CompleteMatch(this, Nil.Value))
						: default(TProduct));

			context.Assimilate(bodyApplicationContext);

			return new SuccessfulMatchingResult(
				null != _matchAction
					? _matchAction(matchBuilder.CompleteMatch(this, bodyMatchResult.Product))
					: bodyMatchResult.Product);
		}


		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
