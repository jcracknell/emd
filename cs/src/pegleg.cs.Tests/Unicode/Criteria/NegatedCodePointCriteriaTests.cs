using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
  public class NegatedCodePointCriteriaTests {
    [Fact] public void NegatedCodePointCriterion_should_be_unsatisfiable() {
      new NegatedCodePointCriteria(SatisfiedCodePointCriterion.Instance)
      .SatisfiedBy('a')
      .Should().BeFalse();
    }
  }
}
