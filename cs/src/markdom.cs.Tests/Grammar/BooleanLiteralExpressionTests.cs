using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using markdom.cs.Expressions;

namespace markdom.cs.Grammar {
	public class BooleanLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void BooleanLiteralExpression_matches_true() {
			var match = Grammar.BooleanLiteralExpression.ShouldMatch("true");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().BeTrue();
		}

		[Fact] public void BooleanLiteralExpression_matches_false() {
			var match = Grammar.BooleanLiteralExpression.ShouldMatch("false");

			match.Succeeded.Should().BeTrue();
			match.Product.Value.Should().BeFalse();
		}

		[Fact] public void BooleanLiteralExpression_does_not_match_TRUE() {
			Grammar.BooleanLiteralExpression.ShouldNotMatch("TRUE");
		}

		[Fact] public void BooleanLiteralExpression_does_not_match_FALSE() {
			Grammar.BooleanLiteralExpression.ShouldNotMatch("FALSE");
		}
	}
}
