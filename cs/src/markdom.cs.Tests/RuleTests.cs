using markdom.cs.DOM;
using markdom.cs.DOM.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs {
	[TestClass]
	public class RuleTests {
		private readonly MarkdomGrammar Grammar = new MarkdomGrammar();

		private IExpressionMatchingResult Match(IExpression expression, params string[] input) {
			var context = new ExpressionMatchingContext(string.Join("", input));
			return expression.Match(context);
		}

		[TestMethod]
		public void AutoLink_matches_uri_only() {
			var expected = new AutoLinkNode(
				new UriExpression("http://www.google.com", new SourceRange(0, 23, 1, 0)),
				new Expression[0],
				new SourceRange(0, 23, 1, 0));

			var matchResult =
				Match(Grammar.AutoLink,
					//0....:....0....:....0....:....0....:....0....:....0....:....0
					@"<http://www.google.com>");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void AutoLink_matches_with_arguments() {
			var expected = new AutoLinkNode(
				new UriExpression("http://slashdot.org", new SourceRange(0, 21, 1, 0)),
				new Expression[] { new StringExpression("title", new SourceRange(22, 7, 1, 22)) },
				new SourceRange(0, 30, 1, 0)); 

			var matchResult =
				Match(Grammar.AutoLink,
					//0....:....0....:....0....:....0....:....0....:....0....:....0
					@"<http://slashdot.org>('title')");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
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
		public void ObjectExpression_matches_empty_object() {
			var expected = new ObjectExpression(new PropertyAssignment[0], new SourceRange(0, 2, 1, 0));

			var matchResult = Match(Grammar.ObjectExpression, "{}");

			Assert.IsTrue(matchResult.Succeeded);
			Assert.AreEqual(expected, matchResult.Product);
		}

		[TestMethod]
		public void ObjectExpression_matches_object_with_string_propertyname() {
			var expected = new ObjectExpression(
				new PropertyAssignment[] {
					new PropertyAssignment(
						new StringExpression("a", new SourceRange(2, 3, 1, 2)),
						new StringExpression("foo", new SourceRange(9, 5, 1, 9)),
						new SourceRange(2, 12, 1, 2)) },
				new SourceRange(0, 16, 1, 0));  

			var matchResult = Match(Grammar.ObjectExpression, "{ 'a' => 'foo' }");

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
		public void StringExpression_matches_single_quoted_string() {
			var matchResult = Match(Grammar.StringExpression, "'string'");

			Assert.IsTrue(matchResult.Succeeded);

			var stringExpression = matchResult.Product as StringExpression;

			Assert.IsNotNull(stringExpression);
			Assert.AreEqual("string", stringExpression.Value);
		}

		[TestMethod]
		public void StringExpression_matches_double_quoted_string() {
			var expected = new StringExpression("string", new SourceRange(0, 8, 1, 0));
			var matchResult = Match(Grammar.StringExpression, @"""string""");

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
		public void UriExpression_matches_remote_uri() {
			var matchResult = Match(Grammar.UriExpression, "http://www.google.com");

			Assert.IsTrue(matchResult.Succeeded);
		}

		[TestMethod]
		public void Bla() {
			var input = new ExpressionMatchingContext(
@" // This is a single line comment
# Heading 1
This is a [paragraph] with **bold text**.

## Heading 2

Another paragraph.

 * This is a tight list.
 * With two items, one of which is spread
across several lines.

Yet another.

+==============+
|2c= The Title |
+=======+======+
|=   a  |=  b  |
+=======+======+
|     1 |    2 |
+-------+------+

* Loose item 1.
* Loose item 2.

* Loose item 3.

    With a second paragraph.");
			
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			var matchResult = Grammar.Document.Match(input);
			stopwatch.Stop();

			stopwatch.ToString();
			Assert.IsTrue(matchResult.Succeeded);
		}
	}
}
