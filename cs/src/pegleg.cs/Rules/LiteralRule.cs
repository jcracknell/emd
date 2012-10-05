using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Rules {
	public class LiteralRule<TProduct> : IRule<TProduct> {
		private string _literal;
		private Func<string, TProduct> _product;

		public LiteralRule(string literal, Func<string, TProduct> product) {
			CodeContract.ArgumentIsNotNull(() => literal, literal);	

			_literal = literal;
			_product = product;
		}

		public string Literal { get { return _literal; } }

		public IRuleApplicationResult<TProduct> Apply(IRuleApplicationContext context) {
			if(context.Matches(Literal)) {
				context.Consume(Literal.Length);
				
				return new SuccessfulRuleApplicationResult<TProduct>(_product(_literal));
			} else {
				return new FailedRuleApplicationResult<TProduct>();
			}
		}
	}
}
