using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class NumericLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void NumericLiteralExpression_matches_integer() {
			var match = MarkdomGrammar.NumericLiteralExpression.ShouldMatch("42");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().Be(42d);
		}

		[Fact] public void NumericLiteralExpression_matches_with_no_integer_part() {
			var match = MarkdomGrammar.NumericLiteralExpression.ShouldMatch(".123");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().Be(0.123d);
		}

		[Fact] public void NumericLiteralExpression_matches_with_exponent_part() {
			var match = MarkdomGrammar.NumericLiteralExpression.ShouldMatch("4.2E1");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().Be(42d);
		}

		[Fact] public void NumericLiteral_matches_hexadecimal_integer() {
			var match = MarkdomGrammar.NumericLiteralExpression.ShouldMatch("0xdeadbeef");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().Be(3735928559d);
		}

		[Fact] public void NumericLiteralExpression_should_not_match_negative_integer() {
			MarkdomGrammar.NumericLiteralExpression.ShouldNotMatch("-42");
		}
	}
}
