using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class InCategoriesCodePointCriterionTests {
		[Fact] public void InCategoriesCodePointCriterion_should_be_satisfied_by_codepoint_in_provided_category() {
			new InCategoriesCodePointCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
			.SatisfiedBy('a')
			.Should().BeTrue();
		}

		[Fact] public void InCategoriesCodePointCriterion_should_not_be_satisfied_by_codepoint_outside_of_provided_category() {
			new InCategoriesCodePointCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
			.SatisfiedBy('1')
			.Should().BeFalse();
		}

		[Fact] public void InCategoriesCodePointCriterion_should_be_satisfied_by_codepoints_in_multiple_provided_categories() {
			var criterion = new InCategoriesCodePointCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter });
			criterion.SatisfiedBy('a').Should().BeTrue();
			criterion.SatisfiedBy('A').Should().BeTrue();
			criterion.SatisfiedBy('1').Should().BeFalse();
		}
	}
}
