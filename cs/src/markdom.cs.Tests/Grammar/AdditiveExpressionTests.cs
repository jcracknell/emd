using FluentAssertions;
using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class AdditiveExpressionTests : GrammarTestFixture {
		[Fact] public void AdditiveExpression_should_match_numeric_literal_addition() {
			var match = Grammar.AdditiveExpression.ShouldMatch("42+42");

			match.Product.ShouldBeEquivalentTo(
				new AdditionExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(42d, new SourceRange(3,2,1,3)),
					new SourceRange(0,5,1,0)
				)
			);
		}

		[Fact] public void AdditiveExpression_should_match_numeric_literal_addition_with_spaces() {
			var match = Grammar.AdditiveExpression.ShouldMatch("42 + 42");

			match.Product.ShouldBeEquivalentTo(
				new AdditionExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(42d, new SourceRange(5,2,1,5)),
					new SourceRange(0,7,1,0)
				)
			);
		}

		[Fact] public void AdditiveExpression_should_match_numeric_literal_subtraction() {
			var match = Grammar.AdditiveExpression.ShouldMatch("42-42");

			match.Product.ShouldBeEquivalentTo(
				new SubtractionExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(42d, new SourceRange(3,2,1,3)),
					new SourceRange(0,5,1,0)
				)
			);
		}

		[Fact] public void AdditiveExpression_should_match_numeric_literal_subtraction_with_spaces() {
			var match = Grammar.AdditiveExpression.ShouldMatch("42 - 42");

			match.Product.ShouldBeEquivalentTo(
				new SubtractionExpression(
					new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
					new NumericLiteralExpression(42d, new SourceRange(5,2,1,5)),
					new SourceRange(0,7,1,0)
				)
			);
		}
	}
}
