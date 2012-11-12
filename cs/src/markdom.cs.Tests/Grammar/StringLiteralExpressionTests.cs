﻿using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	public class StringLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void StringExpression_matches_single_quoted_string() {
			var matchResult = Grammar.StringLiteralExpression.ShouldMatch("'string'");

			matchResult.Product.Value.Should().Be("string");
		}

		[Fact] public void StringExpression_matches_double_quoted_string() {
			var expected = new StringLiteralExpression("string", new SourceRange(0, 8, 1, 0));
			var matchResult = Grammar.StringLiteralExpression.ShouldMatch(@"""string""");

			matchResult.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
