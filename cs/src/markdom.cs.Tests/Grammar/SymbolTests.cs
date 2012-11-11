﻿using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class SymbolTests : GrammarTestFixture {
		[Fact] public void Symbol_matches_asterix() {
			var expected = new SymbolNode("*", new MarkdomSourceRange(0, 1, 1, 0));

			var match = Grammar.Symbol.ShouldMatch("*");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}