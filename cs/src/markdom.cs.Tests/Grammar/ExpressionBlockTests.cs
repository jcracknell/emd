using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class ExpressionBlockTests : GrammarTestFixture {
		[Fact] public void ExpressionBlock_should_match_expression_line() {
			MarkdomGrammar.ExpressionBlock.ShouldMatch("@foo\n");
		}

		[Fact] public void ExpressionBlock_should_not_match_expression_followed_by_content() {
			MarkdomGrammar.ExpressionBlock.ShouldNotMatch("@foo text");
		}

		[Fact] public void ExpressionBlock_should_not_match_expression_line_followed_by_content() {
			MarkdomGrammar.ExpressionBlock.ShouldNotMatch(
				"@foo\n",
				"text"
			);
		}
	}
}
