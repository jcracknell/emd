using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class OptionalParsingExpression<TBody, TProduct> : BaseParsingExpression<TProduct> {
		protected readonly IParsingExpression<TBody> _body;

		public OptionalParsingExpression(IParsingExpression<TBody> body) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
		}

		public IParsingExpression<TBody> Body { get { return _body; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingOptionalParsingExpression<TBody> : OptionalParsingExpression<TBody, TBody> {
		public NonCapturingOptionalParsingExpression(IParsingExpression<TBody> body) : base(body) { }

		public override IMatchingResult<TBody> Matches(MatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchResult.Succeeded) {
				context.Assimilate(bodyMatchingContext);
				return bodyMatchResult;
			}

			return SuccessfulMatchingResult.Create(default(TBody), 0);
		}
	}

	public class CapturingOptionalParsingExpression<TBody, TProduct> : OptionalParsingExpression<TBody, TProduct> {
		private readonly Func<IMatch<TBody>, TProduct> _matchAction;
		private readonly Func<IMatch<Nil>, TProduct> _noMatchAction;

		public CapturingOptionalParsingExpression(IParsingExpression<TBody> body, Func<IMatch<TBody>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction)
			: base(body)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
			CodeContract.ArgumentIsNotNull(() => noMatchAction, noMatchAction);

			_matchAction = matchAction;
			_noMatchAction = noMatchAction;
		}

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);

			var bodyMatchingContext = context.Clone();
			var bodyMatchResult = _body.Matches(bodyMatchingContext);

			if(bodyMatchResult.Succeeded) {
				context.Assimilate(bodyMatchingContext);

				return matchBuilder.Success(bodyMatchResult.Product, _matchAction);
			} else {
				return matchBuilder.Success(Nil.Value, _noMatchAction);
			}
		}
	}
}
