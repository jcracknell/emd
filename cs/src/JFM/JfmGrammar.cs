using JFM.DOM;
using JFM.DOM.Expressions;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM {
	public class JfmGrammar : Grammar<JfmDocument> {
		public struct LineInfo {
			public readonly string LineString;
			public readonly SourceRange SourceRange;

			public LineInfo(string lineString, SourceRange sourceRange) {
				LineString = lineString;
				SourceRange = sourceRange;
			}
		}
		
		/// <summary>
		/// A tab or a space.
		/// </summary>
		public IExpression<string> SpaceChar { get; private set; }
		public IExpression<string> SpaceChars { get; private set; }
		/// <summary>
		/// A whitespace character; space, tab or newline.
		/// </summary>
		public IExpression<string> Whitespace { get; private set; }
		public IExpression<string> Whitespaces { get; private set; }
		/// <summary>
		/// A newline character.
		/// </summary>
		public IExpression<string> NewLine { get; private set; }
		public IExpression<string> SpecialChar { get; private set; }
		public IExpression<string> NormalChar { get; private set; }
		public IExpression<string> Indent { get; private set; }
		public IExpression<string> NonIndentSpace { get; private set; }
		/// <summary>
		/// A raw line of input, including the newline character.
		/// </summary>
		public IExpression<LineInfo> Line { get; private set; }
		/// <summary>
		/// A blank line; composed of any number of spaces followed by a line end.
		/// </summary>
		public IExpression<LineInfo[]> BlankLines { get; private set; }
		public IExpression<LineInfo> BlankLine { get; private set; }
		public IExpression<string> Digit { get; private set; }
		public IExpression<string> HexDigit { get; private set; }
		public IExpression<string> EnglishLowerAlpha { get; private set; }
		public IExpression<string> EnglishUpperAlpha { get; private set; }
		public IExpression<string> EnglishAlpha { get; private set; }
		public IExpression<Node> Comment { get; private set; }
		/// <summary>
		/// A C-style multi-line comment.
		/// </summary>
		public IExpression<MultiLineCommentNode> MultiLineComment { get; private set; }
		/// <summary>
		/// A C-style single line comment. Does not consume the end of the line
		/// </summary>
		public IExpression<SingleLineCommentNode> SingleLineComment { get; private set; }

		public IExpression<JfmDocument> Document { get; private set; }

		public IExpression<Node[]> Blocks { get; private set; }
		public IExpression<Node> Block { get; private set; }

		public IExpression<Node> CommentBlock { get; private set; }
		public IExpression<HeadingNode> Heading { get; private set; }
		public IExpression<UnorderedListNode> UnorderedList { get; private set; }
		public IExpression<UnorderedListNode> UnorderedListTight { get; private set; }
		public IExpression<UnorderedListNode> UnorderedListLoose { get; private set; }
		public IExpression<ParagraphNode> Paragraph { get; private set; }
		public IExpression<LineInfo> NonEmptyBlockLine { get; private set; }
		public IExpression<LineInfo> BlockLine { get; private set; }

		public IExpression<Node[]> Inlines { get; private set; }
		public IExpression<Node> Inline { get; private set; }
		public IExpression<AutoLinkNode> AutoLink { get; private set; }
		public IExpression<LinkNode> Link { get; private set; }
		public IExpression<StrongNode> Strong { get; private set; }
		public IExpression<EmphasisNode> Emphasis { get; private set; }
		public IExpression<QuotedNode> Quoted { get; private set; }
		public IExpression<LineBreakNode> LineBreak { get; private set; }
		public IExpression<TextNode> Text { get; private set; }
		public IExpression<SpaceNode> Space { get; private set; }
		public IExpression<EntityNode> Entity { get; private set; }

		public IExpression<ReferenceId> ReferenceId { get; private set; }

		/// <summary>
		/// A Symbol, an unescaped special character which was not parsed into a valid node.
		/// </summary>
		public IExpression<SymbolNode> Symbol { get; private set; }


		public IExpression<Expression> Expression { get; private set; }
		public IExpression<Expression[]> ArgumentList { get; private set; }
		public IExpression<ObjectExpression> ObjectExpression { get; private set; }
		public IExpression<ObjectExpression> ObjectBodyExpression { get; private set; }
		public IExpression<StringExpression> StringExpression { get; private set; }
		public IExpression<UriExpression> UriExpression { get; private set; }
		public IExpression<Nil> ExpressionWhitespace { get; private set; }

		private IExpression<TProduct> DelimitedInline<TDelimiter, TContent, TProduct>(IExpression<TDelimiter> delimiter, IExpression<TContent> content, Func<IExpressionMatch<object[]>, TContent[], TProduct> matchAction) {
			return Sequence(
				delimiter,
				AtLeast(0, Sequence(NotAhead(delimiter), content, (match, a, b) => b)),
				delimiter,
				(match, s, c, e) => matchAction(match, c));
		}

		public JfmGrammar() {

			Define(() => Document,
				Reference(() => Blocks, match => new JfmDocument(match.Product)));

			#region Comments

			Define(() => Comment,
				Choice<Node>(
					Reference(() => SingleLineComment),
					Reference(() => MultiLineComment)));

			Define(() => SingleLineComment,
				Sequence(
					Literal("//"),
					Reference(() => Line),
					(match, s, l) => new SingleLineCommentNode(l.LineString, match.SourceRange)));

			Define(() => MultiLineComment,
				Sequence(
					Literal("/*"),
					AtLeast(0,
						Sequence( NotAhead(Literal("*/")), Wildcard() ),
						match => match.String),
					Literal("*/"),
					(match, s, c, e) => new MultiLineCommentNode(c, match.SourceRange)));

			#endregion

			#region Block Rules

			Define(() => Blocks,
				AtLeast(0, Reference(() => Block)));

			Define(() => Block,
				Sequence(
					Reference(() => BlankLines),
					OrderedChoice<Node>(
						Reference(() => Heading),
						Reference(() => UnorderedList),
						Reference(() => CommentBlock),
						Reference(() => Paragraph)), // paragraph must come last
					Reference(() => BlankLines),
					(match, a, b, c) => b));

			var singleLineCommentBlock =
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => SingleLineComment),
					Reference(() => BlankLines),
					(match, a, b, c) => b);

			var multiLineCommentBlock =
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => MultiLineComment),
					Reference(() => BlankLine), // no trailing content (don't match a para w/ starting mlc)
					Reference(() => BlankLines),
					(match, a, b, c, d) => b);

			Define(() => CommentBlock,
				Choice<Node>(singleLineCommentBlock, multiLineCommentBlock));

			#region Lists
			// We define two types of lists, a *tight* list wherein no items are separated by blank
			// lines which causes each item to be parsed as a single block of inlines, and a
			// *loose* list, where items may contain multiple blocks separated by blank lines.

			// A list item continues if it is followed by any number of blank lines and an indented line.
			// The indented line is not empty because BlankLines would have consumed it if it was.
			var listItemContinues =
				Sequence(
					Reference(() => BlankLines),
					Ahead(Reference(() => Indent)),
					(match, a, b) => a);

			// We define a custom BlockLine for lists which discards any indent at the beginning of a line
			var listBlockLine =
				Sequence(
					Optional(Reference(() => Indent)),
					Reference(() => NonEmptyBlockLine),
					(match, a, b) => b);

			#region Unordered List

			// We first attempt to parse a tight list, because a loose list is defined as 'one that
			// is not tight'.
			Define(() => UnorderedList,
				OrderedChoice(
					Reference(() => UnorderedListTight),
					Reference(() => UnorderedListLoose)));

			var bullet =
				Sequence(
					Reference(() => NonIndentSpace),
					Choice(new string[] { "*", "-", "+" }.Select(Literal).ToArray()),
					Reference(() => SpaceChars));

			var unorderedListBlockLine =
				Sequence(
					NotAhead(bullet), listBlockLine,
					(match, a, b) => b);

			var unorderedListBlockLines = AtLeast(0, unorderedListBlockLine);

			var unorderedListItemInitialBlock =
				Sequence(
					bullet, Reference(() => BlockLine), // Allow for an empty list item
					unorderedListBlockLines,
					(match, b, fl, ls) => ArrayUtils.Prepend(fl, ls));

			var unorderedListItemSubsequentBlock =
				Sequence(
					listItemContinues,
					unorderedListBlockLines,
					Reference(() => BlankLines),
					(match, a, b, c) => ArrayUtils.Combine(a, b, c));

			var unorderedListItemTight =
				Reference(
					() => unorderedListItemInitialBlock,
					match => new UnorderedListItemNode(ParseLines(Inlines, match.Product), match.SourceRange));

			var unorderedListItemLoose =
				Sequence(
					unorderedListItemInitialBlock,
					AtLeast(0, unorderedListItemSubsequentBlock, match => ArrayUtils.Flatten(match.Product)),
					Reference(() => BlankLines), // chew up any blank lines after an initial block with no subsequent
					(match, a, b, c) => new UnorderedListItemNode(ParseLines(Blocks, ArrayUtils.Combine(a, b)), match.SourceRange));

			var unorderedListContinuesLoose =
				Choice(new IExpression[] {
					 Sequence(Reference(() => BlankLines), bullet),
					 listItemContinues });;

			Define(() => UnorderedListTight,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(1, unorderedListItemTight),
					NotAhead(unorderedListContinuesLoose),
					(match, a, lis, c) => new UnorderedListNode(lis, match.SourceRange)));

			Define(() => UnorderedListLoose,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(1, unorderedListItemLoose),
					(match, a, lis) => new UnorderedListNode(lis, match.SourceRange)));

			#endregion

			#endregion

			Define(() => Heading,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Literal("#"), match => match.Product.Length),
					Reference(() => SpaceChars),
					Reference(() => BlockLine),
					(match, s, l, s2, c) => new HeadingNode(c.LineString, l, match.SourceRange)));

			Define(() => Paragraph,
				AtLeast(1,
					Reference(() => NonEmptyBlockLine),
					match => new ParagraphNode(ParseLines(Inlines, match.Product), match.SourceRange)));

			Define(() => NonEmptyBlockLine,
				Sequence(
					NotAhead(Reference(() => BlankLine)),
					Reference(() => BlockLine),
					(match, a, b) => b));

			Define(() => BlockLine,
				Sequence(
					new IExpression[] {
						AtLeast(0, // must consume something (no empty line)
							Sequence(NotAhead(Reference(() => NewLine)), Wildcard())),
						Optional(Reference(() => NewLine))
					}, match => new LineInfo(match.String, match.SourceRange)));
			#endregion

			#region Inline Rules

			Define(() => Inlines,
				AtLeast(0, Reference(() => Inline)));

			Define(() => Inline,
				OrderedChoice<Node>(
					Reference(() => Text),
					Reference(() => Space),
					Reference(() => Strong),
					Reference(() => Emphasis),
					Reference(() => Quoted),
					Reference(() => Link),
					Reference(() => AutoLink),
					Reference(() => Comment),
					Reference(() => Entity),
					Reference(() => LineBreak),
					Reference(() => Symbol)));

			#region Link

			Define(() => AutoLink,
				Sequence(
					Sequence(
						Ahead(Literal("<")), // do not match undelimited UriExpression
						Reference(() => UriExpression),
						(match, a, u) => u),
					Optional(Reference(() => ArgumentList)),
					(match, u, args) => new AutoLinkNode(u, args ?? new Expression[0], match.SourceRange)));

			var linkLabel =
				Sequence(
					Literal("["),
					AtLeast(0, Sequence(NotAhead(Literal("]")), Reference(() => Inline), (match, a, b) => b)),
					Literal("]"),
					(match, s, c, e) => c);

			Define(() => Link,
				Sequence(
					linkLabel,
					Reference(() => SpaceChars),
					Optional(Reference(() => ArgumentList)),
					(match, l, s, a) => new LinkNode(l, null, match.SourceRange)));

			#endregion

			Define(() => Strong,
				Sequence(
					Literal("**"),
					AtLeast(0, Sequence(NotAhead(Literal("**")), Reference(() => Inline), (match, a, b) => b)),
					Optional(Literal("**")),
					(match, s, c, e) => new StrongNode(c, match.SourceRange)));	

			Define(() => Emphasis,
				Sequence(
					Literal("*"),
					AtLeast(0, Sequence(NotAhead(Literal("*")), Reference(() => Inline), (match, a, b) => b)),
					Optional(Literal("*")),
					(match, s, c, e) => new EmphasisNode(c, match.SourceRange)));

			var singleQuoted =
				Sequence(
					Literal("'"),
					AtLeast(0, Sequence(NotAhead(Literal("'")), Reference(() => Inline), (match, a, b) => b)),
					Literal("'"),
					(match, s, c, e) => new QuotedNode(QuoteType.Single, c, match.SourceRange));

			var doubleQuoted =
				Sequence(
					Literal("\""),
					AtLeast(0, Sequence(NotAhead(Literal("\"")), Reference(() => Inline), (match, a, b) => b)),
					Literal("\""),
					(match, s, c, e) => new QuotedNode(QuoteType.Double, c, match.SourceRange));

			Define(() => Quoted,
				OrderedChoice(doubleQuoted, singleQuoted));

			Define(() => LineBreak,
				Sequence(
					new IExpression[] { Literal(@"\\"), Reference(() => SpaceChars), Reference(() => NewLine) },
					match => new LineBreakNode(match.SourceRange)));

			var decimalHtmlEntity =
				Sequence(
					Literal("&#"),
					Between(1, 6, Reference(() => Digit), match => match.String),
					Literal(";"),
					(match, s, v, e) => new EntityNode(int.Parse(v), match.SourceRange));

			var hexHtmlEntity =
				Sequence(
					Literal("&#x"),
					Between(1, 6, Reference(() => HexDigit), match => match.String),
					Literal(";"),
					(match, s, v, e) => new EntityNode(Convert.ToInt32(v, 16), match.SourceRange));

			// Because of the large number of named entities it is much faster to use a dynamic
			// expression with an assertion to match valid entity names
			var namedHtmlEntity =
				Dynamic(() => {
					string entityName = null;
					return Sequence(
						Literal("&"),
						Between(1, 32, Reference(() => EnglishAlpha), match => { return entityName = match.String; }),
						Assert(() => EntityNode.IsEntityName(entityName)),
						Literal(";"),
						(match, s, n, a, e) => new EntityNode(EntityNode.GetNamedEntityValue(entityName), match.SourceRange));
				});

			Define(() => Entity,
				OrderedChoice(
					decimalHtmlEntity,
					hexHtmlEntity,
					namedHtmlEntity));

			Define(() => Text,
				AtLeast(1,
					Reference(() => NormalChar),
					match => new TextNode(match.String, match.SourceRange)));

			Define(() => Space,
				AtLeast(1, Reference(() => Whitespace), match => new SpaceNode(match.SourceRange)));

			Define(() => Symbol,
				Reference(() => SpecialChar, match => new SymbolNode(match.String, match.SourceRange)));

			#endregion

			Define(() => ReferenceId,
				Sequence(
					Literal("["),
					AtLeast(0,
						Sequence(
							NotAhead(OrderedChoice(Literal("]"), Reference(() => NewLine))),
							Wildcard()),
							match => match.String),
					Literal("]"),
					(match, s, i, e) => new ReferenceId(i)));

			Define(() => Line,
				Sequence(
					AtLeast(0,
						Sequence(
							NotAhead(Reference(() => NewLine)),
							Wildcard())),
					Optional(Reference(() => NewLine)),
					(match, a, b) => new LineInfo(match.String, match.SourceRange)));

			Define(() => BlankLines,
				Sequence(
					AtLeast(0,
						Sequence(
							AtLeast(0, Reference(() => SpaceChar)),
							Reference(() => NewLine),
							(match, a, b) => new LineInfo(match.String, match.SourceRange))),
					Optional(
						Sequence(
							AtLeast(0, Reference(() => SpaceChar)),
							EndOfInput(),
							(match, a, b) => new LineInfo[] { new LineInfo(match.String, match.SourceRange) })),
					(match, ls, ll) => ArrayUtils.Combine(ls, ll ?? new LineInfo[0])));

			// NEVER EVER EVER USE THIS IN A REPETITION CONTEXT
			Define(() => BlankLine,
				Sequence(
					new IExpression[] {
						AtLeast(0, Reference(() => SpaceChar)),
						Choice(
							new IExpression[] { Reference(() => NewLine), EndOfInput() }) },
					match => new LineInfo(match.String, match.SourceRange)));

			Define(() => Indent,
				Choice(Literal("\t"), Literal("    ")));

			Define(() => NonIndentSpace,
				AtMost(3, Literal(" "), match => match.String));

			Define(() => SpaceChar,
				OrderedChoice(
					Literal(" "),
					Literal("\t")));

			Define(() => SpaceChars,
				AtLeast(0, Reference(() => SpaceChar), match => match.String));

			Define(() => NewLine,
				Choice(
					Literal("\n"),
					Literal("\r\n")));

			Define(() => Whitespace,
				Choice(
					Reference(() => SpaceChar),
					Reference(() => NewLine)));

			Define(() => Whitespaces,
				AtLeast(0, Reference(() => Whitespace), match => match.String));

			Define(() => Digit,
				CharacterInRange('0', '9'));

			Define(() => HexDigit,
				OrderedChoice(
					Reference(() => Digit),
					CharacterInRange('a', 'f'),
					CharacterInRange('A', 'F')));

			Define(() => EnglishLowerAlpha,
				CharacterInRange('a', 'z'));

			Define(() => EnglishUpperAlpha,
				CharacterInRange('A', 'Z'));

			Define(() => EnglishAlpha,
				Choice(
					Reference(() => EnglishLowerAlpha),
					Reference(() => EnglishUpperAlpha)));

			Define(() => SpecialChar,
				Choice(
					new string[] { "*", "&", "'", "\"", "/", "\\", "[", "]" }
					.Select(Literal).ToArray()));

			Define(() => NormalChar,
				Sequence(
					NotAhead(
						Choice(
							Reference(() => Whitespace),
							Reference(() => SpecialChar))),
					Wildcard(),
					(match, a, b) => b.ToString()));

			#region Expressions
			// # Expressions
			//
			// JFM uses an expression syntax which is *suspiciously similar* to Javascript (in fact
			// it is designed to emulate javascript for its succinctness, familiarity, and simplicity.
			// Given the primary use cases for JFM, the addition of URI literals to the expression
			// language is a no-brainer. As a result, expression syntax differs from javascript in
			// a few key areas in order to disambiguate URI expressions from everything else.
			//
			//  * Non-literal expressions begin with `@` (conversely a URI literal can never begin with `@`,
			//    which seems like a reasonable limitation).
			//  * Object property assignment is performed using the right arrow symbol (`=>`), as in
			//    `{ foo => 'bar' }`
			// 
			// This allows us to use expressions as arguments to constructs co-opted from Markdown, e.g.
			// `[link text](http://server)` as `(http://server)` is a valid argument list.
			// Also consider: `[link text](some/relative/path, title => 'a link')`.

			// Ordering here is important, we rely on several assumptions in order to
			// implement URI literals:
			// * `StringExpression` gets first crack at quotes
			// * `UriExpression` does not start with `@`
			Define(() => Expression,
				OrderedChoice<Expression>(
					Reference(() => StringExpression),
					Reference(() => ObjectExpression),
					Reference(() => UriExpression)));
			
			var argumentSeparator =
				Sequence(
					Reference(() => ExpressionWhitespace),
					Literal(","),
					Reference(() => ExpressionWhitespace));

			// TODO: argument list accepts optional final object body expression
			var argumentListArguments =
				Optional(
					Sequence(
						Reference(() => Expression),
						AtLeast(0, Sequence( argumentSeparator, Reference(() => Expression), (match, s, e) => e)),
						(match, e, es) => ArrayUtils.Prepend(e, es)));

			Define(() => ArgumentList,
				Sequence(
					Sequence(Literal("("), Reference(() => ExpressionWhitespace)),
					argumentListArguments,
					Sequence(Reference(() => ExpressionWhitespace), Literal(")")),
					(match, s, es, e) => es ?? new Expression[0]));

			#region StringExpression
			
			var stringExpressionEscapes =
				Choice(
					Literal(@"\n", match => "\n"),
					Literal(@"\r", match => "\r"),
					Literal(@"\t", match => "\t"),
					Literal(@"\\", match => "\\"));

			var singleQuotedStringExpressionContent =
				AtLeast(0,
					Choice(
						Literal(@"\'", match => "'"),
						stringExpressionEscapes,
						Sequence(
							NotAhead(Choice(Literal("'"), Reference(() => NewLine))),
							Wildcard(),
							(match, a, c) => c)),
					match => string.Join("", match.Product));

			var doubleQuotedStringExpressionContent =
				AtLeast(0,
					Choice(
						Literal("\\\"", match => "\""),
						stringExpressionEscapes,
						Sequence(
							NotAhead(Choice(Literal("\""), Reference(() => NewLine))),
							Wildcard(),
							(match, a, c) => c)),
					match => string.Join("", match.Product));

			var singleQuotedStringExpression =
				Sequence(
					Literal("'"), singleQuotedStringExpressionContent, Literal("'"),
					(match, s, c, e) => new StringExpression(c, match.SourceRange));

			var doubleQuotedStringExpression =
				Sequence(
					Literal("\""), doubleQuotedStringExpressionContent, Literal("\""),
					(match, s, c, e) => new StringExpression(c, match.SourceRange));

			Define(() => StringExpression,
				Choice(
					singleQuotedStringExpression,
					doubleQuotedStringExpression));

			#endregion

			#region ObjectExpression

			var objectPropertyAssignment =
				Sequence(
					Reference(() => StringExpression), // TODO: Identifier / String / Uri
					Sequence(
						Reference(() => ExpressionWhitespace),
						Literal("=>"),
						Reference(() => ExpressionWhitespace)),
					Reference(() => Expression),
					(match, n, a, v) => new PropertyAssignment(n, v, match.SourceRange));

			var objectPropertyAssignments =
				Optional(
					Sequence(
						objectPropertyAssignment,
						AtLeast(0, Sequence(argumentSeparator, objectPropertyAssignment, (match, s, p) => p)),
						(match, pa, pas) => ArrayUtils.Prepend(pa, pas)));

			var objectExpressionStart =
				Sequence( Literal("{"), Reference(() => ExpressionWhitespace));

			var objectExpressionEnd =
				Sequence( Reference(() => ExpressionWhitespace), Literal("}"));

			Define(() => ObjectExpression,
				Sequence(
					objectExpressionStart, objectPropertyAssignments, objectExpressionEnd,
					(match, s, pas, e) => new ObjectExpression(pas ?? new PropertyAssignment[0], match.SourceRange)));

			Define(() => ObjectBodyExpression,
				Reference(() => objectPropertyAssignments, match => new ObjectExpression(match.Product, match.SourceRange)));

			#endregion

			#region UriExpression
			
			// TODO: handle URIs with spaces (spaces must be followed by some sensible character)

			var uriEscaped =
				Sequence(
					new IExpression[] { Literal("%"), Exactly(2, Reference(() => HexDigit)) },
					match => match.String);

			var bareUriExpressionCharacter =
				OrderedChoice(
					Reference(() => EnglishAlpha),
					Reference(() => Digit),
					Choice(
						new string[] { ";", "/", "?", ":", "@", "&", "=", "?", "+", "$", "-", "_", ".", "!", "~", "*", "'" }
						.Select(Literal).ToArray()),
					uriEscaped);
			
			var bareUriExpression =
				Sequence(
					NotAhead(Literal("@")), // may as well make this explicit
					AtLeast(1, bareUriExpressionCharacter, match => new UriExpression(match.String, match.SourceRange)),
					(match, a, b) => b);

			var delimitedUriExpressionCharacter =
				Choice(
					bareUriExpressionCharacter,
					Choice(new string[] { ",", "(", ")" }.Select(Literal).ToArray()));

			var delimitedUriExpression =
				Sequence(
					Literal("<"), AtLeast(1, delimitedUriExpressionCharacter, match => match.String), Literal(">"),
					(match, s, v, e) => new UriExpression(v, match.SourceRange));


			Define(() => UriExpression,
				Choice(delimitedUriExpression, bareUriExpression));

			#endregion

			Define(() => ExpressionWhitespace,
				AtLeast(0,
					Choice(new IExpression[] {
						Reference(() => Whitespace),
						Reference(() => Comment)
					}),
					match => Nil.Value));

			#endregion
		}

		private T ParseLines<T>(IExpression<T> expression, LineInfo[] lines) {
			var expressionMatchingContext =
				new ExpressionMatchingContext(
					string.Join("", lines.Select(line => line.LineString).ToArray()),
					lines.Select(line => line.SourceRange).ToArray());
			
			var expressionMatchingResult = expression.Match(expressionMatchingContext);

			if(!expressionMatchingResult.Succeeded)
				return default(T);

			return (T)expressionMatchingResult.Product;
		}
	}
}