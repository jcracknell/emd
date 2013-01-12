using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode {
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

		[Fact] public void UnicodeUtils_GetCodePoint_should_work_for_first_plane_character() {
			int length;
			UnicodeUtils.GetCodePoint("a", 0, out length).Should().Be(0x61);
			length.Should().Be(1);
		}

		[Fact] public void UnicodeUtils_GetCodePoint_should_return_first_codepoint_of_invalid_surrogate_pair_verbatim() {
			int length;
			UnicodeUtils.GetCodePoint("\u8812a", 0, out length).Should().Be(0x8812);
			length.Should().Be(1);
		}

		[Fact] public void UnicodeUtils_GetCodePoint_should_work_for_minimum_surrogate_pair() {
			int length;
			UnicodeUtils.GetCodePoint("\uD800\uDC00", 0, out length).Should().Be(0x10000);
			length.Should().Be(2);
		}

		[Fact] public void UnicodeUtils_GetCodePoint_should_work_for_maximum_surrogate_pair() {
			int length;
			UnicodeUtils.GetCodePoint("\uDBFF\uDFFF", 0, out length).Should().Be(0x10FFFF);
			length.Should().Be(2);
		}
	}
}
