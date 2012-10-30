using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class ReferenceParsingExpression : BaseParsingExpression {
		public ReferenceParsingExpression() : base(ParsingExpressionKind.Reference) { }

		public abstract IParsingExpression Referenced { get; }
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

		public override IParsingExpression Referenced { get { return _reference(); } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var referencedExpression = _reference();
			var referenceMatchResult = referencedExpression.Matches(context);

			if(!referenceMatchResult.Succeeded)
				return referenceMatchResult;

			var product = _matchAction(matchBuilder.CompleteMatch(this, referenceMatchResult.Product));

			return new SuccessfulMatchingResult(product);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
