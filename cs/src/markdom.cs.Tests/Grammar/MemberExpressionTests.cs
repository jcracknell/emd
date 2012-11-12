using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class MemberExpressionTests : GrammarTestFixture {
		[Fact] public void MemberExpression_should_match_static_property() {
			var expected = new StaticPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				"bar",
				new SourceRange(1,7,1,1)
			);

			var match = Grammar.MemberExpression.ShouldMatch("@foo.bar");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void MemberExpression_should_match_static_property_with_superfluous_whitespace() {
			var expected = new StaticPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				"bar",
				new SourceRange(1,9,1,1)
			);

			var match = Grammar.MemberExpression.ShouldMatch("@foo . bar");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void MemberExpression_should_match_multiple_static_properties() {
			var expected = new StaticPropertyExpression(
				new StaticPropertyExpression(
					new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
					"bar",
					new SourceRange(1, 7, 1, 1)
				),
				"baz",
				new SourceRange(1, 11, 1, 1)
			);

			var match = Grammar.MemberExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"@foo.bar.baz");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void MemberExpression_should_match_dynamic_property() {
			var expected = new DynamicPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				new StringLiteralExpression("bar", new SourceRange(5,5,1,5)),
				new SourceRange(1,10,1,1)
			);

			var match = Grammar.MemberExpression.ShouldMatch("@foo['bar']");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void MemberExpression_should_match_dynamic_property_with_superfluous_whitespace() {
			var expected = new DynamicPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				new StringLiteralExpression("bar", new SourceRange(7,5,1,7)),
				new SourceRange(1,13,1,1)
			);

			var match = Grammar.MemberExpression.ShouldMatch("@foo [ 'bar' ]");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void MemberExpression_should_match_multiple_dynamic_properties() {
			var expected = new DynamicPropertyExpression(
				new DynamicPropertyExpression(
					new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
					new StringLiteralExpression("bar", new SourceRange(5,5,1,5)),
					new SourceRange(1,10,1,1)
				),
				new StringLiteralExpression("baz", new SourceRange(12,5,1,12)),
				new SourceRange(1,17,1,1)
			);

			var match = Grammar.MemberExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"@foo['bar']['baz']");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
