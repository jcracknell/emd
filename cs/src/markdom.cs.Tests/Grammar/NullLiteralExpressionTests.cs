using FluentAssertions;
using markdom.cs.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class NullLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void NullLiteralExpression_matches_null() {
			var match = MarkdomGrammar.NullLiteralExpression.ShouldMatch("null");
		}

		[Fact] public void NullLiteralExpression_does_not_match_NULL() {
			MarkdomGrammar.NullLiteralExpression.ShouldNotMatch("NULL");
		}
	}
}
