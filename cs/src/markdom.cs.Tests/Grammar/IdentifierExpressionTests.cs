using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class IdentifierExpressionTests : GrammarTestFixture {
		[Fact] public void IdentifierExpression_matches_ascii() {
			var match = Grammar.IdentifierExpression.ShouldMatch("foo");

			match.Product.Name.Should().Be("foo");
		}

		[Fact] public void IdentifierExpression_matches_ascii_and_numbers() {
			var match = Grammar.IdentifierExpression.ShouldMatch("foo12");

			match.Product.Name.Should().Be("foo12");
		}

		[Fact] public void IdentifierExpression_does_not_match_numbers() {
			Grammar.IdentifierExpression.ShouldNotMatch("42");
		}

		[Fact] public void IdentifierExpression_matches_dollar_sign() {
			var match = Grammar.IdentifierExpression.ShouldMatch("$");

			match.Product.Name.Should().Be("$");
		}

		[Fact] public void IdentifierExpression_matches_underscore() {
			var match = Grammar.IdentifierExpression.ShouldMatch("_");

			match.Product.Name.Should().Be("_");
		}

		[Fact] public void IdentifierExpression_should_not_match_true_keyword() {
			Grammar.IdentifierExpression.ShouldNotMatch("true");
		}

		[Fact] public void IdentifierExpression_should_match_true_keyword_followed_by_identifier_part() {
			var match = Grammar.IdentifierExpression.ShouldMatch("trueish");

			match.Product.Name.Should().Be("trueish");
		}

		[Fact] public void IdentifierExpression_matches_unicode_lowercase_omega() {
			var match = Grammar.IdentifierExpression.ShouldMatch("ω");
		}
	}
}
