using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
  public class UnaryExpressionTests : GrammarTestFixture {
    [Fact] public void UnaryExpression_should_match_delete_identifier() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("delete @foo");

      match.Product.ShouldBeEquivalentTo(
        new DeleteExpression(
          new IdentifierExpression("foo", new SourceRange(8,3,1,8)),
          new SourceRange(0,11,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_void_call() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("void @foo()");

      match.Product.ShouldBeEquivalentTo(
        new DeleteExpression(
          new CallExpression(
            new IdentifierExpression("foo", new SourceRange(6,3,1,6)),
            new IExpression[] { },
            new SourceRange(6,5,1,6)
          ),
          new SourceRange(0,11,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_typeof_string_literal() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("typeof 'foo'");

      match.Product.ShouldBeEquivalentTo(
        new TypeofExpression(
          new StringLiteralExpression("foo", new SourceRange(8,5,1,8)),
          new SourceRange(0,12,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_prefix_increment_identifier() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("++@foo");

      match.Product.ShouldBeEquivalentTo(
        new PrefixIncrementExpression(
          new IdentifierExpression("foo", new SourceRange(3,3,1,3)),
          new SourceRange(0,6,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_prefix_decrement_dynamic_member() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("--@foo['bar']");

      match.Product.ShouldBeEquivalentTo(
        new PrefixDecrementExpression(
          new DynamicPropertyExpression(
            new IdentifierExpression("foo", new SourceRange(3,3,1,3)),
            new StringLiteralExpression("foo", new SourceRange(7,5,1,7)),
            new SourceRange(2,11,1,2)
          ),
          new SourceRange(0,13,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_positive_numeric_literal() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("+42");

      match.Product.ShouldBeEquivalentTo(
        new PositiveExpression(
          new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)),
          new SourceRange(0,3,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_negative_numeric_literal() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("-42");

      match.Product.ShouldBeEquivalentTo(
        new NegativeExpression(
          new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)),
          new SourceRange(0,3,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_bitwise_not_numeric_literal() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("~42");

      match.Product.ShouldBeEquivalentTo(
        new BitwiseNotExpression(
          new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)),
          new SourceRange(0,3,1,0)
        )
      );
    }

    [Fact] public void UnaryExpression_should_match_logical_not_of_numeric_literal() {
      var match = EmdGrammar.UnaryExpression.ShouldMatchAllOf("!42");

      match.Product.ShouldBeEquivalentTo(
        new LogicalNotExpression(
          new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)),
          new SourceRange(0,3,1,0)
        )
      );
    }
  }
}
