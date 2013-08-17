using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class SingleCodePointGraphemeCriterionTests {
    [Fact] public void SingleCodePointGraphemeCriterion_should_be_satisfied_by_trivial_single_code_point() {
      new SingleCodePointGraphemeCriterion()
      .SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter)
      .Should().BeTrue();
    }

    [Fact] public void SingleCodePointGraphemeCriterion_should_be_satisfied_by_multi_code_point_grapheme() {
      new SingleCodePointGraphemeCriterion()
      .SatisfiedBy("a\u030a", 0, 2, UnicodeCategory.LowercaseLetter)
      .Should().BeFalse();
    }

    [Fact] public void SingleCodePointGraphemeCriterion_should_be_satisfied_by_surrogate_pair() {
      new SingleCodePointGraphemeCriterion()
      .SatisfiedBy("\uD83D\uDC35", 0, 2, UnicodeCategory.OtherSymbol)
      .Should().BeTrue();
    }

    [Fact] public void SingleCodePointGraphemeCriterion_should_be_satisfied_by_grapheme_satisfying_criteria() {
      new SingleCodePointGraphemeCriterion(CodePointCriteria.InCategories(UnicodeCategory.LowercaseLetter))
      .SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter)
      .Should().BeTrue();
    }

    [Fact] public void SingleCodePointGraphemeCriterion_should_not_be_satisfied_by_grapheme_not_satisfying_criteria() {
      new SingleCodePointGraphemeCriterion(CodePointCriteria.InCategories(UnicodeCategory.LowercaseLetter))
      .SatisfiedBy("A", 0, 1, UnicodeCategory.UppercaseLetter)
      .Should().BeFalse();
    }
  }
}
