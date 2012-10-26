using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface NamedExpression : IExpression {
		string Name { get; }
		IExpression Named { get; }
	}

	public class NamedExpression<TProduct> : NamedExpression, IExpression<TProduct> {
		private readonly string _name;
		private readonly IExpression _named;

		public NamedExpression(string name, IExpression<TProduct> named) {
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => named, named);

			_name = name;
			_named = named;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var namedApplicationResult = _named.Match(context);
			if(namedApplicationResult.Succeeded)
				return namedApplicationResult;

			// TODO: Make fancy error
			return namedApplicationResult;
		}

		public NamedExpression(string name, IExpression named) {
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => named, named);
			
			_name = name;
			_named = named;
		}

		public string Name { get { return _name; } }

		public IExpression Named { get { return _named; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Named; } }


		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			// Named expression is a little different in that it is generally not very
			// useful to display just a single name
			return _named.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
