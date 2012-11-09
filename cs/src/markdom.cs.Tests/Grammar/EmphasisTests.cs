using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar
{
	public class EmphasisTests : GrammarTestFixture {
		[Fact] public void Emphasis_matches_base_case() {
			var expected = new EmphasisNode(
				new IInlineNode[] { new TextNode("text", new MarkdomSourceRange(1, 4, 1, 1)) },
				new MarkdomSourceRange(0, 6, 1, 0)
			);

			var match = Grammar.Emphasis.ShouldMatch("*text*");
			
			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Emphasis_matches_with_missing_end_delimiter() {
			var expected = new EmphasisNode(
				new IInlineNode[] { new TextNode("foo", new MarkdomSourceRange(1, 3, 1, 1)) },
				new MarkdomSourceRange(0, 4, 1, 0)
			);			

			var match = Grammar.Emphasis.ShouldMatch("*foo");

			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
