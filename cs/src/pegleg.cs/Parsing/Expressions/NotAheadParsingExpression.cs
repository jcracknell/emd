using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface NotAheadParsingExpression : IParsingExpression {
		IParsingExpression Body { get; }
	}

	public class NotAheadParsingExpression<TProduct> : NotAheadParsingExpression, IParsingExpression<TProduct> {
		private readonly IParsingExpression _body;
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public NotAheadParsingExpression(IParsingExpression body, Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_body = body;
			_matchAction = matchAction;
		}

		public IParsingExpression Body { get { return _body; } }

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.NegativeLookahead; } }

		public IMatchingResult Match(IMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Match(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				return new UnsuccessfulMatchingResult();
			} else {
				var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));

				return new SuccessfulMatchingResult(product);
			}
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
