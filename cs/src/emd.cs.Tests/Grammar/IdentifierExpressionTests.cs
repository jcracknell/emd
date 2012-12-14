using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class IdentifierExpressionTests : GrammarTestFixture {
		[Fact] public void IdentifierExpression_matches_ascii() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch("foo");

			match.Product.Name.Should().Be("foo");
		}

		[Fact] public void IdentifierExpression_matches_ascii_and_numbers() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch("foo12");

			match.Product.Name.Should().Be("foo12");
		}

		[Fact] public void IdentifierExpression_does_not_match_numbers() {
			EmdGrammar.IdentifierExpression.ShouldNotMatch("42");
		}

		[Fact] public void IdentifierExpression_matches_dollar_sign() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch("$");

			match.Product.Name.Should().Be("$");
		}

		[Fact] public void IdentifierExpression_matches_underscore() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch("_");

			match.Product.Name.Should().Be("_");
		}

		[Fact] public void IdentifierExpression_should_match_unicode_escape_sequence() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch(@"\u0061");

			match.Product.Name.Should().Be("a");
		}

		[Fact] public void IdentifierExpression_should_not_match_true_keyword() {
			EmdGrammar.IdentifierExpression.ShouldNotMatch("true");
		}

		[Fact] public void IdentifierExpression_should_match_true_keyword_followed_by_identifier_part() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch("trueish");

			match.Product.Name.Should().Be("trueish");
		}

		[Fact] public void IdentifierExpression_matches_unicode_lowercase_omega() {
			var match = EmdGrammar.IdentifierExpression.ShouldMatch("ω");
		}
	}
}
