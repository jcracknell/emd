using FluentAssertions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class SymbolTests : GrammarTestFixture {
		[Fact] public void Symbol_matches_asterix() {
			var expected = new SymbolNode("*", new SourceRange(0, 1, 1, 0));

			var match = MarkdomGrammar.Symbol.ShouldMatch("*");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
