using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class UriExpressionTests : GrammarTestFixture {
		[Fact] public void UriExpression_matches_remote_uri() {
			EmdGrammar.UriLiteralExpression.ShouldMatchAllOf("http://www.google.com");
		}

		[Fact] public void UriExpression_matches_uri_with_balanced_parentheses() {
			var match = EmdGrammar.UriLiteralExpression.ShouldMatchAllOf("http://msdn.microsoft.com/en-us/library/a6td98xe(v=vs.71).aspx");

			match.Product.Value.Should().Be("http://msdn.microsoft.com/en-us/library/a6td98xe(v=vs.71).aspx");
		}

		[Fact] public void UriExpression_discards_characters_following_unbalanced_parentheses() {
			var match = EmdGrammar.UriLiteralExpression.ShouldMatchSomeOf("http://msdn.microsoft.com/en-us/library/a6td98xev=vs.71).aspx");

			match.Product.Value.Should().Be("http://msdn.microsoft.com/en-us/library/a6td98xev=vs.71");
		}
	}
}
