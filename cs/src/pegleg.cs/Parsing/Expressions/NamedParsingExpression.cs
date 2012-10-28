using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface NamedParsingExpression : IParsingExpression {
		string Name { get; }
		IParsingExpression Named { get; }
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

		public IMatchingResult Match(IMatchingContext context) {
			var namedApplicationResult = _named.Match(context);
			if(namedApplicationResult.Succeeded)
				return namedApplicationResult;

			// TODO: Make fancy error
			return namedApplicationResult;
		}

		public NamedParsingExpression(string name, IParsingExpression named) {
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => named, named);
			
			_name = name;
			_named = named;
		}

		public string Name { get { return _name; } }

		public IParsingExpression Named { get { return _named; } }

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Named; } }


		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			// Named expression is a little different in that it is generally not very
			// useful to display just a single name
			return _named.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
