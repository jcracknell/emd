using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Utils {
	public class StringUtilsTests {
		[Fact] public void StringUtils_LiteralEncode_should_quote_simple_text() {
			StringUtils.LiteralEncode("cat").Should().Be("\"cat\"");
		}

		[Fact] public void StringUtils_LiteralEncode_should_encode_null() {
			StringUtils.LiteralEncode(null).Should().Be("null");
		}
	}
}
