using Microsoft.VisualStudio.TestTools.UnitTesting;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	public static class RuleTestingExtensionMethods {
		public static IMatchingResult Match(this IParsingExpression expression, params string[] input) {
			var context = new MatchingContext(string.Join("", input));
			var match = expression.Matches(context);
			return match;
		}

		public static IMatchingResult AssertMatch(this IParsingExpression expression, params string[] input) {
			var match = expression.Match(input);
			Assert.IsTrue(match.Succeeded, "Expression does not match.\nExpression:\n" + expression.ToString() + "\nInput:\n" + string.Join("", input));
			return match;
		}

		public static IMatchingResult AssertNoMatch(this IParsingExpression expression, params string[] input) {
			var match = expression.Match(input);
			Assert.IsTrue(match.Succeeded, "Expression matches.\nExpression:\n" + expression.ToString() + "\nInput:\n" + string.Join("", input));
			return match;
		}
	}
}
