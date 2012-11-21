using FluentAssertions;
using markdom.cs.Expressions;
using markdom.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class LinkTests : GrammarTestFixture {
		[Fact] public void Link_matches_explicit_link() {
			var matchResult = MarkdomGrammar.Link.ShouldMatch("[Slashdot: News for nerds, stuff that matters](http://slashdot.org)");

			matchResult.Succeeded.Should().BeTrue();
			matchResult.Product.Arguments.Should().NotBeEmpty();
		}

		[Fact] public void Link_matches_hybrid_link() {
			var match = MarkdomGrammar.Link.ShouldMatch(
				"[Slashdot: News for nerds, stuff that matters][slashdot.org](http://slashdot.org)");

			match.Succeeded.Should().BeTrue();
			match.Product.ReferenceId.Should().NotBeNull();
			match.Product.ReferenceId.Value.Should().Be("slashdot-org");
			match.Product.Arguments.Should().NotBeEmpty();
		}

		[Fact] public void Link_matches_reference_link() {
			var match = MarkdomGrammar.Link.ShouldMatch(
				"[Slashdot: News for nerds, stuff that matters][slashdot.org]");

			match.Succeeded.Should().BeTrue();
			match.Product.ReferenceId.Should().NotBeNull();
			match.Product.ReferenceId.Value.Should().Be("slashdot-org");
			match.Product.Arguments.Should().BeNull();
		}

		[Fact] public void Link_matches_short_reference_link() {
			var match = MarkdomGrammar.Link.ShouldMatch("[google]");

			match.Product.ShouldBeEquivalentTo(
				new LinkNode(
					new IInlineNode[] { new TextNode("google", new SourceRange(1,6,1,1)) },
					null,
					null,
					new SourceRange(0,8,1,0)
				)
			);
		}

		// This behavior seems intuitive per https://github.com/jgm/peg-markdown/commit/1a629de5e2bc62c308be4250e22bacd19e86f6c1
		[Fact] public void Link_should_match_short_label_space_argument_list_as_reference_only() {
			var match = MarkdomGrammar.Link.ShouldMatch("[foo] (baz)");

			match.Product.ShouldBeEquivalentTo(
				new LinkNode(
					new IInlineNode[] { new TextNode("foo", new SourceRange(1,3,1,1)) },
					null,
					null,
					new SourceRange(0,5,1,0)
				)
			);
		}

		[Fact] public void Link_should_match_label_space_reference_as_label_only() {
			var match = MarkdomGrammar.Link.ShouldMatch("[foo] [baz]");

			match.Product.ShouldBeEquivalentTo(
				new LinkNode(
					new IInlineNode[] { new TextNode("foo", new SourceRange(1,3,1,1)) },
					null,
					null,
					new SourceRange(0,5,1,0)
				)
			);
		}
	}
}
