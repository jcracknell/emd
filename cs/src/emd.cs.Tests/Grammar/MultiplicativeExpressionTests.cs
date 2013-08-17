using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
  public class MultiplicativeExpressionTests : GrammarTestFixture {
    [Fact] public void MultiplicativeExpression_should_match_numeric_literal_multiplication() {
      var match = EmdGrammar.MultiplicativeExpression.ShouldMatchAllOf("42*42");

      match.Product.ShouldBeEquivalentTo(
        new MultiplicationExpression(
          new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
          new NumericLiteralExpression(42d, new SourceRange(3,2,1,3)),
          new SourceRange(0,5,1,0)
        )
      );
    }

    [Fact] public void MultiplicativeExpression_should_match_numeric_literal_multiplication_with_spaces() {
      var match = EmdGrammar.MultiplicativeExpression.ShouldMatchAllOf("42 * 42");

      match.Product.ShouldBeEquivalentTo(
        new MultiplicationExpression(
          new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
          new NumericLiteralExpression(42d, new SourceRange(5,2,1,3)),
          new SourceRange(0,7,1,0)
        )
      );
    }

    [Fact] public void MultiplicativeExpression_should_match_numeric_literal_division() {
      var match = EmdGrammar.MultiplicativeExpression.ShouldMatchAllOf("42/42");

      match.Product.ShouldBeEquivalentTo(
        new DivisionExpression(
          new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
          new NumericLiteralExpression(42d, new SourceRange(3,2,1,3)),
          new SourceRange(0,5,1,0)
        )
      );
    }

    [Fact] public void MultiplicativeExpression_should_match_numeric_literal_division_with_spaces() {
      var match = EmdGrammar.MultiplicativeExpression.ShouldMatchAllOf("42 / 42");

      match.Product.ShouldBeEquivalentTo(
        new DivisionExpression(
          new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
          new NumericLiteralExpression(42d, new SourceRange(5,2,1,3)),
          new SourceRange(0,7,1,0)
        )
      );
    }

    [Fact] public void MultiplicativeExpression_should_match_numeric_literal_modulo() {
      var match = EmdGrammar.MultiplicativeExpression.ShouldMatchAllOf("42%42");

      match.Product.ShouldBeEquivalentTo(
        new ModuloExpression(
          new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
          new NumericLiteralExpression(42d, new SourceRange(3,2,1,3)),
          new SourceRange(0,5,1,0)
        )
      );
    }

    [Fact] public void MultiplicativeExpression_should_match_numeric_literal_modulo_with_spaces() {
      var match = EmdGrammar.MultiplicativeExpression.ShouldMatchAllOf("42 % 42");

      match.Product.ShouldBeEquivalentTo(
        new ModuloExpression(
          new NumericLiteralExpression(42d, new SourceRange(0,2,1,0)),
          new NumericLiteralExpression(42d, new SourceRange(5,2,1,3)),
          new SourceRange(0,7,1,0)
        )
      );
    }
  }
}
