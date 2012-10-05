using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Rules {
	public class NamedRule<TProduct> : IRule<TProduct> {
		private string _name;
		private IRule<TProduct> _named;

		public NamedRule(string name, IRule<TProduct> named) {
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => named, named);

			_name = name;
			_named = named;
		}

		public string Name { get { return _name; } }

		public IRule<TProduct> Named { get { return _named; } }

		public IRuleApplicationResult<TProduct> Apply(IRuleApplicationContext context) {
			return _named.Apply(context);
		}
	}
}
