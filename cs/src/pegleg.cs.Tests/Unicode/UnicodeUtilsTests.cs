using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode {
	public class UnicodeUtilsTests {
		[Fact] public void UnicodeUtils_GetCodePoint_debugging() {
			var matchedCount = 0;
			var notMatchedCount = 0;
			int[] myCounts = new int[30];
			int[] theirCounts = new int[30];
			var mismatcheCodePoints = new LinkedList<int>();
			for(var codePoint = UnicodeUtils.MinCodePoint; codePoint <= UnicodeUtils.MaxCodePoint; codePoint++) {
				// CharUnicodeInfo.GetUnicodeCategory throws exception on surrogate values
				if(0xD800 <= codePoint && codePoint <= 0xDFFF)
					continue;

				var myCategory = UnicodeUtils.GetCategory(codePoint);
				var theirCategory = CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(codePoint), 0);
				if(myCategory == theirCategory) {
					matchedCount++;
				} else {
					mismatcheCodePoints.AddLast(codePoint);
					myCounts[(int)myCategory]++;
					theirCounts[(int)theirCategory]++;
					notMatchedCount++;
				}
			}
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_lowercase_letter() {
			UnicodeUtils.GetCategory('a').Should().Be(UnicodeCategory.LowercaseLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_uppercase_letter() {
			UnicodeUtils.GetCategory('A').Should().Be(UnicodeCategory.UppercaseLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_CJK_Ideograph_Extension_A_start() {
			for(var codePoint = 0x3400; codePoint <= 0x4DB5; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.OtherLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_CJK_Ideograph_start() {
			for(var codePoint = 0x4E00; codePoint <= 0x9FCC; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.OtherLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Hangul_syllable_start() {
			for(var codePoint = 0xAC00; codePoint <= 0xD7A3; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.OtherLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Non_Private_Use_High_Surrogate_range() {
			for(var codePoint = 0xD800; codePoint <= 0xDB7F; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.Surrogate);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Private_Use_High_Surrogate_range() {
			for(var codePoint = 0xDB80; codePoint <= 0xDBFF; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.Surrogate);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Low_Surrogate_range() {
			for(var codePoint = 0xDC00; codePoint <= 0xDFFF; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.Surrogate);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Private_Use_range() {
			for(var codePoint = 0xE000; codePoint <= 0xF8FF; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.PrivateUse);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_CJK_Ideograph_Extension_B_range() {
			for(var codePoint = 0x20000; codePoint <= 0x2A6D6; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.OtherLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_CJK_Ideograph_Extension_C_range() {
			for(var codePoint = 0x2A700; codePoint <= 0x2B734; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.OtherLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_CJK_Ideograph_Extension_D_range() {
			for(var codePoint = 0x2B740; codePoint <= 0x2B81D; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.OtherLetter);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Plane_15_Private_Use_range() {
			for(var codePoint = 0xF0000; codePoint <= 0xFFFFD; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.PrivateUse);
		}

		[Fact] public void UnicodeUtils_GetCategory_should_work_for_Plane_16_Private_Use_range() {
			for(var codePoint = 0x100000; codePoint <= 0x10FFFD; codePoint++)
				UnicodeUtils.GetCategory(codePoint).Should().Be(UnicodeCategory.PrivateUse);
		}

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
