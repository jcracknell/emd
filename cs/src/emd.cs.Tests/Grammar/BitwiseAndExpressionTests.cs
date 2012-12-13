using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class BitwiseAndExpressionTests {
		[Fact] public void BitwiseAndExpression_should_match_identifier_and_hex_literal() {
			var match = MarkdomGrammar.BitwiseAndExpression.ShouldMatch("@foo & 0x01");

			match.Product.GetType().Should().Be(typeof(BitwiseAndExpression));
			match.Product.ShouldBeEquivalentTo(
				new BitwiseAndExpression(
					new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
					new NumericLiteralExpression(1d, new SourceRange(7,4,1,7)),
					new SourceRange(1,10,1,1)
				)
			);
		}
	}
}
