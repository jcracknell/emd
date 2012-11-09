using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class OrderedListTests : GrammarTestFixture {
		[Fact] public void EnumeratorishAhead_matches_decimal_dot() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("42.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dot() {
			var match = Grammar.Enumerator.ShouldMatch("42.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dot_with_value() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("42@43.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dot_with_value() {
			var match = Grammar.Enumerator.ShouldMatch("42@43.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_lower_roman_dot() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("ix.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_lower_roman_dot() {
			var match = Grammar.Enumerator.ShouldMatch("ix.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_upper_roman_dot() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("XVIII.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_upper_roman_dot() {
			var match = Grammar.Enumerator.ShouldMatch("XVIII.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_lower_alpha_dot() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("abc.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_lower_alpha_dot() {
			var match = Grammar.Enumerator.ShouldMatch("abc.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_upper_alpha_dot() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("ABC.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_upper_alpha_dot() {
			var match = Grammar.Enumerator.ShouldMatch("ABC.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("42-");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash_with_optional_space() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("42 -");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dash() {
			var match = Grammar.Enumerator.ShouldMatch("42-");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dash_with_optional_space() {
			var match =Grammar.Enumerator.ShouldMatch("42 -");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_parenthesis() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_parenthesis() {
			var match = Grammar.Enumerator.ShouldMatch("42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_alpha_parethesis_with_value() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("a@42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_alpha_parenthesis_with_value() {
			var match = Grammar.Enumerator.ShouldMatch("a@42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAheah_matches_decimal_parentheses() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("(42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_parentheses() {
			var match = Grammar.Enumerator.ShouldMatch("(42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_bracketed() {
			var match = Grammar.EnumeratorishAhead.ShouldMatch("[42]");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_bracketed() {
			var match = Grammar.Enumerator.ShouldMatch("[42]");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorValue_matches_base_case() {
			var match = Grammar.EnumeratorValue.ShouldMatch("@123");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorValue_matches_0() {
			var match = Grammar.EnumeratorValue.ShouldMatch("@0");

			match.Succeeded.Should().BeTrue();
		}
	}
}
