using FluentAssertions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar
{
	public class EmphasisTests : GrammarTestFixture {
		[Fact] public void Emphasis_matches_base_case() {
			var expected = new EmphasisNode(
				new IInlineNode[] { new TextNode("text", new SourceRange(1, 4, 1, 1)) },
				new SourceRange(0, 6, 1, 0)
			);

			var match = MarkdomGrammar.Emphasis.ShouldMatch("*text*");
			
			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Emphasis_matches_with_missing_end_delimiter() {
			var expected = new EmphasisNode(
				new IInlineNode[] { new TextNode("foo", new SourceRange(1, 3, 1, 1)) },
				new SourceRange(0, 4, 1, 0)
			);			

			var match = MarkdomGrammar.Emphasis.ShouldMatch("*foo");

			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
