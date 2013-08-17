using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class DisjunctGraphemeCriteriaTests {
    [Fact] public void DisjunctGraphemeCriteria_should_be_satisfied_by_grapheme_satisfying_first_criterion_only() {
      new DisjunctGraphemeCriteria(new IGraphemeCriteria[] {
        new InValuesGraphemeCriterion(new string[] { "a" }),
        new InValuesGraphemeCriterion(new string[] { "b" }),
      })
      .SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter)
      .Should().BeTrue();
    }

    [Fact] public void DisjunctGraphemeCriteria_should_be_satisfied_by_grapheme_satisfying_second_criterion_only() {
      new DisjunctGraphemeCriteria(new IGraphemeCriteria[] {
        new InValuesGraphemeCriterion(new string[] { "a" }),
        new InValuesGraphemeCriterion(new string[] { "b" }),
      })
      .SatisfiedBy("b", 0, 1, UnicodeCategory.LowercaseLetter)
      .Should().BeTrue();
    }

    [Fact] public void DisjunctGraphemeCriteria_should_not_be_satisfied_by_grapheme_not_satisfying_criteria() {
      new DisjunctGraphemeCriteria(new IGraphemeCriteria[] {
        new InValuesGraphemeCriterion(new string[] { "a" }),
        new InValuesGraphemeCriterion(new string[] { "b" }),
      })
      .SatisfiedBy("c", 0, 1, UnicodeCategory.LowercaseLetter)
      .Should().BeFalse();
    }
  }
}
