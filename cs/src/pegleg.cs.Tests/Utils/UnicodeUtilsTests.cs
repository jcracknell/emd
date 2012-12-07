using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Utils {
	public class UnicodeUtilsTests {
		[Fact] public void UnicodeUtils_GetGraphemeInfo_should_work_for_a() {
			int length;
			UnicodeUtils.GetGraphemeInfo("a", 0, out length).Should().Be(UnicodeCategory.LowercaseLetter);
			length.Should().Be(1);
		}

		[Fact] public void UnicodeUtils_GetGraphemeInfo_should_work_for_a_b() {
			int length;
			UnicodeUtils.GetGraphemeInfo("ab", 0, out length).Should().Be(UnicodeCategory.LowercaseLetter);
			length.Should().Be(1);
		}

		[Fact] public void UnicodeUtils_GetGraphemeInfo_should_work_for_a_combining_ring() {
			int length;
			UnicodeUtils.GetGraphemeInfo("a\u030a", 0, out length).Should().Be(UnicodeCategory.LowercaseLetter);
			length.Should().Be(2);
		}

		[Fact] public void UnicodeUtils_GetGraphemeInfo_should_work_for_a_combining_ring_combining_caron() {
			int length;
			UnicodeUtils.GetGraphemeInfo("a\u030a\u030c", 0, out length).Should().Be(UnicodeCategory.LowercaseLetter);
			length.Should().Be(3);
		}

		[Fact] public void UnicodeUtils_GetGraphemeInfo_should_work_for_combining_ring_a_combining_ring() {
			int length;
			UnicodeUtils.GetGraphemeInfo("\u030aa\u030a", 0, out length).Should().Be(UnicodeCategory.NonSpacingMark);
			length.Should().Be(1);
		}
	}
}
