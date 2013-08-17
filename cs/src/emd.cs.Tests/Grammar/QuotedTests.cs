using FluentAssertions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
  public class QuotedTests : GrammarTestFixture {
    [Fact] public void Quoted_matches_single_quoted() {
      var expected = new QuotedNode(
        QuoteType.Single,
        new IInlineNode[] { new TextNode("text", new SourceRange(1, 4, 1, 1)) },
        new SourceRange(0, 6, 1, 0));

      var match = EmdGrammar.Quoted.ShouldMatchAllOf("'text'");

      match.Product.ShouldBeEquivalentTo(expected);
    }
  }
}
