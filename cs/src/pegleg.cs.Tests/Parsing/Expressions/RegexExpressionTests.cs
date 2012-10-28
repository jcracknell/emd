using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pegleg.cs.Parsing.Expressions {
	[TestClass]
	public class RegexExpressionTests  {
		[TestMethod]
		public void RegexExpression_constructor_works_as_expected() {
			var regex = new Regex("abc");
			var regexExpression = new RegexParsingExpression<string>(regex, (matchContext) => "match");

			Assert.AreEqual(ParsingExpressionKind.Regex, regexExpression.Kind);
			Assert.AreEqual(regex, regexExpression.Regex);
		}

		[TestMethod]
		public void RegexExpression_accepts_trivial_match() {
			var regexExpression = new RegexParsingExpression<string>(
				new Regex(@"cat"),
				(matchContext) => {
					Assert.AreEqual(0, matchContext.SourceRange.Index);
					Assert.AreEqual(3, matchContext.SourceRange.Length);
					Assert.AreEqual(0, matchContext.Product.Index);
					Assert.AreEqual(3, matchContext.Product.Length);
					Assert.AreEqual("cat", matchContext.Product.Groups[0].Value);

					return "match";
				});

			var matchingContext = new MatchingContext("cat");
			var matchingResult = regexExpression.Match(matchingContext);

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void RegexExpression_rejects_when_match_not_at_start() { 
			var regexExpression = new RegexParsingExpression<string>(
				new Regex(@"[0-9]+"),
				matchContext => {
					Assert.Fail("action called");
					return "match";
				});

			var matchingContext = new MatchingContext("abc123def");
			var matchingResult = regexExpression.Match(matchingContext);

			Assert.IsFalse(matchingResult.Succeeded);
		}
	}
}
