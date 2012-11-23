using FluentAssertions;
using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class StringLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void StringLiteralExpression_matches_single_quoted_string() {
			var matchResult = MarkdomGrammar.StringLiteralExpression.ShouldMatch("'string'");

			matchResult.Product.Value.Should().Be("string");
		}
		
		[Fact] public void StringLiteralExpression_should_decode_escaped_newline_in_single_quoted_string() {
			var match = MarkdomGrammar.StringLiteralExpression.ShouldMatch("'\\n'");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression( "\n", new SourceRange(0,4,1,0)));
		}

		[Fact] public void StringLiteralExpression_matches_double_quoted_string() {
			var expected = new StringLiteralExpression("string", new SourceRange(0, 8, 1, 0));
			var matchResult = MarkdomGrammar.StringLiteralExpression.ShouldMatch(@"""string""");

			matchResult.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void StringLiteralExpression_should_decode_escaped_tab_in_double_quoted_string() {
			var match = MarkdomGrammar.StringLiteralExpression.ShouldMatch("\"\\t\"");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("\t", new SourceRange(0,4,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_match_verbatim_string_1() {
			var expected = new StringLiteralExpression("string", new SourceRange(0,8,1,0));

			var match = MarkdomGrammar.StringLiteralExpression.ShouldMatch(@"`string`");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void StringLiteralExpression_should_match_verbatim_string_16() {
			var expected = new StringLiteralExpression("string", new SourceRange(0,38,1,0));

			var match = MarkdomGrammar.StringLiteralExpression.ShouldMatch(@"````````````````string````````````````");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void StringLiteralExpression_should_not_decode_escaped_newline_in_verbatim_string_1() {
			var match = MarkdomGrammar.StringLiteralExpression.ShouldMatch(@"`\n`");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("\\n", new SourceRange(0,4,1,0)));
		}
	}
}
