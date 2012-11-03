using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	[TestClass]
	public class OrderedListTests : GrammarTestFixture {
		[TestMethod]
		public void EnumeratorishAhead_matches_decimal_dot() {
			Grammar.EnumeratorishAhead.AssertMatch("42.");
		}

		[TestMethod]
		public void Enumerator_matches_decimal_dot() {
			Grammar.Enumerator.AssertMatch("42.");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_decimal_dot_with_value() {
			Grammar.EnumeratorishAhead.AssertMatch("42@43.");
		}

		[TestMethod]
		public void Enumerator_matches_decimal_dot_with_value() {
			Grammar.Enumerator.AssertMatch("42@43.");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_lower_roman_dot() {
			Grammar.EnumeratorishAhead.AssertMatch("ix.");
		}

		[TestMethod]
		public void Enumerator_matches_lower_roman_dot() {
			Grammar.Enumerator.AssertMatch("ix.");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_upper_roman_dot() {
			Grammar.EnumeratorishAhead.AssertMatch("XVIII.");
		}

		[TestMethod]
		public void Enumerator_matches_upper_roman_dot() {
			Grammar.Enumerator.AssertMatch("XVIII.");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_lower_alpha_dot() {
			Grammar.EnumeratorishAhead.AssertMatch("abc.");
		}

		[TestMethod]
		public void Enumerator_matches_lower_alpha_dot() {
			Grammar.Enumerator.AssertMatch("abc.");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_upper_alpha_dot() {
			Grammar.EnumeratorishAhead.AssertMatch("ABC.");
		}

		[TestMethod]
		public void Enumerator_matches_upper_alpha_dot() {
			Grammar.Enumerator.AssertMatch("ABC.");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_decimal_dash() {
			Grammar.EnumeratorishAhead.AssertMatch("42-");
			Grammar.EnumeratorishAhead.AssertMatch("42 -");
		}

		[TestMethod]
		public void Enumerator_matches_decimal_dash() {
			Grammar.Enumerator.AssertMatch("42-");
			Grammar.Enumerator.AssertMatch("42 -");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_decimal_parenthesis() {
			Grammar.EnumeratorishAhead.AssertMatch("42)");
		}

		[TestMethod]
		public void Enumerator_matches_decimal_parenthesis() {
			Grammar.Enumerator.AssertMatch("42)");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_alpha_parethesis_with_value() {
			Grammar.EnumeratorishAhead.AssertMatch("a@42)");
		}

		[TestMethod]
		public void Enumerator_matches_alpha_parenthesis_with_value() {
			Grammar.Enumerator.AssertMatch("a@42)");
		}

		[TestMethod]
		public void EnumeratorishAheah_matches_decimal_parentheses() {
			Grammar.EnumeratorishAhead.AssertMatch("(42)");
		}

		[TestMethod]
		public void Enumerator_matches_decimal_parentheses() {
			Grammar.Enumerator.AssertMatch("(42)");
		}

		[TestMethod]
		public void EnumeratorishAhead_matches_decimal_bracketed() {
			Grammar.EnumeratorishAhead.AssertMatch("[42]");
		}

		[TestMethod]
		public void Enumerator_matches_decimal_bracketed() {
			Grammar.Enumerator.AssertMatch("[42]");
		}
	}
}
