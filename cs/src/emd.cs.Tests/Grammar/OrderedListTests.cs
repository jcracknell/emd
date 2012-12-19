using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class OrderedListTests : GrammarTestFixture {
		[Fact] public void EnumeratorishAhead_matches_decimal_dot() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("42.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dot() {
			var match = EmdGrammar.Enumerator.ShouldMatch("42.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dot_with_value() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("42@43.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dot_with_value() {
			var match = EmdGrammar.Enumerator.ShouldMatch("42@43.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_lower_roman_dot() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("ix.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_should_not_match_lower_roman_upper_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("iX.");
		}

		[Fact] public void Enumerator_matches_lower_roman_dot() {
			var match = EmdGrammar.Enumerator.ShouldMatch("ix.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_should_not_match_lower_roman_upper_roman_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("iX.");
		}

		[Fact] public void EnumeratorishAhead_matches_upper_roman_dot() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("XVIII.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_should_not_match_upper_roman_lower_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("Xviii.");
		}

		[Fact] public void Enumerator_matches_upper_roman_dot() {
			var match = EmdGrammar.Enumerator.ShouldMatch("XVIII.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_should_not_match_upper_roman_lower_roman_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("Xv.");
		}

		[Fact] public void EnumeratorishAhead_matches_lower_alpha_dot() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("abc.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_should_not_match_lower_alpha_upper_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("aBC.");
		}

		[Fact] public void Enumerator_matches_lower_alpha_dot() {
			var match = EmdGrammar.Enumerator.ShouldMatch("abc.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_should_not_match_lower_alpha_upper_alpha_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("aB.");
		}

		[Fact] public void EnumeratorishAhead_matches_upper_alpha_dot() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("ABC.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_should_not_match_upper_alpha_lower_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("Ab.");
		}

		[Fact] public void Enumerator_matches_upper_alpha_dot() {
			var match = EmdGrammar.Enumerator.ShouldMatch("ABC.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_should_not_match_upper_alpha_lower_alpha_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("Ab.");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("42-");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash_with_optional_space() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("42 -");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dash() {
			var match = EmdGrammar.Enumerator.ShouldMatch("42-");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dash_with_optional_space() {
			var match =EmdGrammar.Enumerator.ShouldMatch("42 -");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_parenthesis() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_parenthesis() {
			var match = EmdGrammar.Enumerator.ShouldMatch("42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_alpha_parethesis_with_value() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("a@42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_alpha_parenthesis_with_value() {
			var match = EmdGrammar.Enumerator.ShouldMatch("a@42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAheah_matches_decimal_parentheses() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("(42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_parentheses() {
			var match = EmdGrammar.Enumerator.ShouldMatch("(42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_bracketed() {
			var match = EmdGrammar.EnumeratorishAhead.ShouldMatch("[42]");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_bracketed() {
			var match = EmdGrammar.Enumerator.ShouldMatch("[42]");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorValue_matches_base_case() {
			var match = EmdGrammar.EnumeratorValue.ShouldMatch("@123");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorValue_matches_0() {
			var match = EmdGrammar.EnumeratorValue.ShouldMatch("@0");

			match.Succeeded.Should().BeTrue();
		}
	}
}
