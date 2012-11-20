using FluentAssertions;
using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class ShiftExpressionTests {
		[Fact] public void ShiftExpression_should_match_left_shift_numeric_literals() {
			var match = MarkdomGrammar.ShiftExpression.ShouldMatch("1<<1");

			match.Product.GetType().Should().Be(typeof(LeftShiftExpression));
			match.Product.ShouldBeEquivalentTo(
				new LeftShiftExpression(
					new NumericLiteralExpression(1d, new SourceRange(0,1,1,0)),
					new NumericLiteralExpression(1d, new SourceRange(3,1,1,3)),
					new SourceRange(0,4,1,0)
				)
			);
		}

		[Fact] public void ShiftExpression_should_match_right_shift_numeric_literals() {
			var match = MarkdomGrammar.ShiftExpression.ShouldMatch("1>>1");

			match.Product.GetType().Should().Be(typeof(RightShiftExpression));
			match.Product.ShouldBeEquivalentTo(
				new RightShiftExpression(
					new NumericLiteralExpression(1d, new SourceRange(0,1,1,0)),
					new NumericLiteralExpression(1d, new SourceRange(3,1,1,3)),
					new SourceRange(0,4,1,0)
				)
			);
		}

		[Fact] public void ShiftExpression_should_match_unsigned_right_shift_numeric_literals() {
			var match = MarkdomGrammar.ShiftExpression.ShouldMatch("42>>>2");

			match.Product.GetType().Should().Be(typeof(UnsignedRightShiftExpression));
			match.Product.ShouldBeEquivalentTo(
				new UnsignedRightShiftExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(2d, new SourceRange(5,1,1,5)),
					new SourceRange(0,6,1,0)
				)
			);
		}
	}
}
