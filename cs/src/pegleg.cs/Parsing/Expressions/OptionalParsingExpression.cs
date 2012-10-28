using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface OptionalParsingExpression : IParsingExpression {
		IParsingExpression Body { get; }
	}

	public class OptionalParsingExpression<TProduct> : OptionalParsingExpression, IParsingExpression<TProduct> {
		private readonly IParsingExpression _body;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;
		private readonly Func<IExpressionMatch<Nil>, TProduct> _noMatchAction;

		public OptionalParsingExpression(IParsingExpression body, Func<IExpressionMatch<object>, TProduct> matchAction, Func<IExpressionMatch<Nil>, TProduct> noMatchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
			_matchAction = matchAction;
			_noMatchAction = noMatchAction;
		}

		public IMatchingResult Match(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var bodyApplicationContext = context.Clone();
			var bodyMatchResult = _body.Match(bodyApplicationContext);

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

		public IParsingExpression Body { get { return _body; } }

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Optional; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
