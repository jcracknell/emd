using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class RelationalExpressionTests {
		[Fact] public void RelationalExpression_should_match_numeric_literal_greater_than_or_equal_to_numeric_literal() {
			var match = EmdGrammar.RelationalExpression.ShouldMatchAllOf("42 >= 7");

			match.Product.GetType().Should().Be(typeof(GreaterThanOrEqualToExpression));
			match.Product.ShouldBeEquivalentTo(
				new GreaterThanOrEqualToExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(7d, new SourceRange(6,1,1,6)),
					new SourceRange(0,7,1,0)
				)
			);
		}

		[Fact] public void RelationalExpression_should_match_numeric_literal_less_than_or_equal_to_numeric_literal() {
			var match = EmdGrammar.RelationalExpression.ShouldMatchAllOf("42 <= 7");

			match.Product.GetType().Should().Be(typeof(LessThanOrEqualToExpression));
			match.Product.ShouldBeEquivalentTo(
				new LessThanOrEqualToExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(7d, new SourceRange(6,1,1,6)),
					new SourceRange(0,7,1,0)
				)
			);
		}

		[Fact] public void RelationalExpression_should_match_numeric_literal_greater_than_numeric_literal() {
			var match = EmdGrammar.RelationalExpression.ShouldMatchAllOf("42>7");

			match.Product.GetType().Should().Be(typeof(GreaterThanExpression));
			match.Product.ShouldBeEquivalentTo(
				new GreaterThanExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(7d, new SourceRange(3,1,1,3)),
					new SourceRange(0,4,1,0)
				)
			);
		}

		[Fact] public void RelationalExpression_should_match_numeric_literal_less_than_numeric_literal() {
			var match = EmdGrammar.RelationalExpression.ShouldMatchAllOf("42<7");

			match.Product.GetType().Should().Be(typeof(LessThanExpression));
			match.Product.ShouldBeEquivalentTo(
				new LessThanExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(7d, new SourceRange(3,1,1,3)),
					new SourceRange(0,4,1,0)
				)
			);
		}

		[Fact] public void RelationalExpression_should_match_identifier_instanceof_identifier() {
			var match = EmdGrammar.RelationalExpression.ShouldMatchAllOf(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"@foo instanceof @SomePrototype");

			match.Product.GetType().Should().Be(typeof(InstanceOfExpression));
			match.Product.ShouldBeEquivalentTo(
				new InstanceOfExpression(
					new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
					new IdentifierExpression("SomePrototype", new SourceRange(17,13,1,17)),
					new SourceRange(1,29,1,1)
				)
			);
		}

		[Fact] public void RelationalExpression_should_match_string_literal_in_object_literal() {
			var match = EmdGrammar.RelationalExpression.ShouldMatchAllOf(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"'foo' in { foo: true, bar: false }");

			match.Product.GetType().Should().Be(typeof(InExpression));
			match.Product.ShouldBeEquivalentTo(
				new InExpression(
					new StringLiteralExpression("foo", new SourceRange(0,5,1,0)),
					new ObjectLiteralExpression(
						new PropertyAssignment[] {
							new PropertyAssignment(
								"foo",
								new BooleanLiteralExpression(true, new SourceRange(16,4,1,16)),
								new SourceRange(11,9,1,11)
							),
							new PropertyAssignment(
								"bar",
								new BooleanLiteralExpression(false, new SourceRange(27,5,1,27)),
								new SourceRange(22,10,1,22)
							)
						},
						new SourceRange(9,25,1,9)
					),
					new SourceRange(0,34,1,0)
				)
			);
		}
	}
}
