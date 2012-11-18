using FluentAssertions;
using markdom.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class LineBreakTests : GrammarTestFixture {
		[Fact] public void LineBreak_should_match_symbol_followed_by_block_text() {
			var match = Grammar.LineBreak.ShouldMatch(
				/* block text */"\\\n",
				"block text"
			);

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,2,1,0)));
		}

		[Fact] public void LineBreak_should_match_at_end_of_input() {
			var match = Grammar.LineBreak.ShouldMatch("\\");

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,1,1,0)));
		}

		[Fact] public void LineBreak_should_match_no_following_content() {
			var match = Grammar.LineBreak.ShouldMatch("\\\n");

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,1,1,0)));
		}

		[Fact] public void LineBreak_should_match_spaces_on_following_line() {
			var match = Grammar.LineBreak.ShouldMatch(
				/* block text */"\\\n",
				"   block text"
			);

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,5,1,0)));
		}

		[Fact] public void LineBreak_should_match_with_spaces_preceding_newline() {
			var match = Grammar.LineBreak.ShouldMatch(
				/* block text */"\\  \n",
				"block text"
			);

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,4,1,0)));
		}

		[Fact] public void LineBreak_should_match_on_separate_line() {
			// This is an odd case, however this behavior makes sense as it is consistent with our block
			// definition, and it would be very difficult to prevent (as it would be a combination of
			// linebreaks and spaces)

			var match = Grammar.LineBreak.ShouldMatch(
				/* block text */"\n",
				"\\\n",
				"block text"
			);

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,3,1,0)));
		}

		[Fact] public void LineBreak_should_match_following_single_line_comment() {
			var match = Grammar.LineBreak.ShouldMatch(
				"// comment\n",
				"\\\n"
			);

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,12,1,0)));
		}

		[Fact] public void LineBreak_should_match_following_multiple_single_line_comments() {
			var match = Grammar.LineBreak.ShouldMatch(
				"// 1\n",
				"// 2\n",
				"\\\n"
			);

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,11,1,0)));
		}

		[Fact] public void LineBreak_should_match_following_multi_line_comment() {
			var match = Grammar.LineBreak.ShouldMatch("/* comment */\\\n");

			match.Product.ShouldBeEquivalentTo(new LineBreakNode(new SourceRange(0,14,1,0)));
		}

		[Fact] public void LineBreak_should_not_match_with_block_text_on_same_line() {
			Grammar.LineBreak.ShouldNotMatch("\\  block text");
		}
	}
}
