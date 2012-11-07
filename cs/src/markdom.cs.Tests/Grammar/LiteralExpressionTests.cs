using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	[TestClass]
	public class LiteralExpressionTests : GrammarTestFixture {
		[TestMethod]
		public void LiteralExpression_matches_null() {
			var match = Grammar.LiteralExpression.AssertMatch("null");
			
			Assert.AreEqual(ExpressionKind.NullLiteral, match.Product.Kind);
		}

		[TestMethod]
		public void LiteralExpression_matches_true() {
			var match = Grammar.LiteralExpression.AssertMatch("true");

			Assert.AreEqual(ExpressionKind.BooleanLiteral, match.Product.Kind);
		}

		[TestMethod]
		public void LiteralExpression_matches_false() {
			var match = Grammar.LiteralExpression.AssertMatch("false");

			Assert.AreEqual(ExpressionKind.BooleanLiteral, match.Product.Kind);
		}

		[TestMethod]
		public void LiteralExpression_matches_integer_literal() {
			var match = Grammar.LiteralExpression.AssertMatch("42");

			Assert.AreEqual(ExpressionKind.NumericLiteral, match.Product.Kind);
			Assert.AreEqual(2, match.Product.SourceRange.Length);
		}

		[TestMethod]
		public void LiteralExpression_matches_floating_point_literal() {
			var match = Grammar.LiteralExpression.AssertMatch("42.123");

			Assert.AreEqual(ExpressionKind.NumericLiteral, match.Product.Kind);
			Assert.AreEqual(6, match.Product.SourceRange.Length);
		}

		[TestMethod]
		public void LiteralExpression_matches_single_quoted_string_literal() {
			var match = Grammar.LiteralExpression.AssertMatch("'foobar'");
			Assert.AreEqual(ExpressionKind.StringLiteral, match.Product.Kind);
			Assert.AreEqual(8, match.Product.SourceRange.Length);
		}

		[TestMethod]
		public void LiteralExpression_matches_double_quoted_string_literal() {
			var match = Grammar.LiteralExpression.AssertMatch("\"fizzbuzz\"");
			Assert.AreEqual(ExpressionKind.StringLiteral, match.Product.Kind);
			Assert.AreEqual(10, match.Product.SourceRange.Length);
		}

		[TestMethod]
		public void LiteralExpression_matches_uri_literal() {
			var match = Grammar.LiteralExpression.AssertMatch("http://reddit.com");
			Assert.AreEqual(ExpressionKind.UriLiteral, match.Product.Kind);
			Assert.AreEqual(17, match.Product.SourceRange.Length);
		}
	}
}
