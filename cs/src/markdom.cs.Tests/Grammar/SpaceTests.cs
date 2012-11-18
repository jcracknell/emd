using FluentAssertions;
using markdom.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class SpaceTests : GrammarTestFixture {
		[Fact] public void Space_should_match_space() {
			var match = Grammar.Space.ShouldMatch(" block text");

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,1,1,0)));
		}

		[Fact] public void Space_should_match_newline() {
			var match = Grammar.Space.ShouldMatch(
				/* block text */"\n",
				"block continuing on next line"
			);

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,1,1,0)));
		}

		[Fact] public void Space_should_match_spaces_and_newlines() {
			var match = Grammar.Space.ShouldMatch(
				/* block text */"  \n",
				"   block continues with leading spaces"
			);

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,6,1,0)));
		}

		[Fact] public void Space_should_not_match_nothing() {
			Grammar.Space.ShouldNotMatch("block text");
		}

		[Fact] public void Space_should_not_match_newline_at_end_of_block() {
			Grammar.Space.ShouldNotMatch(
				/* block text */"\n",
				"\n",
				"new block"
			);
		}

		[Fact] public void Space_should_not_match_space_at_end_of_block() {
			Grammar.Space.ShouldNotMatch(
				/* block text */"    \n",
				"\n",
				"new block"
			);
		}

		[Fact] public void Space_should_not_match_newline_at_end_of_input() {
			Grammar.Space.ShouldNotMatch("\n");
		}

		[Fact] public void Space_should_not_match_space_at_end_of_input() {
			Grammar.Space.ShouldNotMatch(" ");
		}

		[Fact] public void Space_should_not_match_space_and_newline_at_end_of_input() {
			Grammar.Space.ShouldNotMatch(" \n");
		}
	}
}
