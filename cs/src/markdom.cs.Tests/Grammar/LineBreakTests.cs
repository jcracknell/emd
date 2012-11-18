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

		[Fact] public void LineBreak_should_not_match_with_block_text_on_same_line() {
			Grammar.LineBreak.ShouldNotMatch("\\  block text");
		}
	}
}
