using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class CodePointInValuesCriterionTests {
		[Fact] public void CodePointInValuesCriterion_should_be_satisfied_by_provided_value() {
			new CodePointInValuesCriterion(new int[] { 0x10000 })
			.IsSatisfiedBy(0x10000)
			.Should().BeTrue();
		}

		[Fact] public void CodePointInValuesCriterion_should_not_be_satisfied_by_unprovided_value() {
			new CodePointInValuesCriterion(new int[] { 0x10000 })
			.IsSatisfiedBy(0x0FFFF)
			.Should().BeFalse();
		}

		[Fact] public void CodePointInValuesCriterion_works_for_maximum_code_point() {
			new CodePointInValuesCriterion(new int[] { 0x10FFFF })
			.IsSatisfiedBy(0x10FFFF)
			.Should().BeTrue();
		}

		[Fact] public void CodePointInValuesCriterion_works_for_minimum_code_point() {
			new CodePointInValuesCriterion(new int[] { 0x0000 })
			.IsSatisfiedBy(0x0000)
			.Should().BeTrue();
		}

		[Fact] public void CodePointInValuesCriterion_works_for_multiple_codepoints() {
			var criterion = new CodePointInValuesCriterion(new int[] { 'a', 'b' });
			criterion.IsSatisfiedBy('a').Should().BeTrue();
			criterion.IsSatisfiedBy('b').Should().BeTrue();
			criterion.IsSatisfiedBy('c').Should().BeFalse();
		}

		[Fact] public void CodePointInValuesCriterion_works_for_negative_codepoint_value() {
			new CodePointInValuesCriterion(new int[] { 'a' })
			.IsSatisfiedBy(-1)
			.Should().BeFalse("because it is not a valid code point");
		}

		[Fact] public void CodePointInValuesCriterion_should_not_be_satisfied_by_overlarge_codepoint_value() {
			new CodePointInValuesCriterion(new int[] { 0 })
			.IsSatisfiedBy(0x110000)
			.Should().BeFalse("because it is not a valid code point");
		}
	}
}
