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
			var literalExpression = new CapturingLiteralParsingExpression<string>("cat", StringComparison.Ordinal, c => "match");

			Assert.AreEqual("cat", literalExpression.Literal);
			Assert.AreEqual(ParsingExpressionKind.Literal, literalExpression.Kind);
		}

		[TestMethod]
		public void LiteralExpression_accepts_exact_string() {
			var literalExpression = new CapturingLiteralParsingExpression<string>("cat", StringComparison.Ordinal, matchContext => {
				Assert.AreEqual(0, matchContext.SourceRange.Index);
				Assert.AreEqual(3, matchContext.SourceRange.Length);

				return "match";
			});

			var matchingContext = new MatchingContext("cat");
			
			var matchingResult = literalExpression.Matches(matchingContext);

			Assert.IsNotNull(matchingResult);
			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void LiteralExpression_accepts_start() {
			var literalExpression = new CapturingLiteralParsingExpression<string>("cat", StringComparison.Ordinal, matchContext => {
				Assert.AreEqual(0, matchContext.SourceRange.Index);
				Assert.AreEqual(3, matchContext.SourceRange.Length);

				return "match";
			});

			var matchingContext = new MatchingContext("catatonic");

			var applicationResult = literalExpression.Matches(matchingContext);

			Assert.IsTrue(applicationResult.Succeeded);
			Assert.AreEqual("match", applicationResult.Product);
		}

		[TestMethod]
		public void LiteralExpression_rejects_invalid() {
			var literalExpression = new CapturingLiteralParsingExpression<string>("cat", StringComparison.Ordinal, matchContext => {
				Assert.Fail("action called");
				return "";
			});

			var matchingContext = new MatchingContext("bat");
			var matchingResult = literalExpression.Matches(matchingContext);

			Assert.IsFalse(matchingResult.Succeeded);
		}

		[TestMethod]
		public void LiteralExpression_rejects_short_input() {
			var literalExpression = new CapturingLiteralParsingExpression<string>("cat", StringComparison.Ordinal, matchContext => {
				Assert.Fail("action called");
				return "";
			});

			var matchingContext = new MatchingContext("ca");
			var applicationResult = literalExpression.Matches(matchingContext);

			Assert.IsFalse(applicationResult.Succeeded);
		}
	}
}
