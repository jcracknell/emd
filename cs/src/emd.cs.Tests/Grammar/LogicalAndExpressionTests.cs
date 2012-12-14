using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class LogicalAndExpressionTests {
		[Fact] public void LogicalAndExpression_should_match_identifier_and_identifier() {
			var match = EmdGrammar.LogicalAndExpression.ShouldMatch("@foo && @bar");

			match.Product.GetType().Should().Be(typeof(LogicalAndExpression));
			match.Product.ShouldBeEquivalentTo(
				new LogicalAndExpression(
					new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
					new IdentifierExpression("bar", new SourceRange(9,3,1,9)),
					new SourceRange(1,11,1,1)
				)
			);
		}
	}
}
