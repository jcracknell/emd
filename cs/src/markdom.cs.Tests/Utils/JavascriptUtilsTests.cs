using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Utils {
	public class JavascriptUtilsTests {
		[Fact] public void JavascriptUtils_DecodeString_should_leave_plain_string_unchanged() {
			JavascriptUtils.DecodeString("foo").Should().Be("foo");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_decode_hexadecimal_escape() {
			JavascriptUtils.DecodeString(@"\x61").Should().Be("a");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_handle_hexadecimal_escape_which_is_too_short() {
			JavascriptUtils.DecodeString(@"\x6").Should().Be("x6");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_handle_invalid_hexadecimal_escape() {
			JavascriptUtils.DecodeString(@"\x6g").Should().Be("x6g");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_decode_unicode_escape() {
			JavascriptUtils.DecodeString(@"\u0061").Should().Be("a");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_handle_unicode_escape_which_is_too_short() {
			JavascriptUtils.DecodeString(@"\u006").Should().Be("u006");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_handle_invalid_unicode_escape() {
			JavascriptUtils.DecodeString(@"\u006g").Should().Be("u006g");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_decode_escaped_line_feed() {
			JavascriptUtils.DecodeString("\\\n").Should().Be("\n");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_decode_escaped_carriage_return() {
			JavascriptUtils.DecodeString("\\\r").Should().Be("\r");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_decode_escaped_line_separator() {
			JavascriptUtils.DecodeString("\\\u2028").Should().Be("\u2028");
		}

		[Fact] public void JavascriptUtils_DecodeString_should_decode_escaped_paragraph_separator() {
			JavascriptUtils.DecodeString("\\\u2029").Should().Be("\u2029");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_backspace() {
			JavascriptUtils.EncodeString("\b").Should().Be("\\b");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_tab() {
			JavascriptUtils.EncodeString("\t").Should().Be("\\t");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_newline() {
			JavascriptUtils.EncodeString("\n").Should().Be("\\n");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_vertical_tab() {
			JavascriptUtils.EncodeString("\v").Should().Be("\\v");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_form_feed() {
			JavascriptUtils.EncodeString("\f").Should().Be("\\f");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_double_quote() {
			JavascriptUtils.EncodeString("\"").Should().Be("\\\"");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_single_quote() {
			JavascriptUtils.EncodeString("'").Should().Be("\\'");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_backslash() {
			JavascriptUtils.EncodeString("\\").Should().Be("\\\\");
		}	

		[Fact] public void JavascriptUtils_EncodeString_should_encode_line_separator() {
			JavascriptUtils.EncodeString("\x2028").Should().Be("\\u2028");
		}

		[Fact] public void JavascriptUtils_EncodeString_should_encode_paragraph_separator() {
			JavascriptUtils.EncodeString("\x2029").Should().Be("\\u2029");
		}
	}
}
