using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace emd.cs.Grammar {
	public static class RuleTestingExtensionMethods {
		public static IMatchingResult<TProduct> ShouldMatchAllOf<TProduct>(this IParsingExpression<TProduct> expression, params string[] input) {
			var joinedInput = string.Join("", input);
			var context = new MatchingContext(joinedInput);
			var match = expression.Matches(context);

			if(!match.Succeeded)
				Fail("Match failed, expected success.");

			if(match.Length != joinedInput.Length)
				Fail("Expression did not match all of input, expected full match.");

			return match;
		}

		public static IMatchingResult<TProduct> ShouldMatchSomeOf<TProduct>(this IParsingExpression<TProduct> expression, params string[] input) {
			var joinedInput = string.Join("", input);
			var context = new MatchingContext(joinedInput);
			var match = expression.Matches(context);

			if(!match.Succeeded)
				Fail("Match failed, expected success.");

			if(match.Length == joinedInput.Length)
				Fail("Expression matched all of input, expected partial match.");

			if(0 == match.Length)
				Fail("Expresion match reported length 0.");

			return match;
		}

		public static IMatchingResult<TProduct> ShouldNotMatch<TProduct>(this IParsingExpression<TProduct> expression, params string[] input) {
			var context = new MatchingContext(string.Join("", input));
			var match = expression.Matches(context);

			if(match.Succeeded)
				Fail("Match succeeded, expected failure.");

			return match;
		}

		private static Exception Fail(string message) {
			throw new Xunit.Sdk.AssertException(message);
		}
	}
}
