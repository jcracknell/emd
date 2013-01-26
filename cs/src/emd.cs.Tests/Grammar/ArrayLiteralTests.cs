using FluentAssertions;
using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Grammar {
	public class ArrayLiteralTests : GrammarTestFixture {
		[Fact] public void ArrayLiteralExpression_should_match_empty_array_literal() {
			var match = EmdGrammar.ArrayLiteralExpression.ShouldMatchAllOf("[]");

			match.Product.ShouldBeEquivalentTo(new ArrayLiteralExpression(new IExpression[0], new SourceRange(0,2,1,0)));
		}

		[Fact] public void ArrayLiteralExpression_should_match_array_literal_containing_only_elided_entries() {
			var match = EmdGrammar.ArrayLiteralExpression.ShouldMatchAllOf("[,,,]");

			match.Product.ShouldBeEquivalentTo(new ArrayLiteralExpression(new IExpression[0], new SourceRange(0,5,1,0)));
		}

		[Fact] public void ArrayLiteralExpression_should_match_single_NumericLiteral_entry() {
			var match = EmdGrammar.ArrayLiteralExpression.ShouldMatchAllOf("[42]");

			match.Product.ShouldBeEquivalentTo(new ArrayLiteralExpression(
				new IExpression[] { new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)) },
				new SourceRange(0,4,1,0)
			));	
		}

		[Fact] public void ArrayLiteralExpression_should_match_single_NumericLiteral_entry_with_leading_elided_entries() {
			var match = EmdGrammar.ArrayLiteralExpression.ShouldMatchAllOf("[,,,42]");

			match.Product.ShouldBeEquivalentTo(
				new ArrayLiteralExpression(
					new IExpression[] {
						new ElidedExpression(new SourceRange(1,0,1,1)),
						new ElidedExpression(new SourceRange(2,0,1,2)),
						new ElidedExpression(new SourceRange(3,0,1,3)),
						new NumericLiteralExpression(42d, new SourceRange(4,2,1,4))
					},
					new SourceRange(0,7,1,0)
				)
			);
		}

		[Fact] public void ArrayLiteralExpression_should_match_single_NumericLiteral_entry_with_trailing_elided_entries() {
			var match = EmdGrammar.ArrayLiteralExpression.ShouldMatchAllOf("[42,,,]");

			match.Product.ShouldBeEquivalentTo(new ArrayLiteralExpression(
				new IExpression[] { new NumericLiteralExpression(42d, new SourceRange(1,2,1,1)) },
				new SourceRange(0,7,1,0)
			));
		}

		[Fact] public void ArrayLiteralExpression_should_match_complex_case() {
			var match = EmdGrammar.ArrayLiteralExpression.ShouldMatchAllOf(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
				@"[ ,, , 'foo',, @bar, ,true   ,]"
			);

			match.Product.ShouldBeEquivalentTo(new ArrayLiteralExpression(
				new IExpression[] {
					new ElidedExpression(new SourceRange(1,0,1,1)),
					new ElidedExpression(new SourceRange(3,0,1,3)),
					new ElidedExpression(new SourceRange(4,0,1,4)),
					new StringLiteralExpression("foo", new SourceRange(7,5,1,7)),
					new ElidedExpression(new SourceRange(13,0,1,13)),
					new IdentifierExpression("bar", new SourceRange(16,3,1,16)),
					new ElidedExpression(new SourceRange(20,0,1,20)),
					new BooleanLiteralExpression(true, new SourceRange(22,4,1,22))
				},
				new SourceRange(0,31,1,0)
			));
		}
	}
}
