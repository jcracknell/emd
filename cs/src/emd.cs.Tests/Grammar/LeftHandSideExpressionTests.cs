using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class LeftHandSideExpressionTests : GrammarTestFixture {
		[Fact] public void LeftHandSideExpression_should_match_static_property() {
			var expected = new StaticPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				"bar",
				new SourceRange(1,7,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch("@foo.bar");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_should_match_static_property_with_superfluous_whitespace() {
			var expected = new StaticPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				"bar",
				new SourceRange(1,9,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch("@foo . bar");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_should_match_multiple_static_properties() {
			var expected = new StaticPropertyExpression(
				new StaticPropertyExpression(
					new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
					"bar",
					new SourceRange(1, 7, 1, 1)
				),
				"baz",
				new SourceRange(1, 11, 1, 1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"@foo.bar.baz");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_should_match_dynamic_property() {
			var expected = new DynamicPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				new StringLiteralExpression("bar", new SourceRange(5,5,1,5)),
				new SourceRange(1,10,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch("@foo['bar']");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_should_match_dynamic_property_with_superfluous_whitespace() {
			var expected = new DynamicPropertyExpression(
				new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
				new StringLiteralExpression("bar", new SourceRange(7,5,1,7)),
				new SourceRange(1,13,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch("@foo [ 'bar' ]");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_should_match_multiple_dynamic_properties() {
			var expected = new DynamicPropertyExpression(
				new DynamicPropertyExpression(
					new IdentifierExpression("foo", new SourceRange(1, 3, 1, 1)),
					new StringLiteralExpression("bar", new SourceRange(5,5,1,5)),
					new SourceRange(1,10,1,1)
				),
				new StringLiteralExpression("baz", new SourceRange(12,5,1,12)),
				new SourceRange(1,17,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"@foo['bar']['baz']");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_matches_identifier_body_with_no_arguments() {
			var expected = new CallExpression(
				new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
				new IExpression[0],
				new SourceRange(1,5,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch("@foo()");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void LeftHandSideExpression_matches_identifier_body_with_arguments() {
			var expected = new CallExpression(
				new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
				new IExpression[] {
					new NumericLiteralExpression(1d, new SourceRange(5,1,1,5)),
					new StringLiteralExpression("2", new SourceRange(7,3,1,7))
				},
				new SourceRange(1,10,1,1)
			);

			var match = EmdGrammar.LeftHandSideExpression.ShouldMatch("@foo(1,'2')");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
