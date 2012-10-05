using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface ReferenceExpression : IExpression {
		IExpression Referenced { get; }
	}

	public class ReferenceExpression<TProduct> : ReferenceExpression, IExpression<TProduct> {
		private readonly Func<IExpression> _reference;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;

		public ReferenceExpression(Func<IExpression> reference, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => reference, reference);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_reference = reference;
			_matchAction = matchAction;
		}

		public IExpression Referenced {
			get { return _reference(); }
		}

		public ExpressionType ExpressionType { get { return ExpressionType.Reference; } }

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var referencedExpression = _reference();
			var referenceMatchResult = referencedExpression.Match(context);

			if(!referenceMatchResult.Succeeded)
				return referenceMatchResult;

			var product = _matchAction(matchBuilder.CompleteMatch(this, referenceMatchResult.Product));

			return new SuccessfulExpressionMatchingResult(product);
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
