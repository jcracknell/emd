using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class BitwiseXOrExpressionTests {
		[Fact] public void BitwiseXOrExpression_should_match_identifier_xor_numeric_literal() {
			var match = EmdGrammar.BitwiseXOrExpression.ShouldMatch("@foo ^ 4");

			match.Product.GetType().Should().Be(typeof(BitwiseXOrExpression));
			match.Product.ShouldBeEquivalentTo(
				new BitwiseXOrExpression(
					new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
					new NumericLiteralExpression(4d, new SourceRange(7,1,1,7)),
					new SourceRange(1,7,1,1)
				)
			);
		}
	}
}
