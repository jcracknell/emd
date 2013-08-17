using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class InCategoriesGraphemeCriterionTests {
    [Fact] public void InCategoriesGraphemeCriterion_should_be_satisfied_by_grapheme_from_provided_category() {
      new InCategoriesGraphemeCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
      .SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter)
      .Should().BeTrue();
    }

    [Fact] public void InCategoriesGraphemeCriterion_should_not_be_satisfied_by_grapheme_outside_of_provided_category() {
      new InCategoriesGraphemeCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
      .SatisfiedBy("A", 0, 1, UnicodeCategory.UppercaseLetter)
      .Should().BeFalse();
    }

    [Fact] public void InCategoriesGraphemeCriterion_should_be_satisfied_by_graphemes_from_multiple_provided_categories() {
      var criterion = new InCategoriesGraphemeCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter });
      criterion.SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter).Should().BeTrue();
      criterion.SatisfiedBy("A", 0, 1, UnicodeCategory.UppercaseLetter).Should().BeTrue();
      criterion.SatisfiedBy("1", 0, 1, UnicodeCategory.DecimalDigitNumber).Should().BeFalse();
    }
  }
}
