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
		/// <summary>
		/// A raw line of input, including the newline character.
		/// </summary>
		public IExpression<LineInfo> Line { get; private set; }
		/// <summary>
		/// A blank line; composed of any number of spaces followed by a line end.
		/// </summary>
		public IExpression<Nil> BlankLines { get; private set; }
		public IExpression<Nil> BlankLine { get; private set; }
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

		public IExpression<Node> Block { get; private set; }

		public IExpression<Node> CommentBlock { get; private set; }
		public IExpression<HeadingNode> Heading { get; private set; }
		public IExpression<ParagraphNode> Paragraph { get; private set; }
		public IExpression<LineInfo> BlockLine { get; private set; }

		public IExpression<Node[]> Inlines { get; private set; }
		public IExpression<Node> Inline { get; private set; }
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
		public IExpression<Expression[]> ParameterList { get; private set; }
		public IExpression<ObjectExpression> ObjectExpression { get; private set; }
		public IExpression<ObjectExpression> ObjectBodyExpression { get; private set; }
		public IExpression<StringExpression> StringExpression { get; private set; }
		public IExpression<UriExpression> UriExpression { get; private set; }
		public IExpression<Node[]> ExpressionWhitespace { get; private set; }

		private IExpression<TProduct> DelimitedInline<TDelimiter, TContent, TProduct>(IExpression<TDelimiter> delimiter, IExpression<TContent> content, Func<IExpressionMatch<object[]>, TContent[], TProduct> matchAction) {
			return Sequence(
				delimiter,
				AtLeast(0, Sequence(NotAhead(delimiter), content, (match, a, b) => b)),
				delimiter,
				(match, s, c, e) => matchAction(match, c));
		}

		public JfmGrammar() {

			Define(() => Document,
				AtLeast(0, Reference(() => Block), match => new JfmDocument(match.Product)));

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

			Define(() => Block,
				Sequence(
					Reference(() => BlankLines),
					Choice<Node>(
						Reference(() => Heading),
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

			Define(() => Heading,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Literal("#"), match => match.Product.Length),
					Reference(() => SpaceChars),
					Reference(() => BlockLine),
					(match, s, l, s2, c) => new HeadingNode(c.LineString, l, match.SourceRange)));

			Define(() => Paragraph,
				AtLeast(1,
					Reference(() => BlockLine),
					match => new ParagraphNode(ParseLines(Inlines, match.Product), match.SourceRange)));

			Define(() => BlockLine,
				Sequence(
					new IExpression[] {
						AtLeast(1, // must consume something (no empty line)
							Sequence(NotAhead(Reference(() => NewLine)), Wildcard())),
						Optional(Reference(() => NewLine))
					}, match => new LineInfo(match.String, match.SourceRange)));
			#endregion

			#region Inline Rules

			Define(() => Inlines,
				AtLeast(0, Reference(() => Inline)));

			Define(() => Inline,
				Choice<Node>(
					Reference(() => Text),
					Reference(() => Space),
					Reference(() => Strong),
					Reference(() => Emphasis),
					Reference(() => Quoted),
					Reference(() => Link),
					Reference(() => Comment),
					Reference(() => Entity),
					Reference(() => LineBreak),
					Reference(() => Symbol)));

			var linkLabel = 
				Sequence(
					Literal("["),
					AtLeast(0, Sequence(NotAhead(Literal("]")), Reference(() => Inline), (match, a, b) => b)),
					Literal("]"),
					(match, s, c, e) => c);

			var linkReferenceId =
				Optional(
					Sequence(
						Reference(() => SpaceChars),
						Reference(() => ReferenceId),
						(match, s, i) => i));

			var linkArguments =
				Optional(
					Sequence(
						Reference(() => SpaceChars),
						Reference(() => ParameterList),
						(match, s, p) => p));

			Define(() => Link,
				Sequence(
					linkLabel, linkReferenceId, linkArguments,
					(match, l, i, a) => new LinkNode(l, i, match.SourceRange)));

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
				Choice(doubleQuoted, singleQuoted));

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
				Choice(
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
							NotAhead(Choice(Literal("]"), Reference(() => NewLine))),
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
					new IExpression[] {
						AtLeast(0,
							Sequence(
								AtLeast(0, Reference(() => SpaceChar)),
								Reference(() => NewLine))),
						Optional(
							Sequence(
								AtLeast(0, Reference(() => SpaceChar)),
								EndOfInput()))
					},
					match => Nil.Value));

			// NEVER EVER EVER USE THIS IN A REPETITION CONTEXT
			Define(() => BlankLine,
				Sequence(
					new IExpression[] {
						AtLeast(0, Reference(() => SpaceChar)),
						Choice(
							new IExpression[] { Reference(() => NewLine), EndOfInput() }) },
					match => Nil.Value));

			Define(() => SpaceChar,
				Choice(
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
				Choice(
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

			Define(() => Expression,
				Choice<Expression>(
					Reference(() => StringExpression),
					Reference(() => StringExpression)));

			var stringPartEscapes =
				Choice(
					Literal(@"\n", match => "\n"),
					Literal(@"\t", match => "\t"),
					Literal(@"\r", match => "\r"),
					Literal(@"\\", match => "\\"));

			var singleQuotedStringPartEscapes =
				Choice(
					Literal(@"\'", match => "'"),
					stringPartEscapes);

			var singleQuotedStringPart =
				Sequence(
					Literal("'"),
					AtLeast(0,
						Choice(
							singleQuotedStringPartEscapes,
							Sequence(NotAhead(Literal("'")), Wildcard(), (match, a, b) => b.ToString()))),
					Literal("'"),
					(match, s, c, e) => string.Join("", c));

			var doubleQuotedStringPartEscapes =
				Choice(
					Literal("\\\"", match => "\""),
					stringPartEscapes);

			var doubleQuotedStringPart =
				Sequence(
					Literal("\""),
					AtLeast(0,
						Choice(
							doubleQuotedStringPartEscapes,
							Sequence(NotAhead(Literal("\"")), Wildcard(), (match, a, b) => b.ToString()))),
					Literal("\""),
					(match, s, c, e) => string.Join("", c));

			var stringPart =
				Choice(singleQuotedStringPart, doubleQuotedStringPart);

			Define(() => StringExpression,
				Sequence(
					stringPart,
					AtLeast(0,
						Sequence(
							AtLeast(0, Reference(() => Whitespace)),
							stringPart,
							(match, w, p) => p)),
					(match, i, p) => new StringExpression(string.Join("", ArrayUtils.Combine(new string[] { i }, p)), match.SourceRange)));

			#region Uri
			
			var uriReserved = Choice(new string[] { ";", "/", "?", ":", "@", "&", "=", "+", "$", "," }.Select(Literal).ToArray());
			var uriMark = Choice(new string[] { "-", "_", ".", "!", "~", "*", "'", "(", ")" }.Select(Literal).ToArray());
			var uriEscaped =
				Sequence(
					new IExpression[] { Literal("%"), Exactly(2, Reference(() => HexDigit)) },
					match => match.String);
			var uriUnreserved = Choice(Reference(() => EnglishAlpha), Reference(() => Digit), uriMark);
			var uriCharacter = Choice(uriUnreserved, uriReserved, uriEscaped);

			Define(() => UriExpression,
				Sequence(
					Optional(Literal("<")),
					AtLeast(1, uriCharacter, match => match.String),
					Optional(Literal(">")),
					(match, s, u, e) => new UriExpression(u, match.SourceRange)));

			#endregion

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