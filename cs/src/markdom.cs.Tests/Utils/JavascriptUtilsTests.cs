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
	}
}
