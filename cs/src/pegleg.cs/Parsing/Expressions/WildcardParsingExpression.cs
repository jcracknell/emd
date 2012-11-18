using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class WildcardParsingExpression : BaseParsingExpression<Nil> {
		private static readonly WildcardParsingExpression _instance;

		static WildcardParsingExpression() {
			_instance = new WildcardParsingExpression();
		}

		public static WildcardParsingExpression Instance { get { return _instance; } }

		private WildcardParsingExpression() : base(ParsingExpressionKind.Wildcard) { }

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			if(context.ConsumesAnyCharacter())
				return SuccessfulMatchingResult.Create(Nil.Value, 1);

			return UnsuccessfulMatchingResult.Create(this);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
