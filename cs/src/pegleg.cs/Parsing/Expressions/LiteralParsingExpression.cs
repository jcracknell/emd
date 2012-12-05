using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class LiteralParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		protected readonly string _literal;

		public LiteralParsingExpression(string literal) {
			CodeContract.ArgumentIsNotNull(() => literal, literal);

			_literal = literal;
		}

		public string Literal { get { return _literal; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingLiteralParsingExpression : LiteralParsingExpression<Nil> {
		public NonCapturingLiteralParsingExpression(string literal) : base(literal) { }

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			if(context.ConsumesMatching(_literal))
				return SuccessfulMatchingResult.Create(Nil.Value, _literal.Length);
			else
				return UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class CapturingLiteralParsingExpression<TProduct> : LiteralParsingExpression<TProduct> {
		private readonly Func<IMatch<string>, TProduct> _matchAction;

		public CapturingLiteralParsingExpression(string literal, Func<IMatch<string>, TProduct> matchAction)
			: base(literal)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);

			if(context.ConsumesMatching(_literal))
				return matchBuilder.Success(_literal, _matchAction);	

			return UnsuccessfulMatchingResult.Create(this);
		}
	}
}
