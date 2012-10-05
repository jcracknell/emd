using Microsoft.VisualStudio.TestTools.UnitTesting;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFM.Grammar {
	[TestClass]
	public class JFMGrammarTests {
		private readonly JfmGrammar Grammar = new JfmGrammar();

		private IExpressionMatchingResult Match(IExpression expression, string s) {
			var matchContext = new ExpressionMatchingContext(s);
			return expression.Match(matchContext);
		}

		[TestMethod]
		public void Strong_base_case() {
			var result = Match(Grammar.Strong, "**text**");

			Assert.IsTrue(result.Succeeded);
		}
	}
}
