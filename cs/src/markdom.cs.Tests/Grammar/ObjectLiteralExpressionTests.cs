using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar {
	class ObjectLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void ObjectLiteralExpression_matches_empty_object() {
			var expected = new ObjectLiteralExpression(
				new PropertyAssignment[0],
				new MarkdomSourceRange(0, 2, 1, 0));

			var match = Grammar.ObjectLiteralExpression.ShouldMatch("{}");
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ObjectLiteralExpression_matches_object_with_string_propertyname() {
			var expected = new ObjectLiteralExpression(
				new PropertyAssignment[] {
					new PropertyAssignment(
						new StringLiteralExpression("a", new MarkdomSourceRange(2, 3, 1, 2)),
						new StringLiteralExpression("foo", new MarkdomSourceRange(8, 5, 1, 8)),
						new MarkdomSourceRange(2, 11, 1, 2)) },
				new MarkdomSourceRange(0, 15, 1, 0));  

			var match = Grammar.ObjectLiteralExpression.ShouldMatch("{ 'a' : 'foo' }");
			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
