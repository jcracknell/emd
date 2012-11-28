using FluentAssertions;
using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class BitwiseOrExpressionTests {
		[Fact] public void BitwiseOrExpression_should_match_identifier_or_numeric_literal() {
			var match = MarkdomGrammar.BitwiseOrExpression.ShouldMatch("@foo | 1");

			match.Product.GetType().Should().Be(typeof(BitwiseOrExpression));
			match.Product.ShouldBeEquivalentTo(
				new BitwiseOrExpression(
					new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
					new NumericLiteralExpression(1d, new SourceRange(7,1,1,7)),
					new SourceRange(1,7,1,1)
				)
			);
		}
	}
}
