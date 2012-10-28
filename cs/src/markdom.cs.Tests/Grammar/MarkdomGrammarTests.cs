using Microsoft.VisualStudio.TestTools.UnitTesting;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	[TestClass]
	public class MarkdomGrammarTests {
		private readonly MarkdomGrammar Grammar = new MarkdomGrammar();

		private IMatchingResult Match(IParsingExpression expression, string s) {
			var matchContext = new MatchingContext(s);
			return expression.Match(matchContext);
		}

		[TestMethod]
		public void Strong_base_case() {
			var result = Match(Grammar.Strong, "**text**");

			Assert.IsTrue(result.Succeeded);
		}
	}
}
