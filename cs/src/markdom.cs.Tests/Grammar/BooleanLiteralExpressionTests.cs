using markdom.cs.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	[TestClass]
	public class BooleanLiteralExpressionTests : GrammarTestFixture {
		[TestMethod]
		public void BooleanLiteralExpression_matches_true() {
			var match = Grammar.BooleanLiteralExpression.AssertMatch("true");

			Assert.IsTrue(match.Product.Value);
			Assert.AreEqual(ExpressionKind.BooleanLiteral, match.Product.Kind);
		}

		[TestMethod]
		public void BooleanLiteralExpression_matches_false() {
			var match = Grammar.BooleanLiteralExpression.AssertMatch("false");

			Assert.IsFalse(match.Product.Value);
			Assert.AreEqual(ExpressionKind.BooleanLiteral, match.Product.Kind);
		}

		[TestMethod]
		public void BooleanLiteralExpression_does_not_match_TRUE() {
			var match = Grammar.BooleanLiteralExpression.AssertNoMatch("TRUE");
		}

		[TestMethod]
		public void BooleanLiteralExpression_does_not_match_FALSE() {
			var match = Grammar.BooleanLiteralExpression.AssertNoMatch("FALSE");
		}
	}
}
