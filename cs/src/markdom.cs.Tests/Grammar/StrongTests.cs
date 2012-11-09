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
	public class StrongTests : GrammarTestFixture {
		[Fact] public void Strong_matches_base_case() {
			var expected = new StrongNode(
				new IInlineNode[] { new TextNode("text", new MarkdomSourceRange(2, 4, 1, 2)) },
				new MarkdomSourceRange(0, 8, 1, 0));

			var match = Grammar.Strong.ShouldMatch("**text**");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
