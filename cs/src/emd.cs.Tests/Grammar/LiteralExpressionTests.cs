using FluentAssertions;
using emd.cs.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class LiteralExpressionTests : GrammarTestFixture {
		[Fact] public void LiteralExpression_matches_null() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("null");
		}

		[Fact] public void LiteralExpression_matches_true() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("true");
		}

		[Fact] public void LiteralExpression_matches_false() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("false");
		}

		[Fact] public void LiteralExpression_matches_integer_literal() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("42");
		}

		[Fact] public void LiteralExpression_matches_floating_point_literal() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("42.123");
		}

		[Fact] public void LiteralExpression_matches_single_quoted_string_literal() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("'foobar'");
		}

		[Fact] public void LiteralExpression_matches_double_quoted_string_literal() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("\"fizzbuzz\"");
		}

		[Fact] public void LiteralExpression_matches_uri_literal() {
			var match = EmdGrammar.LiteralExpression.ShouldMatchAllOf("http://reddit.com");
		}
	}
}
