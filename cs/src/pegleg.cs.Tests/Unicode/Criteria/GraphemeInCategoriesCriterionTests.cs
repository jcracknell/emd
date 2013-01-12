using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class GraphemeInCategoriesCriterionTests {
		[Fact] public void GraphemeInCategoriesCriterion_should_be_satisfied_by_grapheme_from_provided_category() {
			new GraphemeInCategoriesCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
			.IsSatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter)
			.Should().BeTrue();
		}

		[Fact] public void GraphemeInCategoriesCriterion_should_not_be_satisfied_by_grapheme_outside_of_provided_category() {
			new GraphemeInCategoriesCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter })
			.IsSatisfiedBy("A", 0, 1, UnicodeCategory.UppercaseLetter)
			.Should().BeFalse();
		}

		[Fact] public void GraphemeInCategoriesCriterion_should_be_satisfied_by_graphemes_from_multiple_provided_categories() {
			var criterion = new GraphemeInCategoriesCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter });
			criterion.IsSatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter).Should().BeTrue();
			criterion.IsSatisfiedBy("A", 0, 1, UnicodeCategory.UppercaseLetter).Should().BeTrue();
			criterion.IsSatisfiedBy("1", 0, 1, UnicodeCategory.DecimalDigitNumber).Should().BeFalse();
		}
	}
}
