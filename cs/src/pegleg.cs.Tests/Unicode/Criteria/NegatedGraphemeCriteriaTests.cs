using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class NegatedGraphemeCriteriaTests {
		[Fact] public void NegatedGraphemeCriteria_should_be_unsatisfiable() {
			new NegatedGraphemeCriteria(SatisfiedGraphemeCriterion.Instance)
			.SatisfiedBy("a", 0, 1, UnicodeCategory.LowercaseLetter)
			.Should().BeFalse();
		}
	}
}
