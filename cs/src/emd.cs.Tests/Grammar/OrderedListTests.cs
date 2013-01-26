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
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("42. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_decimal_dot_with_no_trailing_space() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("42.abc");
		}

		[Fact] public void Enumerator_matches_decimal_dot() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("42. ");
		}

		[Fact] public void Enumerator_should_not_match_decimal_dot_with_no_trailing_space() {
			EmdGrammar.Enumerator.ShouldNotMatch("42.abc");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dot_with_value() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("42@43. ");
		}

		[Fact] public void Enumerator_matches_decimal_dot_with_value() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("42@43. ");
		}

		[Fact] public void EnumeratorishAhead_matches_lower_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("ix. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_lower_roman_upper_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("iX. ");
		}

		[Fact] public void Enumerator_matches_lower_roman_dot() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("ix. ");
		}

		[Fact] public void Enumerator_should_not_match_lower_roman_upper_roman_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("iX. ");
		}

		[Fact] public void EnumeratorishAhead_matches_upper_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("XVIII. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_upper_roman_lower_roman_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("Xviii. ");
		}

		[Fact] public void Enumerator_matches_upper_roman_dot() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("XVIII. ");
		}

		[Fact] public void Enumerator_should_not_match_upper_roman_lower_roman_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("Xv. ");
		}

		[Fact] public void EnumeratorishAhead_matches_lower_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("abc. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_lower_alpha_upper_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("aBC. ");
		}

		[Fact] public void Enumerator_matches_lower_alpha_dot() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("abc. ");
		}

		[Fact] public void Enumerator_should_not_match_lower_alpha_upper_alpha_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("aB. ");
		}

		[Fact] public void EnumeratorishAhead_matches_upper_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("ABC. ");
		}

		[Fact] public void EnumeratorishAhead_should_not_match_upper_alpha_lower_alpha_dot() {
			EmdGrammar.EnumeratorishAhead.ShouldNotMatch("Ab. ");
		}

		[Fact] public void Enumerator_matches_upper_alpha_dot() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("ABC. ");
		}

		[Fact] public void Enumerator_should_not_match_upper_alpha_lower_alpha_dot() {
			EmdGrammar.Enumerator.ShouldNotMatch("Ab. ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("42- ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash_with_optional_space() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("42 - ");
		}

		[Fact] public void Enumerator_matches_decimal_dash() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("42- ");
		}

		[Fact] public void Enumerator_matches_decimal_dash_with_optional_space() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("42 - ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_parenthesis() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("42) ");
		}

		[Fact] public void Enumerator_matches_decimal_parenthesis() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("42) ");
		}

		[Fact] public void EnumeratorishAhead_matches_alpha_parethesis_with_value() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("a@42) ");
		}

		[Fact] public void Enumerator_matches_alpha_parenthesis_with_value() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("a@42) ");
		}

		[Fact] public void EnumeratorishAheah_matches_decimal_parentheses() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("(42) ");
		}

		[Fact] public void Enumerator_matches_decimal_parentheses() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("(42) ");
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_bracketed() {
			EmdGrammar.EnumeratorishAhead.ShouldMatchAllOf("[42] ");
		}

		[Fact] public void Enumerator_matches_decimal_bracketed() {
			EmdGrammar.Enumerator.ShouldMatchAllOf("[42] ");
		}

		[Fact] public void EnumeratorValue_matches_base_case() {
			EmdGrammar.EnumeratorValue.ShouldMatchAllOf("@123");
		}

		[Fact] public void EnumeratorValue_matches_0() {
			EmdGrammar.EnumeratorValue.ShouldMatchAllOf("@0");
		}
	}
}
