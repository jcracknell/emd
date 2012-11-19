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
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("42.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dot() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("42.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dot_with_value() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("42@43.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dot_with_value() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("42@43.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_lower_roman_dot() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("ix.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_lower_roman_dot() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("ix.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_upper_roman_dot() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("XVIII.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_upper_roman_dot() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("XVIII.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_lower_alpha_dot() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("abc.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_lower_alpha_dot() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("abc.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_upper_alpha_dot() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("ABC.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_upper_alpha_dot() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("ABC.");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("42-");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_dash_with_optional_space() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("42 -");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dash() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("42-");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_dash_with_optional_space() {
			var match =MarkdomGrammar.Enumerator.ShouldMatch("42 -");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_parenthesis() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_parenthesis() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_alpha_parethesis_with_value() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("a@42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_alpha_parenthesis_with_value() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("a@42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAheah_matches_decimal_parentheses() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("(42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_parentheses() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("(42)");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorishAhead_matches_decimal_bracketed() {
			var match = MarkdomGrammar.EnumeratorishAhead.ShouldMatch("[42]");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void Enumerator_matches_decimal_bracketed() {
			var match = MarkdomGrammar.Enumerator.ShouldMatch("[42]");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorValue_matches_base_case() {
			var match = MarkdomGrammar.EnumeratorValue.ShouldMatch("@123");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void EnumeratorValue_matches_0() {
			var match = MarkdomGrammar.EnumeratorValue.ShouldMatch("@0");

			match.Succeeded.Should().BeTrue();
		}
	}
}
