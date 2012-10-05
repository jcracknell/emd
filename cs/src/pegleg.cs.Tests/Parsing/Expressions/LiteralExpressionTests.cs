using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pegleg.cs.Parsing.Expressions {
	[TestClass]
	public class LiteralExpressionTests {
		[TestMethod]
		public void LiteralExpression_constructor_works_as_expected() {
			var literalExpression = new LiteralExpression<string>("cat", c => "match");

			Assert.AreEqual("cat", literalExpression.Literal);
			Assert.AreEqual(ExpressionType.Literal, literalExpression.ExpressionType);
		}

		[TestMethod]
		public void LiteralExpression_accepts_exact_string() {
			var literalExpression = new LiteralExpression<string>("cat", matchContext => {
				Assert.AreEqual(0, matchContext.SourceRange.Index);
				Assert.AreEqual(3, matchContext.SourceRange.Length);

				return "match";
			});

			var matchingContext = new ExpressionMatchingContext("cat");
			
			var matchingResult = literalExpression.Match(matchingContext);

			Assert.IsNotNull(matchingResult);
			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void LiteralExpression_accepts_start() {
			var literalExpression = new LiteralExpression<string>("cat", matchContext => {
				Assert.AreEqual(0, matchContext.SourceRange.Index);
				Assert.AreEqual(3, matchContext.SourceRange.Length);

				return "match";
			});

			var matchingContext = new ExpressionMatchingContext("catatonic");

			var applicationResult = literalExpression.Match(matchingContext);

			Assert.IsTrue(applicationResult.Succeeded);
			Assert.AreEqual("match", applicationResult.Product);
		}

		[TestMethod]
		public void LiteralExpression_rejects_invalid() {
			var literalExpression = new LiteralExpression<string>("cat", matchContext => {
				Assert.Fail("action called");
				return "";
			});

			var matchingContext = new ExpressionMatchingContext("bat");
			var matchingResult = literalExpression.Match(matchingContext);

			Assert.IsFalse(matchingResult.Succeeded);
		}

		[TestMethod]
		public void LiteralExpression_rejects_short_input() {
			var literalExpression = new LiteralExpression<string>("cat", matchContext => {
				Assert.Fail("action called");
				return "";
			});

			var matchingContext = new ExpressionMatchingContext("ca");
			var applicationResult = literalExpression.Match(matchingContext);

			Assert.IsFalse(applicationResult.Succeeded);
		}
	}
}
