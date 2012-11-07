using markdom.cs.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	[TestClass]
	public class NullLiteralExpressionTests : GrammarTestFixture {
		[TestMethod]
		public void NullLiteralExpression_matches_null() {
			var match = Grammar.NullLiteralExpression.AssertMatch("null");

			Assert.AreEqual(ExpressionKind.NullLiteral, match.Product.Kind);
		}

		[TestMethod]
		public void NullLiteralExpression_does_not_match_NULL() {
			Grammar.NullLiteralExpression.AssertNoMatch("NULL");
		}
	}
}
