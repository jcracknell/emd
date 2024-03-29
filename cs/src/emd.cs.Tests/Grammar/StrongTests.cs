﻿using FluentAssertions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
  public class StrongTests : GrammarTestFixture {
    [Fact] public void Strong_matches_base_case() {
      var expected = new StrongNode(
        new IInlineNode[] { new TextNode("text", new SourceRange(2, 4, 1, 2)) },
        new SourceRange(0, 8, 1, 0));

      var match = EmdGrammar.RichStrong.ShouldMatchAllOf("**text**");

      match.Product.ShouldBeEquivalentTo(expected);
    }

    [Fact] public void Strong_should_not_match_with_missing_end_delimiter() {
      EmdGrammar.RichStrong.ShouldNotMatch("**text");
    }

    [Fact] public void Strong_should_not_match_when_empty() {
      EmdGrammar.RichStrong.ShouldNotMatch("****");
    }
  }
}
