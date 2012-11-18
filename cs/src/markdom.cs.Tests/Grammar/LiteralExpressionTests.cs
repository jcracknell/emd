using FluentAssertions;
using markdom.cs.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class LiteralExpressionTests : GrammarTestFixture {
		[Fact] public void LiteralExpression_matches_null() {
			var match = Grammar.LiteralExpression.ShouldMatch("null");
		}

		[Fact] public void LiteralExpression_matches_true() {
			var match = Grammar.LiteralExpression.ShouldMatch("true");
		}

		[Fact] public void LiteralExpression_matches_false() {
			var match = Grammar.LiteralExpression.ShouldMatch("false");
		}

		[Fact] public void LiteralExpression_matches_integer_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("42");
		}

		[Fact] public void LiteralExpression_matches_floating_point_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("42.123");
		}

		[Fact] public void LiteralExpression_matches_single_quoted_string_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("'foobar'");
		}

		[Fact] public void LiteralExpression_matches_double_quoted_string_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("\"fizzbuzz\"");
		}

		[Fact] public void LiteralExpression_matches_uri_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("http://reddit.com");
		}
	}
}
