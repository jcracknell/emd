using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class InValuesCodePointCriterionTests {
    [Fact] public void InValuesCodePointCriterion_should_be_satisfied_by_provided_value() {
      new InValuesCodePointCriterion(new int[] { 0x10000 })
      .SatisfiedBy(0x10000)
      .Should().BeTrue();
    }

    [Fact] public void InValuesCodePointCriterion_should_not_be_satisfied_by_unprovided_value() {
      new InValuesCodePointCriterion(new int[] { 0x10000 })
      .SatisfiedBy(0x0FFFF)
      .Should().BeFalse();
    }

    [Fact] public void InValuesCodePointCriterion_works_for_maximum_code_point() {
      new InValuesCodePointCriterion(new int[] { 0x10FFFF })
      .SatisfiedBy(0x10FFFF)
      .Should().BeTrue();
    }

    [Fact] public void InValuesCodePointCriterion_works_for_minimum_code_point() {
      new InValuesCodePointCriterion(new int[] { 0x0000 })
      .SatisfiedBy(0x0000)
      .Should().BeTrue();
    }

    [Fact] public void InValuesCodePointCriterion_works_for_multiple_codepoints() {
      var criterion = new InValuesCodePointCriterion(new int[] { 'a', 'b' });
      criterion.SatisfiedBy('a').Should().BeTrue();
      criterion.SatisfiedBy('b').Should().BeTrue();
      criterion.SatisfiedBy('c').Should().BeFalse();
    }

    [Fact] public void InValuesCodePointCriterion_works_for_negative_codepoint_value() {
      new InValuesCodePointCriterion(new int[] { 'a' })
      .SatisfiedBy(-1)
      .Should().BeFalse("because it is not a valid code point");
    }

    [Fact] public void InValuesCodePointCriterion_should_not_be_satisfied_by_overlarge_codepoint_value() {
      new InValuesCodePointCriterion(new int[] { 0 })
      .SatisfiedBy(0x110000)
      .Should().BeFalse("because it is not a valid code point");
    }
  }
}
