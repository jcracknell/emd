using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class NotAheadParsingExpression : BaseParsingExpression<Nil> {
		protected readonly IParsingExpression _body;

		public NotAheadParsingExpression(IParsingExpression body)
			: base(ParsingExpressionKind.NegativeLookahead)
		{
			CodeContract.ArgumentIsNotNull(() => body, body);
			
			_body = body;
		}

		public IParsingExpression Body { get { return _body; } }

		protected override IMatchingResult<Nil> MatchesCore(MatchingContext context) {
			var bodyMatch = _body.Matches(context.Clone());
			if(bodyMatch.Succeeded)
				return UnsuccessfulMatchingResult.Create(this);
			return SuccessfulMatchingResult.NilProduct;
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
