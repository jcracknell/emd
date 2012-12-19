using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class BlockWhitespaceTests {
		[Fact] public void BlockWhitespace_should_not_match_newline_followed_by_blank_line() {
			EmdGrammar.BlockWhitespace.ShouldNotMatch(
				"\n",
				"    \n"
			);
		}
	}
}
