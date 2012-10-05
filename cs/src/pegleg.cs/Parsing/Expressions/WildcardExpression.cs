using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface WildcardExpression : IExpression {
	}

	public class WildcardExpression<TProduct> : WildcardExpression, IExpression<TProduct> {
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction = null;

		public WildcardExpression(Func<IExpressionMatch<string>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;			
		}

		public ExpressionType ExpressionType { get { return ExpressionType.Wildcard; } }

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			if(context.AtEndOfInput)
				return new UnsuccessfulExpressionMatchingResult();

			char c;
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();

				if(!context.TryConsumeAnyCharacter(out c))
					return new UnsuccessfulExpressionMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, c.ToString()));

				return new SuccessfulExpressionMatchingResult(product);
			} else {
				if(!context.TryConsumeAnyCharacter(out c))
					return new UnsuccessfulExpressionMatchingResult();

				return new SuccessfulExpressionMatchingResult(c.ToString());
			}
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
