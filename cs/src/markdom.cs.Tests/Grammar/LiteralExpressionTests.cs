using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Expressions;
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
			
			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.NullLiteral);
		}

		[Fact] public void LiteralExpression_matches_true() {
			var match = Grammar.LiteralExpression.ShouldMatch("true");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.BooleanLiteral);
		}

		[Fact] public void LiteralExpression_matches_false() {
			var match = Grammar.LiteralExpression.ShouldMatch("false");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.BooleanLiteral);
		}

		[Fact] public void LiteralExpression_matches_integer_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("42");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.NumericLiteral);
		}

		[Fact] public void LiteralExpression_matches_floating_point_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("42.123");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.NumericLiteral);
		}

		[Fact] public void LiteralExpression_matches_single_quoted_string_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("'foobar'");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.StringLiteral);
		}

		[Fact] public void LiteralExpression_matches_double_quoted_string_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("\"fizzbuzz\"");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.StringLiteral);
		}

		[Fact] public void LiteralExpression_matches_uri_literal() {
			var match = Grammar.LiteralExpression.ShouldMatch("http://reddit.com");

			match.Succeeded.Should().BeTrue();
			match.Product.Kind.Should().Be(ExpressionKind.UriLiteral);
		}
	}
}
