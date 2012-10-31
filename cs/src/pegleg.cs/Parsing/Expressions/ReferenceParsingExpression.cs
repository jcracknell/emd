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
		private readonly Func<IMatch<object>, TProduct> _matchAction;
		private IParsingExpression _referencedExpression;

		public ReferenceParsingExpression(Func<IParsingExpression> reference, Func<IMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => reference, reference);

			_reference = reference;
			_matchAction = matchAction;
			_referencedExpression = null;
		}

		public override IParsingExpression Referenced { get { return _reference(); } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(null == _referencedExpression)
				_referencedExpression = _reference();

			// If there is no matchaction, then the ReferenceParsingExpression is completely transparent
			if(null == _matchAction)
				return _referencedExpression.Matches(context);

			var matchBuilder = context.StartMatch();

			var referenceMatchResult = _referencedExpression.Matches(context);

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
