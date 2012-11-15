using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class NamedParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		protected readonly string _name;
		protected readonly IParsingExpression<TProduct> _named;

		public NamedParsingExpression(string name, IParsingExpression<TProduct> named)
			: base(ParsingExpressionKind.Named)
		{
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => named, named);
			
			_name = name;
			_named = named;	
		}

		public string Name { get { return _name; } }

		public IParsingExpression<TProduct> Named { get { return _named; } }

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			return _named.Matches(context);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
