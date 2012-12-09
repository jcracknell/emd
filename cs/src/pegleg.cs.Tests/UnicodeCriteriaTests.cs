using FluentAssertions;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs {
	public class UnicodeCriteriaTests {
		[Fact] public void UnicodeCriteria_AnyCharacter_should_accept_all_char_values() {
			var criteria = UnicodeCriteria.AnyCharacter;

			int length;
			foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
				criteria.Accepts(c.ToString(), 0, out length).Should().BeTrue();
				length.Should().Be(1);
			}
		}

		[Fact] public void UnicodeCriteria_NoCharacter_should_accept_no_char_values() {
			var criteria = UnicodeCriteria.NoCharacter;

			int length;
			foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
				criteria.Accepts(c.ToString(), 0, out length).Should().BeFalse();
			}
		}

		[Fact] public void UnicodeCriteria_NoCharacter_Except_a_should_accept_a() {
			var criteria = UnicodeCriteria.NoCharacter.Except('a');

			int length;
			criteria.Accepts("a", 0, out length).Should().BeTrue();
			length.Should().Be(1);
		}

		[Fact] public void UnicodeCriteria_NoCharacter_Except_b_Except_a_should_accept_a_and_b() {
			var criteria = UnicodeCriteria.NoCharacter.Except('b').Except('a');

			int length;
			criteria.Accepts("b", 0, out length).Should().BeTrue();
			length.Should().Be(1);
			criteria.Accepts("a", 0, out length).Should().BeTrue();
			length.Should().Be(1);
		}

		[Fact] public void UnicodeCriteria_NoCharacter_Except_a_combining_ring_should_only_match_a_combining_ring() {
			var criteria = UnicodeCriteria.NoCharacter.Except("a\u030a");

			int length;
			criteria.Accepts("a\u030a", 0, out length).Should().BeTrue();
			length.Should().Be(2);
			criteria.Accepts("a", 0, out length).Should().BeFalse();
			criteria.Accepts("a\u030a", 1, out length).Should().BeFalse();
		}

		[Fact] public void UnicodeCriteria_NoCharacter_Except_a_should_not_accept_a_combining_ring() {
			int length;
			UnicodeCriteria.NoCharacter.Except('a').Accepts("a\u030a", 0, out length).Should().BeFalse("because this is a multi-char grapheme");
		}

		/*
		[Fact] public void UnicodeCriteria_NoCharacter_Except_ab_should_throw_exception() {
			UnicodeCriteria.NoCharacter.Invoking(c => c.Save("ab")).ShouldThrow<ArgumentException>();
		}
		*/

		[Fact] public void UnicodeCriteria_NoCharacter_Except_LowercaseLetter_should_accept_lowercase_letters() {
			var criteria = UnicodeCriteria.NoCharacter.Except(UnicodeCategory.LowercaseLetter);

			int length;
			int acceptedCount = 0;
			int lowercaseLetterCount = 0;
			foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
				if(criteria.Accepts(c.ToString(), 0, out length)) {
					Char.GetUnicodeCategory(c).Should().Be(UnicodeCategory.LowercaseLetter);
					acceptedCount++;
				}

				if(UnicodeCategory.LowercaseLetter == Char.GetUnicodeCategory(c))
					lowercaseLetterCount++;
			}

			acceptedCount.Should().Be(lowercaseLetterCount, "because this is the number of lowercase letter charaters which were tested");
		}

		[Fact] public void UnicodeCriteria_AnyCharacter_Except_LowercaseLetter_should_not_accept_lowercase_letters() {
			var criteria = UnicodeCriteria.AnyCharacter.Except(UnicodeCategory.LowercaseLetter);

			int length;
			int rejectedCount = 0;
			int lowercaseLetterCount = 0;
			foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
				if(!criteria.Accepts(c.ToString(), 0, out length)) {
					Char.GetUnicodeCategory(c).Should().Be(UnicodeCategory.LowercaseLetter);
					rejectedCount++;
				}

				if(UnicodeCategory.LowercaseLetter == Char.GetUnicodeCategory(c))
					lowercaseLetterCount++;
			}

			rejectedCount.Should().Be(lowercaseLetterCount, "because this is the number of lowercase letter charaters which were tested");
		}

		[Fact] public void UnicodeCriteria_NoCharacter_Except_LowercaseLetter_Except_A_should_accept_lowercase_letters_and_A() {
			var criteria = UnicodeCriteria.NoCharacter.Except(UnicodeCategory.LowercaseLetter).Except('A');

			int length;
			int acceptedCount = 0;
			int lowercaseLetterCount = 0;
			foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
				if(criteria.Accepts(c.ToString(), 0, out length)) {
					if(UnicodeCategory.LowercaseLetter != Char.GetUnicodeCategory(c))
						c.Should().Be('A');

					acceptedCount++;
				}

				if(UnicodeCategory.LowercaseLetter == Char.GetUnicodeCategory(c)) {
					lowercaseLetterCount++;
				}
			}

			acceptedCount.Should().Be(lowercaseLetterCount + 1);
		}
	}
}
