using FluentAssertions;
using emd.cs.Expressions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar
{
	public class AutoLinkTests : GrammarTestFixture {
		[Fact] public void AutoLink_matches_uri_only() {
			var expected = new AutoLinkNode(
				"http://www.google.com",
				new IExpression[0],
				new SourceRange(0, 23, 1, 0));

			var match = MarkdomGrammar.AutoLink.ShouldMatch(
					//0....:....0....:....0....:....0....:....0....:....0....:....0
					@"<http://www.google.com>");


			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void AutoLink_matches_with_arguments() {
			var expected = new AutoLinkNode(
				"http://slashdot.org",
				new IExpression[] { new StringLiteralExpression("title", new SourceRange(22, 7, 1, 22)) },
				new SourceRange(0, 30, 1, 0));

			var match = MarkdomGrammar.AutoLink.ShouldMatch(
				//0....:....0....:....0....:....0....:....0....:....0....:....0
					@"<http://slashdot.org>('title')");

			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
