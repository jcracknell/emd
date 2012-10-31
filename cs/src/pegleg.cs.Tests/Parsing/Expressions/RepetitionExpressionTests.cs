using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pegleg.cs.Parsing.Expressions {
	[TestClass]
	public class RepetitionExpressionTests {
		[TestMethod]
		public void RepetitionExpression_constructor_works_as_expected() {
			var literalExpression = new LiteralParsingExpression<string>("a", m => m.Product);
			var repetitionExpression = new RepetitionParsingExpression<string, string>(42, 0, literalExpression, m => "match");

			Assert.AreEqual(ParsingExpressionKind.Repetition, repetitionExpression.Kind);
			Assert.AreEqual(42u, repetitionExpression.MinOccurs);
			Assert.AreEqual(literalExpression, repetitionExpression.Body);
		}

		[TestMethod]
		public void RepetitionExpression_matches_nothing_with_minOccurs_0() {
			var repetitionExpression = new RepetitionParsingExpression<string, string>(
				0, 0,
				new LiteralParsingExpression<string>("abcd", m => m.Product),
				match => {
					Assert.AreEqual(0, match.SourceRange.Index);
					Assert.AreEqual(0, match.SourceRange.Length);
					Assert.AreEqual(0, match.Product.Count());
					return "match";
				});

			var matchingContext = new MatchingContext("");
			var matchResult = repetitionExpression.Matches(matchingContext);

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual("match", matchResult.Product);
		}
	}
}
