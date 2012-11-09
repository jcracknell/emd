using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class SingleLineCommentTests : GrammarTestFixture {
		[Fact] public void SingleLineComment_matches_base_case() {
			var match = Grammar.SingleLineComment.ShouldMatch("// text");
		}
	}
}
