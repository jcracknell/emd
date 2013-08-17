using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class ConjunctGraphemeCriteriaTests {
    [Fact] public void ConjunctGraphemeCriteria_should_work() {
      var criteria = new ConjunctGraphemeCriteria(new IGraphemeCriteria[] {
        new InCategoriesGraphemeCriterion(new UnicodeCategory[] { UnicodeCategory.UppercaseLetter }),
        new InValuesGraphemeCriterion(new string[] { "A", "a" })
      });

      criteria.SatisfiedBy("A", 0, 1, UnicodeCategory.UppercaseLetter).Should().BeTrue();
      criteria.SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter).Should().BeFalse();
    }
  }
}
