using FluentAssertions;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class GraphemeCriteriaTests {
    /*
    [Fact] public void GraphemeCriteria_All_Graphemes_should_accept_all_char_values() {
      var criteria = GraphemeCriteria.All.Graphemes;

      int length;
      foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
        criteria.AreSatisfiedBy(c.ToString(), 0, out length).Should().BeTrue();
        length.Should().Be(1);
      }
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_should_accept_no_char_values() {
      var criteria = GraphemeCriteria.No.Graphemes;

      int length;
      foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
        criteria.AreSatisfiedBy(c.ToString(), 0, out length).Should().BeFalse();
      }
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_Excluding_a_should_accept_a() {
      var criteria = GraphemeCriteria.In("a");

      int length;
      criteria.IsSatisfiedBy("a", 0, out length).Should().BeTrue();
      length.Should().Be(1);
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_Excluding_b_Excluding_a_should_accept_a_and_b() {
      var criteria = GraphemeCriteria.In("a", "b");

      int length;
      criteria.AreSatisfiedBy("b", 0, out length).Should().BeTrue();
      length.Should().Be(1);
      criteria.AreSatisfiedBy("a", 0, out length).Should().BeTrue();
      length.Should().Be(1);
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_Excluding_a_combining_ring_should_only_match_a_combining_ring() {
      var criteria = GraphemeCriteria.No.Graphemes.Excluding("a\u030a");

      int length;
      criteria.AreSatisfiedBy("a\u030a", 0, out length).Should().BeTrue();
      length.Should().Be(2);
      criteria.AreSatisfiedBy("a", 0, out length).Should().BeFalse();
      criteria.AreSatisfiedBy("a\u030a", 1, out length).Should().BeFalse();
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_Excluding_a_should_not_accept_a_combining_ring() {
      int length;
      GraphemeCriteria.No.Graphemes.Excluding('a').AreSatisfiedBy("a\u030a", 0, out length).Should().BeFalse("because this is a multi-char grapheme");
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_Excluding_LowercaseLetter_should_accept_lowercase_letters() {
      var criteria = GraphemeCriteria.No.Graphemes.Excluding(UnicodeCategory.LowercaseLetter);

      int length;
      int acceptedCount = 0;
      int lowercaseLetterCount = 0;
      foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
        if(criteria.AreSatisfiedBy(c.ToString(), 0, out length)) {
          UnicodeUtils.GetCategory(c).Should().Be(UnicodeCategory.LowercaseLetter);
          acceptedCount++;
        }

        if(UnicodeCategory.LowercaseLetter == UnicodeUtils.GetCategory(c))
          lowercaseLetterCount++;
      }

      acceptedCount.Should().Be(lowercaseLetterCount, "because this is the number of lowercase letter charaters which were tested");
    }

    [Fact] public void GraphemeCriteria_All_Graphemes_Excluding_LowercaseLetter_should_not_accept_lowercase_letters() {
      var criteria = GraphemeCriteria.All.Graphemes.Excluding(UnicodeCategory.LowercaseLetter);

      int length;
      int rejectedCount = 0;
      int lowercaseLetterCount = 0;
      foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
        if(!criteria.AreSatisfiedBy(c.ToString(), 0, out length)) {
          UnicodeUtils.GetCategory(c).Should().Be(UnicodeCategory.LowercaseLetter);
          rejectedCount++;
        }

        if(UnicodeCategory.LowercaseLetter == UnicodeUtils.GetCategory(c))
          lowercaseLetterCount++;
      }

      rejectedCount.Should().Be(lowercaseLetterCount, "because this is the number of lowercase letter charaters which were tested");
    }

    [Fact] public void GraphemeCriteria_No_Graphemes_Excluding_LowercaseLetter_Excluding_A_should_accept_lowercase_letters_and_A() {
      var criteria = GraphemeCriteria.No.Graphemes.Excluding(UnicodeCategory.LowercaseLetter).Excluding('A');

      int length;
      int acceptedCount = 0;
      int lowercaseLetterCount = 0;
      foreach(var c in CharUtils.Range(char.MinValue, char.MaxValue)) {
        if(criteria.AreSatisfiedBy(c.ToString(), 0, out length)) {
          if(UnicodeCategory.LowercaseLetter != UnicodeUtils.GetCategory(c))
            c.Should().Be('A');

          acceptedCount++;
        }

        if(UnicodeCategory.LowercaseLetter == UnicodeUtils.GetCategory(c)) {
          lowercaseLetterCount++;
        }
      }

      acceptedCount.Should().Be(lowercaseLetterCount + 1);
    }
    */
  }
}
