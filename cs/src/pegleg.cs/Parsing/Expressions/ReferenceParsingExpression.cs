using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class ReferenceParsingExpression : BaseParsingExpression {
		private readonly Func<IParsingExpression> _reference;
		private IParsingExpression _referencedExpression;

		public ReferenceParsingExpression(Func<IParsingExpression> reference)
			: base(ParsingExpressionKind.Reference)
		{
			_reference = reference;
			_referencedExpression = null;
		}

		public IParsingExpression Referenced { get { return _referencedExpression ?? (_referencedExpression = _reference()); } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingReferenceParsingExpression<TReferenced> : ReferenceParsingExpression, IParsingExpression<TReferenced> {
		public NonCapturingReferenceParsingExpression(Func<IParsingExpression<TReferenced>> referenced) : base(referenced) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var referenceMatchResult = Referenced.Matches(context);
			if(referenceMatchResult.Succeeded)
				return referenceMatchResult;

			return new UnsuccessfulMatchingResult();
		}
	}

	public class CapturingReferenceParsingExpression<TReferenced, TProduct> : ReferenceParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IMatch<TReferenced>, TProduct> _matchAction;

		public CapturingReferenceParsingExpression(Func<IParsingExpression<TReferenced>> reference, Func<IMatch<TReferenced>, TProduct> matchAction)
			: base(reference)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var referenceMatchResult = Referenced.Matches(context);

			if(!referenceMatchResult.Succeeded)
				return new UnsuccessfulMatchingResult();

			var product = _matchAction(matchBuilder.CompleteMatch(this, (TReferenced)referenceMatchResult.Product));
			return new SuccessfulMatchingResult(product);
		}
	}
}
