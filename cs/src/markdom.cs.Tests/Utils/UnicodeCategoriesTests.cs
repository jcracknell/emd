using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Utils {
	public class UnicodeCategoriesTests {
		[Fact] public void UnicodeCategories_should_contain_all_codepoints() {
			UnicodeCategories.All.Should().HaveCount(24429);
			UnicodeCategories.All.Distinct().Should().HaveCount(24429);
		}

		[Fact] public void Unicode_experimentation() {
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			int[] codepointCountByFirstNibble = new int[16];
			foreach(var cp in UnicodeCategories.All.Where(cp => !cp.IsSurrogatePair))
				codepointCountByFirstNibble[cp.FirstCodeUnit >> 12]++;

			var surrogatePairCountsByFirstCodeUnit =
				UnicodeCategories.All
				.Where(cp => cp.IsSurrogatePair)
				.GroupBy(cp => cp.FirstCodeUnit)
				.ToDictionary(g => g.Key, g => g.Count());


			stopwatch.Stop();
			stopwatch.ToString();
		}
	}
}
