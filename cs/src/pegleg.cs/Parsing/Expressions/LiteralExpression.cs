using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface LiteralExpression : IExpression {
		string Literal { get; }
	}

	public class LiteralExpression<TProduct> : LiteralExpression, IExpression<TProduct> {
		private readonly string _literal;
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction;

		public LiteralExpression(string literal, Func<IExpressionMatch<string>, TProduct> matchAction)
		{
			CodeContract.ArgumentIsNotNull(() => literal, literal);

			_literal = literal;
			_matchAction = matchAction;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();

				if(!context.TryConsumeMatching(_literal))
					return new UnsuccessfulExpressionMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, _literal));

				return new SuccessfulExpressionMatchingResult(product);
			} else {
				if(!context.TryConsumeMatching(_literal))
					return new UnsuccessfulExpressionMatchingResult();
				return new SuccessfulExpressionMatchingResult(_literal);
			}
		}

		public string Literal { get { return _literal; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Literal; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
