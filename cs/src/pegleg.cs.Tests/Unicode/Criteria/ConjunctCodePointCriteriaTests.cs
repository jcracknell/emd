using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class ConjunctCodePointCriteriaTests {
		[Fact] public void ConjunctCodePointCriteria_should_work() {
			var criteria = new ConjunctCodePointCriteria(new ICodePointCriteria[] {
				new InCategoriesCodePointCriterion(new UnicodeCategory[] { UnicodeCategory.LowercaseLetter }),
				new InRangeCodePointCriterion('M', 'm')
			});

			criteria.SatisfiedBy('c').Should().BeTrue();
			criteria.SatisfiedBy('Z').Should().BeFalse();
			criteria.SatisfiedBy('m').Should().BeTrue();
			criteria.SatisfiedBy('n').Should().BeFalse();
		}
	}
}
