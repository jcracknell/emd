using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class SingleLineCommentTests : GrammarTestFixture {
		[Fact] public void SingleLineComment_matches_base_case() {
			EmdGrammar.SingleLineComment.ShouldMatchAllOf("// text");
		}

		[Fact] public void SingleLineComment_should_match_with_no_comment_text() {
			EmdGrammar.SingleLineComment.ShouldMatchAllOf("//");
		}

		[Fact] public void SingleLineComment_should_not_match_trailing_newline() {
			var context = new pegleg.cs.Parsing.MatchingContext("// comment\n");
			var match = EmdGrammar.SingleLineComment.Matches(context);

			match.Succeeded.Should().BeTrue();
			context.Index.Should().Be(10);
		}
	}
}
