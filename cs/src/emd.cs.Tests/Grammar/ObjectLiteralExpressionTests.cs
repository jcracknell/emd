using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
  public class ObjectLiteralExpressionTests : GrammarTestFixture {
    [Fact] public void ObjectLiteralExpression_matches_empty_object() {
      var expected = new ObjectLiteralExpression(
        new PropertyAssignment[0],
        new SourceRange(0, 2, 1, 0));

      var match = EmdGrammar.ObjectLiteralExpression.ShouldMatchAllOf("{}");
      match.Product.ShouldBeEquivalentTo(expected);
    }

    [Fact] public void ObjectLiteralExpression_matches_object_with_string_propertyname() {
      var expected = new ObjectLiteralExpression(
        new PropertyAssignment[] {
          new PropertyAssignment(
            "a",
            new StringLiteralExpression("foo", new SourceRange(8, 5, 1, 8)),
            new SourceRange(2, 11, 1, 2)) },
        new SourceRange(0, 15, 1, 0));  

      var match = EmdGrammar.ObjectLiteralExpression.ShouldMatchAllOf("{ 'a' : 'foo' }");
      match.Product.ShouldBeEquivalentTo(expected);
    }

    [Fact] public void ObjectLiteralExpression_should_match_numeric_propertyname() {
      var expected = new ObjectLiteralExpression(
        new PropertyAssignment[] {
          new PropertyAssignment(
            "0",
            new StringLiteralExpression("a", new SourceRange(3,3,1,3)),
            new SourceRange(1,5,1,1)
          ),
          new PropertyAssignment(
            "1",
            new StringLiteralExpression("b", new SourceRange(9,3,1,9)),
            new SourceRange(7,5,1,7)
          )
        },
        new SourceRange(0,13,1,0)
      );

      var match = EmdGrammar.ObjectLiteralExpression.ShouldMatchAllOf(
        //0....:....0....:....0....:....0....:....0....:....0....:....0
        @"{0:'a',1:'b'}");

      match.Product.ShouldBeEquivalentTo(expected);
    }

    [Fact] public void ObjectLiteralExpression_should_match_identifier_propertyname() {
      var expected = new ObjectLiteralExpression(
        new PropertyAssignment[] {
          new PropertyAssignment(
            "foo",
            new NumericLiteralExpression(1d, new SourceRange(7,1,1,7)),
            new SourceRange(2,6,1,2)
          ),
          new PropertyAssignment(
            "bar",
            new NumericLiteralExpression(2d, new SourceRange(15,1,1,15)),
            new SourceRange(10,6,1,10)
          ),
        },
        new SourceRange(0,18,1,0)
      );

      var match = EmdGrammar.ObjectLiteralExpression.ShouldMatchAllOf(
        //0....:....0....:....0....:....0....:....0....:....0....:....0
        @"{ foo: 1, bar: 2 }");

      match.Product.ShouldBeEquivalentTo(expected);
    }

    [Fact] public void ObjectLiteralExpression_should_match_object_literal_with_trailing_comma() {
      var match = EmdGrammar.ObjectLiteralExpression.ShouldMatchAllOf("{a: 'a',}");

      match.Product.ShouldBeEquivalentTo(
        new ObjectLiteralExpression(
          new PropertyAssignment[] {
            new PropertyAssignment("a", new StringLiteralExpression("a", new SourceRange(4,3,1,4)), new SourceRange(1,6,1,1))
          },
          new SourceRange(0,9,1,0)
        )
      );
    }
  }
}
