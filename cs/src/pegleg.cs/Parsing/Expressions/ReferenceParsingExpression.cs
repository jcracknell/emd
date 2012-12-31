using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class ReferenceParsingExpression<TReferenced, TProduct> : BaseParsingExpression<TProduct> {
		private readonly Func<IParsingExpression<TReferenced>> _reference;
		private IParsingExpression<TReferenced> _referencedExpression;

		public ReferenceParsingExpression(Func<IParsingExpression<TReferenced>> reference) {
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

		public override IMatchingResult<TReferenced> Matches(MatchingContext context) {
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
			if(null == matchAction) throw Xception.Because.ArgumentNull(() => matchAction);

			_matchAction = matchAction;
		}

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);

			var referenceMatchResult = Referenced.Matches(context);

			if(!referenceMatchResult.Succeeded)
				return UnsuccessfulMatchingResult.Create(this);

			return matchBuilder.Success(referenceMatchResult.Product, _matchAction);
		}
	}
}
