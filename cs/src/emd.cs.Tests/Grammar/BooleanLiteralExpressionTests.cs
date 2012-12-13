using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using emd.cs.Expressions;

namespace emd.cs.Grammar {
	public class BooleanLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void BooleanLiteralExpression_matches_true() {
			var match = MarkdomGrammar.BooleanLiteralExpression.ShouldMatch("true");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().BeTrue();
		}

		[Fact] public void BooleanLiteralExpression_matches_false() {
			var match = MarkdomGrammar.BooleanLiteralExpression.ShouldMatch("false");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().BeFalse();
		}

		[Fact] public void BooleanLiteralExpression_does_not_match_TRUE() {
			MarkdomGrammar.BooleanLiteralExpression.ShouldNotMatch("TRUE");
		}

		[Fact] public void BooleanLiteralExpression_does_not_match_FALSE() {
			MarkdomGrammar.BooleanLiteralExpression.ShouldNotMatch("FALSE");
		}
	}
}
