using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class QuotedTests : GrammarTestFixture {
		[Fact] public void Quoted_matches_single_quoted() {
			var expected = new QuotedNode(
				QuoteType.Single,
				new IInlineNode[] { new TextNode("text", new MarkdomSourceRange(1, 4, 1, 1)) },
				new MarkdomSourceRange(0, 6, 1, 0));

			var match = Grammar.Quoted.ShouldMatch("'text'");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
