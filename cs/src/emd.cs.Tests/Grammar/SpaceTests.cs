using FluentAssertions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class SpaceTests : GrammarTestFixture {
		[Fact] public void Space_should_match_space() {
			var match = EmdGrammar.Space.ShouldMatchSomeOf(" block text");

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,1,1,0)));
		}

		[Fact] public void Space_should_match_newline() {
			var match = EmdGrammar.Space.ShouldMatchSomeOf(
				/* block text */"\n",
				"block continuing on next line"
			);

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,1,1,0)));
		}

		[Fact] public void Space_should_match_spaces_and_newlines() {
			var match = EmdGrammar.Space.ShouldMatchSomeOf(
				/* block text */"  \n",
				"   block continues with leading spaces"
			);

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,6,1,0)));
		}

		[Fact] public void Space_should_not_match_nothing() {
			EmdGrammar.Space.ShouldNotMatch("block text");
		}

		[Fact] public void Space_should_not_match_newline_at_end_of_block() {
			EmdGrammar.Space.ShouldNotMatch(
				/* block text */"\n",
				"\n",
				"new block"
			);
		}

		[Fact] public void Space_should_not_match_space_at_end_of_block() {
			EmdGrammar.Space.ShouldNotMatch(
				/* block text */"    \n",
				"\n",
				"new block"
			);
		}

		[Fact] public void Space_should_not_match_newline_at_end_of_input() {
			EmdGrammar.Space.ShouldNotMatch("\n");
		}

		[Fact] public void Space_should_not_match_space_at_end_of_input() {
			EmdGrammar.Space.ShouldNotMatch(" ");
		}

		[Fact] public void Space_should_not_match_space_and_newline_at_end_of_input() {
			EmdGrammar.Space.ShouldNotMatch(" \n");
		}

		[Fact] public void Space_should_match_single_line_comment() {
			var match = EmdGrammar.Space.ShouldMatchSomeOf(
				"// comment\n",
				"block continues"
			);

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,11,1,0)));
		}

		[Fact] public void Space_should_not_match_single_line_comment_at_end_of_block() {
			EmdGrammar.Space.ShouldNotMatch(
				"// comment\n",
				"\n",
				"new block"
			);
		}

		[Fact] public void Space_should_match_multi_line_comment() {
			var match = EmdGrammar.Space.ShouldMatchSomeOf("/* comment */text");

			match.Product.ShouldBeEquivalentTo(new SpaceNode(new SourceRange(0,13,1,0)));
		}

		[Fact] public void Space_should_not_match_multi_line_comment_at_end_of_block() {
			EmdGrammar.Space.ShouldNotMatch(
				"/* comment */\n",
				"\n",
				"new block"
			);
		}
	}
}
