using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pegleg.cs.Parsing.Expressions {
	[TestClass]
	public class ChoiceExpressionTests {
		[TestMethod]
		public void ChoiceExpression_constructor_works_as_expected() {
			var literalExpressionA = new LiteralParsingExpression<string>("a", m => "A");
			var literalExpressionB = new LiteralParsingExpression<string>("b", m => "B");
			var choiceExpression = new OrderedChoiceParsingExpression<string>(
				new IParsingExpression[] { literalExpressionA, literalExpressionB },
				match => "match");

			Assert.AreEqual(ParsingExpressionKind.OrderedChoice, choiceExpression.Kind);
			Assert.AreEqual(literalExpressionA, choiceExpression.Choices.ElementAt(0));
			Assert.AreEqual(literalExpressionB, choiceExpression.Choices.ElementAt(1));
		}

		[TestMethod]
		public void ChoiceExpression_matches_first_choice() {
			var choiceExpression = new OrderedChoiceParsingExpression<string>(
				new IParsingExpression[] {
					new LiteralParsingExpression<string>("a", m => "A"),
					new LiteralParsingExpression<string>("b", m => { Assert.Fail(); return "B"; }) },
				match => {
					Assert.AreEqual("A", match.Product);
					return "match";
				});

			var matchingContext = new MatchingContext("a");
			var matchingResult = choiceExpression.Match(matchingContext);

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void ChoiceExpression_matches_second_choice() {
			var choiceExpression = new OrderedChoiceParsingExpression<string>(
				new IParsingExpression[] {
					new LiteralParsingExpression<string>("a", m => "A"),
					new LiteralParsingExpression<string>("b", m => "B") },
				match => {
					Assert.AreEqual("B", match.Product);
					Assert.AreEqual(0, match.Index);
					return "match";
				});

			var matchingContext = new MatchingContext("b");
			var matchingResult = choiceExpression.Match(matchingContext);

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void ChoiceExpression_order_implies_precedence() {
			var choiceExpression = new OrderedChoiceParsingExpression<string>(
				new IParsingExpression[] {
					new LiteralParsingExpression<string>("aa", m => m.Product),
					new LiteralParsingExpression<string>("a", m => m.Product) },
				match => {
					Assert.AreEqual("aa", match.Product);
					return "match";
				});

			var matchingContext = new MatchingContext("aaa");
			var matchingResult = choiceExpression.Match(matchingContext); 

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void ChoiceExpression_rejects_when_no_choice_matches() {
			var choiceExpression = new OrderedChoiceParsingExpression<string>(
				new IParsingExpression[] {
					new LiteralParsingExpression<string>("a", m => { Assert.Fail(); return m.Product; }),
					new LiteralParsingExpression<string>("b", m => { Assert.Fail(); return m.Product; }) },
				match => {
					Assert.Fail("match action called");
					return "match";
				});

			var matchingContext = new MatchingContext("c");
			var matchingResult = choiceExpression.Match(matchingContext);

			Assert.IsFalse(matchingResult.Succeeded);
		}
	}
}
