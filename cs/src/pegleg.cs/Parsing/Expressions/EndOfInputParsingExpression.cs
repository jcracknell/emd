using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class EndOfInputParsingExpression : BaseParsingExpression<Nil> {
		private static readonly EndOfInputParsingExpression _instance;

		public static EndOfInputParsingExpression Instance { get { return _instance; } }

		static EndOfInputParsingExpression() {
			_instance = new EndOfInputParsingExpression();
		}

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			if(context.AtEndOfInput)
				return SuccessfulMatchingResult.Create(Nil.Value, 0);

			return UnsuccessfulMatchingResult.Create(this);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
