using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class NamedParsingExpression : BaseParsingExpression {
		public NamedParsingExpression() : base(ParsingExpressionKind.Named) { }
		public abstract string Name { get; }
		public abstract IParsingExpression Named { get; }
	}

	public class NamedParsingExpression<TProduct> : NamedParsingExpression, IParsingExpression<TProduct> {
		private readonly string _name;
		private readonly IParsingExpression _named;

		public NamedParsingExpression(string name, IParsingExpression<TProduct> named) {
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => named, named);

			_name = name;
			_named = named;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var namedApplicationResult = _named.Matches(context);
			if(namedApplicationResult.Succeeded)
				return namedApplicationResult;

			// TODO: Make fancy error
			return namedApplicationResult;
		}

		public override string Name { get { return _name; } }

		public override IParsingExpression Named { get { return _named; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			// Named expression is a little different in that it is generally not very
			// useful to display just a single name
			return _named.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
