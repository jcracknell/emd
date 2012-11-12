using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class CallExpressionTests : GrammarTestFixture {
		[Fact] public void CallExpression_matches_identifier_body_with_no_arguments() {
			var expected = new CallExpression(
				new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
				new IExpression[0],
				new SourceRange(1,5,1,1)
			);

			var match = Grammar.CallExpression.ShouldMatch("@foo()");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void CallExpression_matches_identifier_body_with_arguments() {
			var expected = new CallExpression(
				new IdentifierExpression("foo", new SourceRange(1,3,1,1)),
				new IExpression[] {
					new NumericLiteralExpression(1d, new SourceRange(5,1,1,5)),
					new StringLiteralExpression("2", new SourceRange(7,3,1,7))
				},
				new SourceRange(1,10,1,1)
			);

			var match = Grammar.CallExpression.ShouldMatch("@foo(1,'2')");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
