using JFM.DOM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFM {
	[TestClass]
	public class RuleTests {
		private readonly JfmGrammar Grammar = new JfmGrammar();

		private IExpressionMatchingResult Match(IExpression expression, params string[] input) {
			var context = new ExpressionMatchingContext(string.Join("", input));
			return expression.Match(context);
		}

		[TestMethod]
		public void CommentBlock_matches_single_line_comment() {
			var expected = new SingleLineCommentNode(" text", new SourceRange(0, 7, 1, 0));

			var matchResult = Match(Grammar.CommentBlock, "// text");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void CommentBlock_matches_indented_single_line_comment() {
			var expected = new SingleLineCommentNode(" text", new SourceRange(4, 7, 1, 4));
			var matchResult =
				Match(Grammar.CommentBlock,
					"    // text");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void CommentBlock_does_not_match_multi_line_comment_with_trailing_content() {
			var matchResult =
				Match(Grammar.CommentBlock, 
					"/* This is a multi-line comment\n",
					"with trailing content */ text");

			Assert.IsFalse(matchResult.Succeeded);
		}

		[TestMethod]
		public void Entity_matches_decimal_html_entity() {
			var expected = new EntityNode(233, new SourceRange(0, 6, 1, 0));

			var matchResult = Match(Grammar.Entity, "&#233;");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Entity_matches_hexadecimal_html_entity() {
			var expected = new EntityNode(233, new SourceRange(0, 6, 1, 0));
			var matchResult = Match(Grammar.Entity, "&#xE9;");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Entity_matches_named_html_entity() {
			var expected = new EntityNode(233, new SourceRange(0, 8, 1, 0));

			var matchResult = Match(Grammar.Entity, "&eacute;");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Emphasis_matches_base_case() {
			var input = new ExpressionMatchingContext("*text*");
			var expected = new EmphasisNode(
				new Node[] {
					new TextNode("text", new SourceRange(1, 4, 1, 1)) },
				new SourceRange(0, 6, 1, 0));

			var matchResult = Grammar.Emphasis.Match(input);
			
			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Quoted_matches_single_quoted() {
			var input = new ExpressionMatchingContext("'text'");
			var expected = new QuotedNode(
				QuoteType.Single,
				new Node[] {
					new TextNode("text", new SourceRange(1, 4, 1, 1)) },
				new SourceRange(0, 6, 1, 0));

			var matchResult = Grammar.Quoted.Match(input);

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void SingleLineComment_matches_base_case() {
			var input = new ExpressionMatchingContext("// text");
			var expected = new SingleLineCommentNode(" text", new SourceRange(0, 7, 1, 0));

			var matchResult = Grammar.SingleLineComment.Match(input);

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Strong_matches_base_case() {
			var input = new ExpressionMatchingContext("**text**");
			var expected = new StrongNode(
				new Node[] {
					new TextNode("text", new SourceRange(2, 4, 1, 2)) },
				new SourceRange(0, 8, 1, 0));

			var matchResult = Grammar.Strong.Match(input);

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Symbol_matches_asterix() {
			var input = new ExpressionMatchingContext("*");
			var expected = new SymbolNode("*", new SourceRange(0, 1, 1, 0));

			var matchResult = Grammar.Symbol.Match(input);

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void Bla() {
			var input = new ExpressionMatchingContext(
@"
# Heading 1
This is a paragraph with **bold text**.

## Heading 2

Another paragraph.

Yet another.");
			
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			var matchResult = Grammar.Document.Match(input);
			stopwatch.Stop();

			stopwatch.ToString();
			Assert.IsTrue(matchResult.Succeeded);
		}
	}
}
