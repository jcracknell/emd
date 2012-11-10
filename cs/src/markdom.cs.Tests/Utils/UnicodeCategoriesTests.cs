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
	}
}
