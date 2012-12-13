using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class PostfixExpressionTests : GrammarTestFixture {
		[Fact] public void PostfixExpression_should_match_identifier_increment() {
			var match = MarkdomGrammar.PostfixExpression.ShouldMatch("@a++");

			match.Product.ShouldBeEquivalentTo(
				new PostfixIncrementExpression(
					new IdentifierExpression("a", new SourceRange(1,1,1,1)),
					new SourceRange(1,3,1,1)
				)
			);
		}

		[Fact] public void PostfixExperssion_should_match_member_expression_increment_separated_by_space() {
			var match = MarkdomGrammar.PostfixExpression.ShouldMatch("@foo.bar ++");

			match.Product.ShouldBeEquivalentTo(
				new PostfixIncrementExpression(
					new StaticPropertyExpression(
						new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
						"bar",
						new SourceRange(1,7,1,1)
					),
					new SourceRange(1,10,1,1)
				)
			);
		}

		[Fact] public void PostfixExpression_should_match_identifier_decrement() {
			var match = MarkdomGrammar.PostfixExpression.ShouldMatch("@foo--");

			match.Product.ShouldBeEquivalentTo(
				new PostfixDecrementExpression(
					new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
					new SourceRange(1,5,1,1)
				)
			);
		}

		[Fact] public void PostfixExpression_should_match_string_literal_decrement_separated_by_space() {
			var match = MarkdomGrammar.PostfixExpression.ShouldMatch("'foo' --");

			match.Product.ShouldBeEquivalentTo(
				new PostfixDecrementExpression(
					new StringLiteralExpression("foo", new SourceRange(0,5,1,0)),
					new SourceRange(0,8,1,0)
				)
			);
		}
	}
}
