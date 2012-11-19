using FluentAssertions;
using markdom.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class CodeTests : GrammarTestFixture {
		[Fact] public void Code_should_match_ticks_1() {
			var expected = new CodeNode("fizzbuzz()", new SourceRange(0,12,1,0));

			var match = MarkdomGrammar.Code.ShouldMatch("`fizzbuzz()`");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Code_should_match_ticks_8() {
			var expected = new CodeNode("fizzbuzz()", new SourceRange(0,26,1,0));

			var match = MarkdomGrammar.Code.ShouldMatch("````````fizzbuzz()````````");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Code_should_span_multiple_lines() {
			var expected = new CodeNode("line 1\nline 2", new SourceRange(0,15,1,0));

			var match = MarkdomGrammar.Code.ShouldMatch(
				"`line 1\n",
				"line 2`"
			);

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Code_should_not_span_blank_line() {
			MarkdomGrammar.Code.ShouldNotMatch(
				"`line 1\n",
				"\n",
				"line 3`"
			);
		}
	}
}
