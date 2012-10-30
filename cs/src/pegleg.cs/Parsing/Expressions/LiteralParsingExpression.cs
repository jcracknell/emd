using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class LiteralParsingExpression : BaseParsingExpression {
		public LiteralParsingExpression() : base(ParsingExpressionKind.Literal) { }

		public abstract string Literal { get; }
	}

	public class LiteralParsingExpression<TProduct> : LiteralParsingExpression, IParsingExpression<TProduct> {
		private readonly string _literal;
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction;

		public LiteralParsingExpression(string literal, Func<IExpressionMatch<string>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => literal, literal);

			_literal = literal;
			_matchAction = matchAction;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();

				if(!context.ConsumesMatching(_literal))
					return new UnsuccessfulMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, _literal));

				return new SuccessfulMatchingResult(product);
			} else {
				if(!context.ConsumesMatching(_literal))
					return new UnsuccessfulMatchingResult();
				return new SuccessfulMatchingResult(_literal);
			}
		}

		public override string Literal { get { return _literal; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
