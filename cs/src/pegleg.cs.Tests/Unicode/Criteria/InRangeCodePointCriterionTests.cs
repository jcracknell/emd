using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class InRangeCodePointCriterionTests {
    [Fact] public void InRangeCodePointCriterion_should_be_satisfied_by_codepoint_at_start_of_range() {
      new InRangeCodePointCriterion('a', 'c')
      .SatisfiedBy('a')
      .Should().BeTrue();
    }

    [Fact] public void InRangeCodePointCriterion_should_be_satisfied_by_codepoint_at_end_of_range() {
      new InRangeCodePointCriterion('a', 'c')
      .SatisfiedBy('c')
      .Should().BeTrue();
    }

    [Fact] public void InRangeCodePointCriterion_should_not_be_satisfied_by_codepoint_preceding_range() {
      new InRangeCodePointCriterion('b', 'd')
      .SatisfiedBy('a')
      .Should().BeFalse();
    }

    [Fact] public void InRangeCodePointCriterion_should_not_be_satisfied_by_codepoint_following_range() {
      new InRangeCodePointCriterion('b', 'd')
      .SatisfiedBy('e')
      .Should().BeFalse();
    }
  }
}
