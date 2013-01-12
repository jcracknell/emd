using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Unicode.Criteria {
	public class GraphemeInValuesCriterionTests {
		private static bool Satisfied(GraphemeInValuesCriterion criterion, string grapheme) {
			int length;
			UnicodeCategory category = UnicodeUtils.GetGraphemeInfo(grapheme, 0, out length);

			return criterion.IsSatisfiedBy(grapheme, 0, length, category);
		}

		[Fact] public void GraphemeInValuesCriterion_should_be_satisfied_by_provided_value() {
			Satisfied(new GraphemeInValuesCriterion(new string[] { "a" }), "a")
			.Should().BeTrue();
		}

		[Fact] public void GraphemeInValuesCriterion_should_not_be_satisfied_by_unprovided_value() {
			Satisfied(new GraphemeInValuesCriterion(new string[] { "a" }), "b")
			.Should().BeFalse();
		}

		[Fact] public void GraphemeInValuesCriterion_should_be_satisfied_by_any_provided_value() {
			var criterion = new GraphemeInValuesCriterion(new string[] { "a", "b" });
			Satisfied(criterion, "a").Should().BeTrue();
			Satisfied(criterion, "b").Should().BeTrue();
			Satisfied(criterion, "c").Should().BeFalse();
		}

		[Fact] public void GraphemeInValuesCriterion_should_not_be_satisfied_by_superstring_of_provided_grapheme() {
			Satisfied(new GraphemeInValuesCriterion(new string[] { "a" }), "a\u030a")
			.Should().BeFalse();
		}

		[Fact] public void GraphemeInValuesCriterion_should_not_be_satisfied_by_substring_of_provided_grapheme() {
			Satisfied(new GraphemeInValuesCriterion(new string[] { "a\u030a" }), "a")
			.Should().BeFalse();
		}
	}
}
