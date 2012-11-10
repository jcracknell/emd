using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Utils {
	public class CharUtilsTests {
		[Fact] public void RangesCovering_works_as_expected_for_single_char() {
			CharUtils.RangesCovering(new char[] { 'a' })
				.Should().HaveCount(1)
				.And.Contain(new CharacterRange('a', 'a'));
		}

		[Fact] public void RangesCovering_works_as_expected_for_multiple_consecutive_chars() {
			CharUtils.RangesCovering(new char[] { 'a', 'b', 'c' })
				.Should().HaveCount(1)
				.And.Contain(new CharacterRange('a', 'c'));
		}

		[Fact] public void RangesCovering_works_as_expected_for_multiple_non_consecutive_chars() {
			CharUtils.RangesCovering(new char[] { 'd', 'm', 'b', 'c', 'n' })
				.Should().HaveCount(2)
				.And.Contain(new CharacterRange('b', 'd'))
				.And.Contain(new CharacterRange('m', 'n'));
		}

		[Fact] public void RangesExcluding_works_as_expected_for_single_char() {
			CharUtils.RangesExcluding(new char[] { 'j' })
				.Should().HaveCount(2)
				.And.Contain(new CharacterRange(char.MinValue, 'i'))
				.And.Contain(new CharacterRange('k', char.MaxValue));
		}

		[Fact] public void RangesExcluding_works_as_expected_for_multiple_consecutive_chars() {
			CharUtils.RangesExcluding(new char[] { 'i', 'j', 'k' })
				.Should().HaveCount(2)
				.And.Contain(new CharacterRange(char.MinValue, 'h'))
				.And.Contain(new CharacterRange('l', char.MaxValue));
		}

		[Fact] public void RangesExcluding_works_as_expected_for_multiple_non_consecutive_chars() {
			CharUtils.RangesExcluding(new char[] { 'd', 'm', 'b', 'c', 'n' })
				.Should().HaveCount(3)
				.And.Contain(new CharacterRange(char.MinValue, 'a'))
				.And.Contain(new CharacterRange('e', 'l'))
				.And.Contain(new CharacterRange('o', char.MaxValue));
		}

		[Fact] public void LiteralEncode_converts_visible_ascii_to_char_literal() {
			CharUtils.LiteralEncode('a').Should().Be("'a'");
		}

		[Fact] public void LiteralEncode_converts_non_visible_ascii_to_hex_literal() {
			CharUtils.LiteralEncode((char)1).Should().Be("'\\x0001'");
		}
	}
}
