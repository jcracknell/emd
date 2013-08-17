using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pegleg.cs.Parsing;
using Xunit;
using emd.cs.Expressions;

namespace emd.cs.Grammar {
  public class ConditionalExpressionTests {
    [Fact] public void ConditionalExpression_should_match_identifier_numeric_literal_numeric_literal() {
      var match = EmdGrammar.ConditionalExpression.ShouldMatchAllOf("@foo ? 1 : 2");

      match.Product.GetType().Should().Be(typeof(ConditionalExpression));
      match.Product.ShouldBeEquivalentTo(
        new ConditionalExpression(
          new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
          new NumericLiteralExpression(1d, new SourceRange(7,1,1,7)),
          new NumericLiteralExpression(2d, new SourceRange(11,1,1,11)),
          new SourceRange(0,12,1,0)
        )
      );
    }

    [Fact] public void ConditionalExpression_should_match_nested_conditional_expression() {
      var match = EmdGrammar.ConditionalExpression.ShouldMatchAllOf(
        "@foo ? 'bar' :\n",
        "@fiz ? 'buz' :\n",
        "'bop'"
      );

      match.Product.GetType().Should().Be(typeof(ConditionalExpression));
      match.Product.ShouldBeEquivalentTo(
        new ConditionalExpression(
          new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
          new StringLiteralExpression("bar", new SourceRange(7,5,1,7)),
          new ConditionalExpression(
            new IdentifierExpression("fiz", new SourceRange(16,3,2,1)),
            new StringLiteralExpression("buz", new SourceRange(22,5,2,7)),
            new StringLiteralExpression("bop", new SourceRange(30,5,3,0)),
            new SourceRange(15,20,2,0)
          ),
          new SourceRange(0,35,1,0)
        )
      );
    }
  }
}
