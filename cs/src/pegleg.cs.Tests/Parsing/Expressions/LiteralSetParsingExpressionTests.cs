using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Parsing.Expressions {
  public class LiteralSetParsingExpressionTests {
    [Fact] public void NonCapturingLiteralSetParsingExpression_Matches_should_be_greedy() {
      var parsingExpression = new NonCapturingLiteralSetParsingExpression(new string[] { "in", "interface" });

      var match = parsingExpression.ShouldMatchAllOf("interface");
      match.Length.Should().Be("interface".Length);
    }
  }
}
