using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class DisjunctCodePointCriteriaTests {
    [Fact] public void DisjunctCodePointCriteria_should_be_satisfied_by_codepoint_satisfying_first_criterion_only() {
      var criteria = new DisjunctCodePointCriteria(new ICodePointCriteria[] {
        new InValuesCodePointCriterion(new int[] { 'a' }),
        new InValuesCodePointCriterion(new int[] { 'b' })
      })
      .SatisfiedBy('a')
      .Should().BeTrue();
    }

    [Fact] public void DisjunctCodePointCriteria_should_be_satisfied_by_codepoint_satisfying_second_criterion_only() {
      new DisjunctCodePointCriteria(new ICodePointCriteria[] {
        new InValuesCodePointCriterion(new int[] { 'a' }),
        new InValuesCodePointCriterion(new int[] { 'b' })
      })
      .SatisfiedBy('b')
      .Should().BeTrue();
    }

    [Fact] public void DisjunctCodePointCriteria_should_not_be_satisfied_by_codepoint_not_satisfying_criteria() {
      new DisjunctCodePointCriteria(new ICodePointCriteria[] {
        new InValuesCodePointCriterion(new int[] { 'a' }),
        new InValuesCodePointCriterion(new int[] { 'b' })
      })
      .SatisfiedBy('c')
      .Should().BeFalse();
    }
  }
}
