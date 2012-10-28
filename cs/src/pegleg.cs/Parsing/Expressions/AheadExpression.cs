using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface AheadExpression : IParsingExpression {
		IParsingExpression Body { get; }
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

		public IParsingExpression Body { get { return _body; } }

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Lookahead; } }

		public IMatchingResult Match(IMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Match(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				var product = _matchAction(context.StartMatch().CompleteMatch(this, bodyMatchingResult.Product));

				return new SuccessfulMatchingResult(product);
			} else {
				return new UnsuccessfulMatchingResult();
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
