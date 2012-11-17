using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Parsing.Expressions {
	public class SequenceParsingExpressionTests {
		[Fact] public void NonCapturingSequenceParsingExpression_should_not_consume_partial_match() {
			var candidate = new NonCapturingSequenceParsingExpression(new IParsingExpression[] {
				new NonCapturingLiteralParsingExpression("a", StringComparison.Ordinal),
				new NonCapturingLiteralParsingExpression("b", StringComparison.Ordinal)
			});

			var context = new MatchingContext("ac");

			var match = candidate.Matches(context);

			match.Succeeded.Should().BeFalse();
			context.Index.Should().Be(0);
		}

		[Fact] public void CapturingSequenceParsingExpression_should_not_consume_partial_match() {
			var candidate = new CapturingSequenceParsingExpression<bool>(
				new IParsingExpression[] {
					new NonCapturingLiteralParsingExpression("a", StringComparison.Ordinal),
					new NonCapturingLiteralParsingExpression("b", StringComparison.Ordinal)
				},
				m => true
			);

			var context = new MatchingContext("ac");

			var match = candidate.Matches(context);

			match.Succeeded.Should().BeFalse();
			context.Index.Should().Be(0);
		}
	}
}
