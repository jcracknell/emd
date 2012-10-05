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
			var literalExpressionA = new LiteralExpression<string>("a", m => "A");
			var literalExpressionB = new LiteralExpression<string>("b", m => "B");
			var choiceExpression = new ChoiceExpression<string>(
				new IExpression[] { literalExpressionA, literalExpressionB },
				match => "match");

			Assert.AreEqual(ExpressionType.Choice, choiceExpression.ExpressionType);
			Assert.AreEqual(literalExpressionA, choiceExpression.Choices.ElementAt(0));
			Assert.AreEqual(literalExpressionB, choiceExpression.Choices.ElementAt(1));
		}

		[TestMethod]
		public void ChoiceExpression_matches_first_choice() {
			var choiceExpression = new ChoiceExpression<string>(
				new IExpression[] {
					new LiteralExpression<string>("a", m => "A"),
					new LiteralExpression<string>("b", m => { Assert.Fail(); return "B"; }) },
				match => {
					Assert.AreEqual("A", match.Product);
					return "match";
				});

			var matchingContext = new ExpressionMatchingContext("a");
			var matchingResult = choiceExpression.Match(matchingContext);

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void ChoiceExpression_matches_second_choice() {
			var choiceExpression = new ChoiceExpression<string>(
				new IExpression[] {
					new LiteralExpression<string>("a", m => "A"),
					new LiteralExpression<string>("b", m => "B") },
				match => {
					Assert.AreEqual("B", match.Product);
					Assert.AreEqual(0, match.Index);
					return "match";
				});

			var matchingContext = new ExpressionMatchingContext("b");
			var matchingResult = choiceExpression.Match(matchingContext);

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void ChoiceExpression_order_implies_precedence() {
			var choiceExpression = new ChoiceExpression<string>(
				new IExpression[] {
					new LiteralExpression<string>("aa", m => m.Product),
					new LiteralExpression<string>("a", m => m.Product) },
				match => {
					Assert.AreEqual("aa", match.Product);
					return "match";
				});

			var matchingContext = new ExpressionMatchingContext("aaa");
			var matchingResult = choiceExpression.Match(matchingContext); 

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("match", matchingResult.Product);
		}

		[TestMethod]
		public void ChoiceExpression_rejects_when_no_choice_matches() {
			var choiceExpression = new ChoiceExpression<string>(
				new IExpression[] {
					new LiteralExpression<string>("a", m => { Assert.Fail(); return m.Product; }),
					new LiteralExpression<string>("b", m => { Assert.Fail(); return m.Product; }) },
				match => {
					Assert.Fail("match action called");
					return "match";
				});

			var matchingContext = new ExpressionMatchingContext("c");
			var matchingResult = choiceExpression.Match(matchingContext);

			Assert.IsFalse(matchingResult.Succeeded);
		}
	}
}
