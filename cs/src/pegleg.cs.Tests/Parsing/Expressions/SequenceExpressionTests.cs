using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pegleg.cs.Parsing.Expressions {
	[TestClass]
	public class SequenceExpressionTests {
		[TestMethod]
		public void SequenceExpression_constructor_works_as_expected() {
			var literalExpressionA = new CapturingLiteralParsingExpression<string>("a", StringComparison.Ordinal, c => "A");
			var literalExpressionB = new CapturingLiteralParsingExpression<string>("b", StringComparison.Ordinal, c => "B");

			var sequenceExpression = new CapturingSequenceParsingExpression<string>(
				new IParsingExpression[] { literalExpressionA, literalExpressionB },
				matchContext => "seq");

			Assert.AreEqual(2, sequenceExpression.Sequence.Count());
		}

		[TestMethod]
		public void SequenceExpression_accepts_sequence_of_literals() {
			var literalExpressionA = new CapturingLiteralParsingExpression<string>("a", StringComparison.Ordinal, c => "A");
			var literalExpressionB = new CapturingLiteralParsingExpression<string>("b", StringComparison.Ordinal, c => "B");

			var sequenceExpression = new CapturingSequenceParsingExpression<string>(
				new IParsingExpression[] { literalExpressionA, literalExpressionB },
				matchContext => {
					Assert.AreEqual("A", matchContext.Product[0]);
					Assert.AreEqual("B", matchContext.Product[1]);

					return "seq";
				});

			var matchingContext = new MatchingContext("abcdef");
			var matchingResult = sequenceExpression.Matches(matchingContext);

			Assert.IsTrue(matchingResult.Succeeded);
			Assert.AreEqual("seq", matchingResult.Product);
		}
	}
}
