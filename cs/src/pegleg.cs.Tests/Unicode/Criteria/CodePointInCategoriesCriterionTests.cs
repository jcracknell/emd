using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class CodePointInCategoriesCriterionTests {
		[Fact] public void CodePointInCategoriesCriterion_should_be_satisfied_by_codepoint_in_provided_category() {
			new CodePointInCategoriesCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
			.IsSatisfiedBy('a')
			.Should().BeTrue();
		}

		[Fact] public void CodePointInCategoriesCriterion_should_not_be_satisfied_by_codepoint_outside_of_provided_category() {
			new CodePointInCategoriesCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
			.IsSatisfiedBy('1')
			.Should().BeFalse();
		}

		[Fact] public void CodePointInCategoriesCriterion_should_be_satisfied_by_codepoints_in_multiple_provided_categories() {
			var criterion = new CodePointInCategoriesCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter });
			criterion.IsSatisfiedBy('a').Should().BeTrue();
			criterion.IsSatisfiedBy('A').Should().BeTrue();
			criterion.IsSatisfiedBy('1').Should().BeFalse();
		}
	}
}
