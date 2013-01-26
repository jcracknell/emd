using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class StringLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void StringLiteralExpression_matches_single_quoted_string() {
			var matchResult = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'string'");

			matchResult.Product.Value.Should().Be("string");
		}
		
		[Fact] public void StringLiteralExpression_should_decode_escaped_newline_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'\\n'");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression( "\n", new SourceRange(0,4,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_decode_hexadecimal_escape_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf(@"'\x0a'");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("\n", new SourceRange(0,6,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_decode_unicode_escape_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf(@"'\u000a'");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("\n", new SourceRange(0,8,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_match_escaped_verbatim_line_feed_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'foo\\\nbar'");
			
			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("foo\nbar", new SourceRange(0,10,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_not_match_unescaped_line_feed_in_single_quoted_string() {
			EmdGrammar.StringLiteralExpression.ShouldNotMatch("'foo\nbar'");
		}

		[Fact] public void StringLiteralExpression_should_match_escaped_verbatim_carriage_return_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'foo\\\rbar'");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("foo\rbar", new SourceRange(0,10,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_not_match_unescaped_carriage_return_in_single_quoted_string() {
			EmdGrammar.StringLiteralExpression.ShouldNotMatch("'foo\rbar'");
		}

		[Fact] public void StringLiteralExpression_should_match_escaped_verbatim_line_separator_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'foo\\\x2028bar'");
			
			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("foo\x2028bar", new SourceRange(0,10,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_not_match_unescaped_line_separator_in_single_quoted_string() {
			EmdGrammar.StringLiteralExpression.ShouldNotMatch("'foo\x2028bar'");
		}

		[Fact] public void StringLiteralExpression_should_match_escaped_verbatim_paragraph_separator_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'foo\\\x2029bar'");
			 match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("foo\x2029bar", new SourceRange(0,10,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_not_match_unescaped_paragraph_separator_in_single_quoted_string() {
			EmdGrammar.StringLiteralExpression.ShouldNotMatch("'foo\x2029bar'");
		}

		[Fact] public void StringLiteralExpression_should_match_escaped_verbatim_windows_line_terminator_in_single_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("'foo\\\r\nbar'");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("foo\r\nbar", new SourceRange(0,11,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_not_match_unescaped_windows_line_terminator_in_single_quoted_string() {
			EmdGrammar.StringLiteralExpression.ShouldNotMatch("'foo\r\nbar'");
		}

		[Fact] public void StringLiteralExpression_matches_double_quoted_string() {
			var expected = new StringLiteralExpression("string", new SourceRange(0, 8, 1, 0));
			var matchResult = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf(@"""string""");

			matchResult.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void StringLiteralExpression_should_decode_escaped_tab_in_double_quoted_string() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf("\"\\t\"");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("\t", new SourceRange(0,4,1,0)));
		}

		[Fact] public void StringLiteralExpression_should_match_verbatim_string_1() {
			var expected = new StringLiteralExpression("string", new SourceRange(0,8,1,0));

			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf(@"`string`");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void StringLiteralExpression_should_match_verbatim_string_16() {
			var expected = new StringLiteralExpression("string", new SourceRange(0,38,1,0));

			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf(@"````````````````string````````````````");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void StringLiteralExpression_should_not_decode_escaped_newline_in_verbatim_string_1() {
			var match = EmdGrammar.StringLiteralExpression.ShouldMatchAllOf(@"`\n`");

			match.Product.ShouldBeEquivalentTo(new StringLiteralExpression("\\n", new SourceRange(0,4,1,0)));
		}
	}
}
