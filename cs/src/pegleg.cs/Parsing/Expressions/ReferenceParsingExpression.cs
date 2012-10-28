using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface ReferenceParsingExpression : IParsingExpression {
		IParsingExpression Referenced { get; }
	}

	public class ReferenceParsingExpression<TProduct> : ReferenceParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IParsingExpression> _reference;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;

		public ReferenceParsingExpression(Func<IParsingExpression> reference, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => reference, reference);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_reference = reference;
			_matchAction = matchAction;
		}

		public IParsingExpression Referenced {
			get { return _reference(); }
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Reference; } }

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IMatchingResult Match(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var referencedExpression = _reference();
			var referenceMatchResult = referencedExpression.Match(context);

			if(!referenceMatchResult.Succeeded)
				return referenceMatchResult;

			var product = _matchAction(matchBuilder.CompleteMatch(this, referenceMatchResult.Product));

			return new SuccessfulMatchingResult(product);
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
