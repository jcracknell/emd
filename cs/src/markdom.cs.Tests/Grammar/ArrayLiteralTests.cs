using FluentAssertions;
using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Grammar {
	public class ArrayLiteralTests : GrammarTestFixture {
		[Fact] public void ArrayLiteralExpression_should_match_empty_array_literal() {
			var expected = new ArrayLiteralExpression(new IExpression[0], new SourceRange(0,2,1,0));

			var match = Grammar.ArrayLiteralExpression.ShouldMatch("[]");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ArrayLiteralExpression_should_match_array_literal_containing_only_elided_entries() {
			var expected = new ArrayLiteralExpression(new IExpression[0], new SourceRange(0,5,1,0));
			var match = Grammar.ArrayLiteralExpression.ShouldMatch("[,,,]");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ArrayLiteralExpression_should_match_single_NumericLiteral_entry() {
			var expected = new ArrayLiteralExpression(
				new IExpression[] { new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)) },
				new SourceRange(0,4,1,0)
			);

			var match = Grammar.ArrayLiteralExpression.ShouldMatch("[42]");

			match.Product.ShouldBeEquivalentTo(expected);	
		}

		[Fact] public void ArrayLiteralExpression_should_match_single_NumericLiteral_entry_with_leading_elided_entries() {
			var expected = new ArrayLiteralExpression(
				new IExpression[] {
					null,
					null,
					null,
					new NumericLiteralExpression(42d, new SourceRange(4,2,1,4))
				},
				new SourceRange(0,7,1,0)
			);

			var match = Grammar.ArrayLiteralExpression.ShouldMatch("[,,,42]");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ArrayLiteralExpression_should_match_single_NumericLiteral_entry_with_trailing_elided_entries() {
			var expected = new ArrayLiteralExpression(
				new IExpression[] { new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)) },
				new SourceRange(0,7,1,0)
			);

			var match = Grammar.ArrayLiteralExpression.ShouldMatch("[42,,,]");

			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void ArrayLiteralExpression_should_match_complex_case() {
			var expected = new ArrayLiteralExpression(
				new IExpression[] {
					null,
					null,
					null,
					new StringLiteralExpression("foo", new SourceRange(7,5,1,7)),
					null,
					new IdentifierExpression("bar", new SourceRange(16,3,1,16)),
					null,
					new BooleanLiteralExpression(true, new SourceRange(22,4,1,22))
				},
				new SourceRange(0,31,1,0)
			);

			var match = Grammar.ArrayLiteralExpression.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"[ ,, , 'foo',, @bar, ,true   ,]"
			);

			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
