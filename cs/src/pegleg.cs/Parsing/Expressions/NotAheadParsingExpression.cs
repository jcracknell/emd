using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class NotAheadParsingExpression : BaseParsingExpression<Nil> {
		protected readonly IParsingExpression _body;

		public NotAheadParsingExpression(IParsingExpression body) {
			if(null == body) throw Xception.Because.ArgumentNull(() => body);
			
			_body = body;
		}

		public IParsingExpression Body { get { return _body; } }

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			var bodyMatch = _body.Matches(context.Clone());
			if(bodyMatch.Succeeded)
				return UnsuccessfulMatchingResult.Create(this);
			return SuccessfulMatchingResult.Create(Nil.Value, 0);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
