using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Utils {
	public class JavaScriptUtilsTests {
		[Fact] public void JavascriptUtiles_IdentifierDecode_should_leave_plain_identifier_unchanged() {
			JavaScriptUtils.IdentifierDecode("foo").Should().Be("foo");
		}

		[Fact] public void JavaScriptUtilities_IdentifierDecode_should_decode_unicode_escape() {
			JavaScriptUtils.IdentifierDecode(@"\u0061").Should().Be("a");
		}

		[Fact] public void JavaScriptUtils_IdentifierDecode_should_not_decode_invalid_hexadecimal_escape() {
			JavaScriptUtils.IdentifierDecode(@"\u006g").Should().Be(@"\u006g");
		}

		[Fact] public void JavaScriptUtils_IdentifierDecode_should_not_decode_hexadecimal_escape() {
			JavaScriptUtils.IdentifierDecode(@"\x61").Should().Be(@"\x61");
		}

		[Fact] public void JavaScriptUtils_IdentifierEncode_should_leave_plain_identifier_name_unchanged() {
			JavaScriptUtils.IdentifierEncode("foo").Should().Be("foo");
		}

		[Fact] public void JavaScriptUtils_IdentifierEncode_should_encode_non_printable_ascii_character_as_unicode_escape() {
			JavaScriptUtils.IdentifierEncode("\x0233glise").Should().Be(@"\u0233glise");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_leave_plain_string_unchanged() {
			JavaScriptUtils.StringDecode("foo").Should().Be("foo");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_decode_hexadecimal_escape() {
			JavaScriptUtils.StringDecode(@"\x61").Should().Be("a");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_handle_hexadecimal_escape_which_is_too_short() {
			JavaScriptUtils.StringDecode(@"\x6").Should().Be("x6");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_handle_invalid_hexadecimal_escape() {
			JavaScriptUtils.StringDecode(@"\x6g").Should().Be("x6g");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_decode_unicode_escape() {
			JavaScriptUtils.StringDecode(@"\u0061").Should().Be("a");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_handle_unicode_escape_which_is_too_short() {
			JavaScriptUtils.StringDecode(@"\u006").Should().Be("u006");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_handle_invalid_unicode_escape() {
			JavaScriptUtils.StringDecode(@"\u006g").Should().Be("u006g");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_decode_escaped_line_feed() {
			JavaScriptUtils.StringDecode("\\\n").Should().Be("\n");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_decode_escaped_carriage_return() {
			JavaScriptUtils.StringDecode("\\\r").Should().Be("\r");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_decode_escaped_line_separator() {
			JavaScriptUtils.StringDecode("\\\u2028").Should().Be("\u2028");
		}

		[Fact] public void JavascriptUtils_StringDecode_should_decode_escaped_paragraph_separator() {
			JavaScriptUtils.StringDecode("\\\u2029").Should().Be("\u2029");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_backspace() {
			JavaScriptUtils.StringEncode("\b").Should().Be("\\b");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_tab() {
			JavaScriptUtils.StringEncode("\t").Should().Be("\\t");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_newline() {
			JavaScriptUtils.StringEncode("\n").Should().Be("\\n");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_vertical_tab() {
			JavaScriptUtils.StringEncode("\v").Should().Be("\\v");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_form_feed() {
			JavaScriptUtils.StringEncode("\f").Should().Be("\\f");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_double_quote() {
			JavaScriptUtils.StringEncode("\"").Should().Be("\\\"");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_single_quote() {
			JavaScriptUtils.StringEncode("'").Should().Be("\\'");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_backslash() {
			JavaScriptUtils.StringEncode("\\").Should().Be("\\\\");
		}	

		[Fact] public void JavascriptUtils_StringEncode_should_encode_line_separator() {
			JavaScriptUtils.StringEncode("\x2028").Should().Be("\\u2028");
		}

		[Fact] public void JavascriptUtils_StringEncode_should_encode_paragraph_separator() {
			JavaScriptUtils.StringEncode("\x2029").Should().Be("\\u2029");
		}
	}
}
