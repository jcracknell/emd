using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class ReferenceParsingExpression<TReferenced, TProduct> : BaseParsingExpression<TProduct> {
		private readonly Func<IParsingExpression<TReferenced>> _reference;
		private IParsingExpression<TReferenced> _referencedExpression;

		public ReferenceParsingExpression(Func<IParsingExpression<TReferenced>> reference)
			: base(ParsingExpressionKind.Reference)
		{
			_reference = reference;
			_referencedExpression = null;
		}

		public IParsingExpression<TReferenced> Referenced {
			get { return _referencedExpression ?? (_referencedExpression = _reference()); }
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingReferenceParsingExpression<TReferenced> : ReferenceParsingExpression<TReferenced, TReferenced> {
		public NonCapturingReferenceParsingExpression(Func<IParsingExpression<TReferenced>> referenced) : base(referenced) { }

		protected override IMatchingResult<TReferenced> MatchesCore(MatchingContext context) {
			var referenceMatchResult = Referenced.Matches(context);
			if(referenceMatchResult.Succeeded)
				return referenceMatchResult;

			return UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class CapturingReferenceParsingExpression<TReferenced, TProduct> : ReferenceParsingExpression<TReferenced, TProduct> {
		private readonly Func<IMatch<TReferenced>, TProduct> _matchAction;

		public CapturingReferenceParsingExpression(Func<IParsingExpression<TReferenced>> reference, Func<IMatch<TReferenced>, TProduct> matchAction)
			: base(reference)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult<TProduct> MatchesCore(MatchingContext context) {
			var matchBuilder = context.StartMatch();

			var referenceMatchResult = Referenced.Matches(context);

			if(!referenceMatchResult.Succeeded)
				return UnsuccessfulMatchingResult.Create(this);

			var product = _matchAction(matchBuilder.CompleteMatch(this, (TReferenced)referenceMatchResult.Product));
			return SuccessfulMatchingResult.Create(product);
		}
	}
}
