using FluentAssertions;
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
	public class ObjectLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void ObjectLiteralExpression_matches_empty_object() {
			var expected = new ObjectLiteralExpression(
				new PropertyAssignment[0],
				new SourceRange(0, 2, 1, 0));

			var match = Grammar.ObjectLiteralExpression.ShouldMatch("{}");
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ObjectLiteralExpression_matches_object_with_string_propertyname() {
			var expected = new ObjectLiteralExpression(
				new PropertyAssignment[] {
					new PropertyAssignment(
						new StringLiteralExpression("a", new SourceRange(2, 3, 1, 2)),
						new StringLiteralExpression("foo", new SourceRange(8, 5, 1, 8)),
						new SourceRange(2, 11, 1, 2)) },
				new SourceRange(0, 15, 1, 0));  

			var match = Grammar.ObjectLiteralExpression.ShouldMatch("{ 'a' : 'foo' }");
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ObjectLiteralExpression_should_match_numeric_propertyname() {
			var expected = new ObjectLiteralExpression(
				new PropertyAssignment[] {
					new PropertyAssignment(
						new NumericLiteralExpression(0d, new SourceRange(1,1,1,1)),
						new StringLiteralExpression("a", new SourceRange(3,3,1,3)),
						new SourceRange(1,5,1,1)
					),
					new PropertyAssignment(
						new NumericLiteralExpression(1d, new SourceRange(7,1,1,7)),
						new StringLiteralExpression("b", new SourceRange(9,3,1,9)),
						new SourceRange(7,5,1,7)
					)
				},
				new SourceRange(0,13,1,0)
			);

			var match = Grammar.ObjectLiteralExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"{0:'a',1:'b'}");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ObjectLiteralExpression_should_match_identifier_propertyname() {
			var expected = new ObjectLiteralExpression(
				new PropertyAssignment[] {
					new PropertyAssignment(
						new IdentifierExpression("foo", new SourceRange(2,3,1,2)),
						new NumericLiteralExpression(1d, new SourceRange(7,1,1,7)),
						new SourceRange(2,6,1,2)
					),
					new PropertyAssignment(
						new IdentifierExpression("bar", new SourceRange(10,3,1,10)),
						new NumericLiteralExpression(2d, new SourceRange(15,1,1,15)),
						new SourceRange(10,6,1,10)
					),
				},
				new SourceRange(0,18,1,0)
			);

			var match = Grammar.ObjectLiteralExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"{ foo: 1, bar: 2 }");

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
