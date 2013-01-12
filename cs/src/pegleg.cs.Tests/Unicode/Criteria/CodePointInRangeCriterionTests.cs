using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class CodePointInRangeCriterionTests {
		[Fact] public void CodePointInRangeCriterion_should_be_satisfied_by_codepoint_at_start_of_range() {
			new CodePointInRangeCriterion('a', 'c')
			.IsSatisfiedBy('a')
			.Should().BeTrue();
		}

		[Fact] public void CodePointInRangeCriterion_should_be_satisfied_by_codepoint_at_end_of_range() {
			new CodePointInRangeCriterion('a', 'c')
			.IsSatisfiedBy('c')
			.Should().BeTrue();
		}

		[Fact] public void CodePointInRangeCriterion_should_not_be_satisfied_by_codepoint_preceding_range() {
			new CodePointInRangeCriterion('b', 'd')
			.IsSatisfiedBy('a')
			.Should().BeFalse();
		}

		[Fact] public void CodePointInRangeCriterion_should_not_be_satisfied_by_codepoint_following_range() {
			new CodePointInRangeCriterion('b', 'd')
			.IsSatisfiedBy('e')
			.Should().BeFalse();
		}
	}
}
