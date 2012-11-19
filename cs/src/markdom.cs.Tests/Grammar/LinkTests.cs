using FluentAssertions;
using markdom.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class LinkTests : GrammarTestFixture {
		[Fact] public void Link_matches_explicit_link() {
			var matchResult = MarkdomGrammar.Link.ShouldMatch("[Slashdot: News for nerds, stuff that matters](http://slashdot.org)");

			matchResult.Succeeded.Should().BeTrue();
			matchResult.Product.Arguments.Should().NotBeEmpty();
		}

		[Fact] public void Link_matches_hybrid_link() {
			var match = MarkdomGrammar.Link.ShouldMatch(
				"[Slashdot: News for nerds, stuff that matters][slashdot.org](http://slashdot.org)");

			match.Succeeded.Should().BeTrue();
			match.Product.Arguments.Should().NotBeEmpty();
		}

		[Fact] public void Link_matches_reference_link() {
			var match = MarkdomGrammar.Link.ShouldMatch(
				"[Slashdot: News for nerds, stuff that matters][slashdot.org]");

			match.Succeeded.Should().BeTrue();
			match.Product.Arguments.Should().BeEmpty();
		}
	}
}
