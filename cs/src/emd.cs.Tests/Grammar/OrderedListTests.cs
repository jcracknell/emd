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
			EmdGrammar.EnumeratorishAhead.ShouldMatch("42. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_decimal_dot_with_no_trailing_space() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("42.abc");
		}

		[Fact] public void Enumerator_matches_decimal_dot() {
			EmdGrammar.Enumerator.ShouldMatch("42. ");
		}

		[Fact] public void Enumerator_should_not_match_decimal_dot_with_no_trailing_space() {
			EmdGrammar.Enumerator.ShouldNotMatch("42.abc");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dot_with_value() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("42@43. ");
		}

		[Fact] public void Enumerator_matches_decimal_dot_with_value() {
			EmdGrammar.Enumerator.ShouldMatch("42@43. ");
		}

		[Fact] public void EnumeratorishAhead_matches_lower_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("ix. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_lower_roman_upper_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("iX. ");
		}

		[Fact] public void Enumerator_matches_lower_roman_dot() {
			EmdGrammar.Enumerator.ShouldMatch("ix. ");
		}

		[Fact] public void Enumerator_should_not_match_lower_roman_upper_roman_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("iX. ");
		}

		[Fact] public void EnumeratorishAhead_matches_upper_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("XVIII. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_upper_roman_lower_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("Xviii. ");
		}

		[Fact] public void Enumerator_matches_upper_roman_dot() {
			EmdGrammar.Enumerator.ShouldMatch("XVIII. ");
		}

		[Fact] public void Enumerator_should_not_match_upper_roman_lower_roman_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("Xv. ");
		}

		[Fact] public void EnumeratorishAhead_matches_lower_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("abc. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_lower_alpha_upper_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("aBC. ");
		}

		[Fact] public void Enumerator_matches_lower_alpha_dot() {
			EmdGrammar.Enumerator.ShouldMatch("abc. ");
		}

		[Fact] public void Enumerator_should_not_match_lower_alpha_upper_alpha_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("aB. ");
		}

		[Fact] public void EnumeratorishAhead_matches_upper_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("ABC. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_upper_alpha_lower_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("Ab. ");
		}

		[Fact] public void Enumerator_matches_upper_alpha_dot() {
			EmdGrammar.Enumerator.ShouldMatch("ABC. ");
		}

		[Fact] public void Enumerator_should_not_match_upper_alpha_lower_alpha_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("Ab. ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("42- ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash_with_optional_space() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("42 - ");
		}

		[Fact] public void Enumerator_matches_decimal_dash() {
			EmdGrammar.Enumerator.ShouldMatch("42- ");
		}

		[Fact] public void Enumerator_matches_decimal_dash_with_optional_space() {
			EmdGrammar.Enumerator.ShouldMatch("42 - ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_parenthesis() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("42) ");
		}

		[Fact] public void Enumerator_matches_decimal_parenthesis() {
			EmdGrammar.Enumerator.ShouldMatch("42) ");
		}

		[Fact] public void EnumeratorishAhead_matches_alpha_parethesis_with_value() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("a@42) ");
		}

		[Fact] public void Enumerator_matches_alpha_parenthesis_with_value() {
			EmdGrammar.Enumerator.ShouldMatch("a@42) ");
		}

		[Fact] public void EnumeratorishAheah_matches_decimal_parentheses() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("(42) ");
		}

		[Fact] public void Enumerator_matches_decimal_parentheses() {
			EmdGrammar.Enumerator.ShouldMatch("(42) ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_bracketed() {
			EmdGrammar.EnumeratorishAhead.ShouldMatch("[42] ");
		}

		[Fact] public void Enumerator_matches_decimal_bracketed() {
			EmdGrammar.Enumerator.ShouldMatch("[42] ");
		}

		[Fact] public void EnumeratorValue_matches_base_case() {
			EmdGrammar.EnumeratorValue.ShouldMatch("@123");
		}

		[Fact] public void EnumeratorValue_matches_0() {
			EmdGrammar.EnumeratorValue.ShouldMatch("@0");
		}
	}
}
