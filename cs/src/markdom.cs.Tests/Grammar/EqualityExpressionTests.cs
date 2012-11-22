using FluentAssertions;
using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class EqualityExpressionTests {
		[Fact] public void EqualityExpression_should_match_relational_expression_equals_identifier() {
			var match = MarkdomGrammar.EqualityExpression.ShouldMatch("42 >= @a == @foo");

			match.Product.GetType().Should().Be(typeof(EqualsExpression));
			match.Product.ShouldBeEquivalentTo(
				new EqualsExpression(
					new GreaterThanOrEqualToExpression(
						new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
						new IdentifierExpression("a", new SourceRange(8,1,1,8)),
						new SourceRange(0,8,1,0)
					),
					new IdentifierExpression("foo", new SourceRange(13,3,1,13)),
					new SourceRange(0,16,1,0)
				)
			);
		}

		[Fact] public void EqualityExpression_should_match_numeric_literal_not_equals_identifier() {
			var match = MarkdomGrammar.EqualityExpression.ShouldMatch("0 != @bar");

			match.Product.GetType().Should().Be(typeof(NotEqualsExpression));
			match.Product.ShouldBeEquivalentTo(
				new NotEqualsExpression(
					new NumericLiteralExpression(0d, new SourceRange(0,1,1,0)),
					new IdentifierExpression("bar", new SourceRange(6,3,1,6)),
					new SourceRange(0,9,1,0)
				)
			);
		}

		[Fact] public void EqualityExpression_should_match_boolean_literal_strict_equals_identifier() {
			var match = MarkdomGrammar.EqualityExpression.ShouldMatch("true === @foo");

			match.Product.GetType().Should().Be(typeof(StrictEqualsExpression));
			match.Product.ShouldBeEquivalentTo(
				new StrictEqualsExpression(
					new BooleanLiteralExpression(true, new SourceRange(0,4,1,0)),
					new IdentifierExpression("foo", new SourceRange(11,3,1,11)),
					new SourceRange(0,13,1,0)
				)
			);
		}

		[Fact] public void EqualityExpression_should_match_null_literal_strict_not_equals_bla() {
			var match = MarkdomGrammar.EqualityExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"null !== @foo.bar()");

			match.Product.GetType().Should().Be(typeof(StrictNotEqualsExpression));
			match.Product.ShouldBeEquivalentTo(
				new StrictNotEqualsExpression(
					new NullLiteralExpression(new SourceRange(0,4,1,0)),
					new CallExpression(
						new StaticPropertyExpression(
							new IdentifierExpression("foo", new SourceRange(10,3,1,10)),
							"bar",
							new SourceRange(10,7,1,10)
						),
						new IExpression[0],
						new SourceRange(10,9,1,10)
					),
					new SourceRange(0,19,1,0)
				)
			);
		}
	}
}
