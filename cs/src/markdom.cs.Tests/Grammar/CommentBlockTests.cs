using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar
{
	public class CommentBlockTests : GrammarTestFixture {
		[Fact] public void CommentBlock_matches_single_line_comment() {
			var match = Grammar.CommentBlock.ShouldMatch("// text");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void CommentBlock_matches_indented_single_line_comment() {
			var match = Grammar.CommentBlock.ShouldMatch("    // text");

			match.Succeeded.Should().BeTrue();
		}

		[Fact] public void CommentBlock_does_not_match_multi_line_comment_with_trailing_content() {
			Grammar.CommentBlock.ShouldNotMatch(
				"/* This is a multi-line comment\n",
				"with trailing content */ text");
		}
	}
}
