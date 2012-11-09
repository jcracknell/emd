using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace markdom.cs.Grammar {
	public static class RuleTestingExtensionMethods {
		public static IMatchingResult<TProduct> ShouldMatch<TProduct>(this IParsingExpression<TProduct> expression, params string[] input) {
			var context = new MatchingContext(string.Join("", input));
			var match = expression.Matches(context);

			match.Succeeded.Should().BeTrue();

			return match;
		}

		public static IMatchingResult<TProduct> ShouldNotMatch<TProduct>(this IParsingExpression<TProduct> expression, params string[] input) {
			var context = new MatchingContext(string.Join("", input));
			var match = expression.Matches(context);

			match.Succeeded.Should().BeFalse();

			return match;
		}
	}
}
