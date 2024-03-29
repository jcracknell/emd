﻿using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
  public class LogicalOrExpressionTests {
    [Fact] public void LogicalOrExpression_should_match_static_member_or_string_literal() {
      var match = EmdGrammar.LogicalOrExpression.ShouldMatchAllOf("@foo.bar || 'baz'");

      match.Product.GetType().Should().Be(typeof(LogicalOrExpression));
      match.Product.ShouldBeEquivalentTo(
        new LogicalOrExpression(
          new StaticPropertyExpression(
            new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
            "bar",
            new SourceRange(1,7,1,1)
          ),
          new StringLiteralExpression("baz", new SourceRange(12,5,1,12)),
          new SourceRange(1,16,1,1)
        )
      );
    }
  }
}
