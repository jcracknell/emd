using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using markdom.cs.Model.Nodes;
using pegleg.cs;
using pegleg.cs.Parsing;
using pegleg.cs.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs {
	public class MarkdomGrammar : Grammar<MarkdomDocument> {
		public struct LineInfo {
			public readonly string LineString;
			public readonly SourceRange SourceRange;

			public LineInfo(string lineString, SourceRange sourceRange) {
				LineString = lineString;
				SourceRange = sourceRange;
			}

			public static LineInfo FromMatch(IMatch match) {
				return new LineInfo(match.String, match.SourceRange);
			}
		}

		public struct EnumeratorInfo {
			public readonly int Start;
			public readonly int Increment;
			public readonly OrderedListStyle Style;
		}

		public struct TableCellSpanningInfo {
			public readonly int ColumnSpan;
			public readonly int RowSpan;

			public TableCellSpanningInfo(int columnSpan, int rowSpan) {
				ColumnSpan = columnSpan;
				RowSpan = rowSpan;
			}
		}
		
		/// <summary>
		/// A tab or a space.
		/// </summary>
		public IParsingExpression<string> SpaceChar { get; private set; }
		public IParsingExpression<string> SpaceChars { get; private set; }
		/// <summary>
		/// A whitespace character; space, tab or newline.
		/// </summary>
		public IParsingExpression<string> Whitespace { get; private set; }
		public IParsingExpression<string> Whitespaces { get; private set; }
		/// <summary>
		/// A newline character.
		/// </summary>
		public IParsingExpression<string> NewLine { get; private set; }
		public IParsingExpression<string> SpecialChar { get; private set; }
		public IParsingExpression<string> NormalChar { get; private set; }
		public IParsingExpression<string> Indent { get; private set; }
		public IParsingExpression<string> NonIndentSpace { get; private set; }
		/// <summary>
		/// A raw line of input, including the newline character.
		/// </summary>
		public IParsingExpression<LineInfo> Line { get; private set; }
		/// <summary>
		/// A blank line; composed of any number of spaces followed by a line end.
		/// </summary>
		public IParsingExpression<LineInfo[]> BlankLines { get; private set; }
		public IParsingExpression<LineInfo> BlankLine { get; private set; }
		public IParsingExpression<LineInfo> NonTerminalBlankLine { get; private set; }
		public IParsingExpression<string> Digit { get; private set; }
		public IParsingExpression<string> HexDigit { get; private set; }
		public IParsingExpression<string> EnglishLowerAlpha { get; private set; }
		public IParsingExpression<string> EnglishUpperAlpha { get; private set; }
		public IParsingExpression<string> EnglishAlpha { get; private set; }
		public IParsingExpression<LineInfo> Comment { get; private set; }
		/// <summary>
		/// A C-style multi-line comment.
		/// </summary>
		public IParsingExpression<LineInfo> MultiLineComment { get; private set; }
		/// <summary>
		/// A C-style single line comment. Does not consume the end of the line
		/// </summary>
		public IParsingExpression<LineInfo> SingleLineComment { get; private set; }

		public IParsingExpression<MarkdomDocument> Document { get; private set; }

		public IParsingExpression<IBlockNode[]> Blocks { get; private set; }
		public IParsingExpression<IBlockNode> Block { get; private set; }

		public IParsingExpression<LineInfo> CommentBlock { get; private set; }
		public IParsingExpression<TableNode> Table { get; private set; }
		public IParsingExpression<TableRowNode> TableRow { get; private set; }
		public IParsingExpression<LineInfo> TableRowSeparator { get; private set; }
		public IParsingExpression<HeadingNode> Heading { get; private set; }
		public IParsingExpression<UnorderedListNode> UnorderedList { get; private set; }
		public IParsingExpression<UnorderedListNode> UnorderedListTight { get; private set; }
		public IParsingExpression<UnorderedListNode> UnorderedListLoose { get; private set; }
		public IParsingExpression<ParagraphNode> Paragraph { get; private set; }
		public IParsingExpression<LineInfo> NonEmptyBlockLine { get; private set; }
		public IParsingExpression<LineInfo> BlockLine { get; private set; }
		public IParsingExpression<Nil> BlockLineAtomic { get; private set; }
		public IParsingExpression<Nil> Bullet { get; private set; }
		public IParsingExpression<EnumeratorInfo> Enumerator { get; private set; }

		public IParsingExpression<IInlineNode[]> Inlines { get; private set; }
		public IParsingExpression<IInlineNode> Inline { get; private set; }
		public IParsingExpression<AutoLinkNode> AutoLink { get; private set; }
		public IParsingExpression<LinkNode> Link { get; private set; }
		public IParsingExpression<StrongNode> Strong { get; private set; }
		public IParsingExpression<EmphasisNode> Emphasis { get; private set; }
		public IParsingExpression<QuotedNode> Quoted { get; private set; }
		public IParsingExpression<LineBreakNode> LineBreak { get; private set; }
		public IParsingExpression<TextNode> Text { get; private set; }
		public IParsingExpression<SpaceNode> Space { get; private set; }
		public IParsingExpression<EntityNode> Entity { get; private set; }

		public IParsingExpression<ReferenceId> ReferenceId { get; private set; }

		/// <summary>
		/// A Symbol, an unescaped special character which was not parsed into a valid node.
		/// </summary>
		public IParsingExpression<SymbolNode> Symbol { get; private set; }


		public IParsingExpression<Expression> Expression { get; private set; }
		public IParsingExpression<Expression[]> ArgumentList { get; private set; }
		public IParsingExpression<ObjectExpression> ObjectExpression { get; private set; }
		public IParsingExpression<ObjectExpression> ObjectBodyExpression { get; private set; }
		public IParsingExpression<StringExpression> StringExpression { get; private set; }
		public IParsingExpression<UriExpression> UriExpression { get; private set; }
		public IParsingExpression<Nil> ExpressionWhitespace { get; private set; }

		public IParsingExpression<int> LowerRomanNumeral { get; private set; }
		public IParsingExpression<int> UpperRomanNumeral { get; private set; }
		public IParsingExpression<int> RomanNumeral { get; private set; }

		public MarkdomGrammar() {

			Define(() => Document,
				Reference(() => Blocks, match => new MarkdomDocument(match.Product)));

			#region Comments

			Define(() => Comment,
				ChoiceUnordered<LineInfo>(
					Reference(() => SingleLineComment),
					Reference(() => MultiLineComment)));

			Define(() => SingleLineComment,
				Sequence(
					Literal("//"),
					Reference(() => Line),
					match => LineInfo.FromMatch(match)));

			Define(() => MultiLineComment,
				Sequence(
					Literal("/*"),
					AtLeast(0,
						Sequence( NotAhead(Literal("*/")), Wildcard() ),
						match => match.String),
					Literal("*/"),
					match => LineInfo.FromMatch(match)));

			#endregion

			#region Block Rules

			Define(() => Blocks,
				AtLeast(0,
					Sequence(
						AtLeast(0, Reference(() => CommentBlock)),
						Reference(() => Block),
						AtLeast(0, Reference(() => CommentBlock)),
						match => match.Product.Of2)));

			// Ordering notes:
			//   * Paragraph must come last, because it will sweep up just about anything
			//   * Table must precede unordered list, because of fancy row separators starting with +/-
			Define(() => Block,
				Sequence(
					Reference(() => BlankLines),
					ChoiceOrdered<IBlockNode>(
						Reference(() => Heading),
						Reference(() => Table),
						Reference(() => UnorderedList),
						Reference(() => Paragraph)), // paragraph must come last
					Reference(() => BlankLines),
					match => match.Product.Of2));

			var singleLineCommentBlock =
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => SingleLineComment),
					Reference(() => BlankLines),
					match => match.Product.Of2);

			var multiLineCommentBlock =
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => MultiLineComment),
					Reference(() => BlankLine), // no trailing content (don't match a para w/ starting mlc)
					Reference(() => BlankLines),
					match => match.Product.Of2);

			Define(() => CommentBlock,
				ChoiceUnordered<LineInfo>(
					singleLineCommentBlock,
					multiLineCommentBlock));

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
					match => match.Product.Of1);

			// We define a custom BlockLine for lists which discards any indent at the beginning of a line
			var listBlockLine =
				Sequence(
					Optional(Reference(() => Indent)),
					Reference(() => NonEmptyBlockLine),
					match => match.Product.Of2);

			#region Unordered List

			// We first attempt to parse a tight list, because a loose list is defined as 'one that
			// is not tight'.
			Define(() => UnorderedList,
				ChoiceOrdered(
					Reference(() => UnorderedListTight),
					Reference(() => UnorderedListLoose)));

			Define(() => Bullet,
				Sequence(
					Reference(() => NonIndentSpace),
					ChoiceUnordered(new string[] { "*", "-", "+" }.Select(Literal).ToArray()),
					Reference(() => SpaceChars),
					match => Nil.Value));

			var unorderedListBlockLine =
				Sequence(
					NotAhead(Reference(() => Bullet)), listBlockLine,
					match => match.Product.Of2);

			var unorderedListBlockLines = AtLeast(0, unorderedListBlockLine);

			var unorderedListItemInitialBlock =
				Sequence(
					Reference(() => Bullet),
					Reference(() => BlockLine), // Allow for an empty list item
					unorderedListBlockLines,
					match => ArrayUtils.Prepend(match.Product.Of2, match.Product.Of3));

			var unorderedListItemSubsequentBlock =
				Sequence(
					listItemContinues,
					unorderedListBlockLines,
					Reference(() => BlankLines),
					match => ArrayUtils.Combine(match.Product.Of1, match.Product.Of2, match.Product.Of3));

			var unorderedListItemTight =
				Reference(
					() => unorderedListItemInitialBlock,
					match => new UnorderedListItemNode(ParseLines(Inlines, match.Product), MarkdomSourceRange.FromMatch(match)));

			var unorderedListItemLoose =
				Sequence(
					unorderedListItemInitialBlock,
					AtLeast(0, unorderedListItemSubsequentBlock, match => ArrayUtils.Flatten(match.Product)),
					Reference(() => BlankLines), // chew up any blank lines after an initial block with no subsequent
					match => new UnorderedListItemNode(ParseLines(Blocks, ArrayUtils.Combine(match.Product.Of1, match.Product.Of2)), MarkdomSourceRange.FromMatch(match)));

			var unorderedListContinuesLoose =
				ChoiceUnordered(new IParsingExpression[] {
					 Sequence(Reference(() => BlankLines), Reference(() => Bullet)),
					 listItemContinues });;

			Define(() => UnorderedListTight,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(1, unorderedListItemTight),
					NotAhead(unorderedListContinuesLoose),
					match => new UnorderedListNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => UnorderedListLoose,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(1, unorderedListItemLoose),
					match => new UnorderedListNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			#endregion

			#region Ordered List

			// To avoid tedious mental gymnastics, you can specify a specify a combination of style
			// any value for the initial item in a list using the form `style@value`, e.g. `a@7`.
			var enumeratorValue =
				Sequence(
					Literal("@"),
					AtLeast(1, Reference(() => Digit), match => { int v; return int.TryParse(match.String, out v) ? v : 1; }),
					match => match.Product.Of2);

			// You can also specify an increment using the form `style@value+/-increment`, e.g. `a@8-1`.
			var enumeratorIncrement =
				Sequence(
					ChoiceUnordered(
						Literal("+", match => 1),
						Literal("-", match => -1)),
					AtLeast(1, Reference(() => Digit), match => match.String),
					match => match.Product.Of1 * match.Product.Of2.ParseDefault(1));

			#endregion

			#endregion

			Define(() => Heading,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Literal("#"), match => match.Product.Length),
					Reference(() => SpaceChars),
					Reference(() => BlockLine),
					match => new HeadingNode(match.Product.Of4.LineString, match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => Paragraph,
				AtLeast(1,
					Reference(() => NonEmptyBlockLine),
					match => new ParagraphNode(ParseLines(Inlines, match.Product), MarkdomSourceRange.FromMatch(match))));

			Define(() => NonEmptyBlockLine,
				Sequence(
					NotAhead(Reference(() => BlankLine)),
					Reference(() => BlockLine),
					match => match.Product.Of2));

			Define(() => BlockLine,
				Sequence(
					AtLeast(0, Reference(() => BlockLineAtomic)),
					Optional(Reference(() => NewLine)),
					match => LineInfo.FromMatch(match)));

			// TODO: multi-line atomics - comments, expressions
			Define(() => BlockLineAtomic,
				Sequence(
					NotAhead(Reference(() => NewLine)),
					Wildcard(),
					match => Nil.Value));

			#region Tables

			Define(() => Table,
				AtLeast(1,
					Sequence(
						AtLeast(0, Reference(() => TableRowSeparator)),
						Reference(() => TableRow),
						AtLeast(0, Reference(() => TableRowSeparator)),
						match => match.Product.Of2),
					match => new TableNode(match.Product, MarkdomSourceRange.FromMatch(match))));

			var tableCellContents =
				AtLeast(0,
					ChoiceOrdered(
						Literal(@"\|"),
						Sequence(
							NotAhead(Literal("|")),
							Reference(() => BlockLineAtomic))),
					match => LineInfo.FromMatch(match));

			var tableCellRowSpan =
				Sequence(
					AtLeast(1, Reference(() => Digit), match => match.String),
					ChoiceUnordered(Literal("r"), Literal("R")),
					match => match.Product.Of1);

			var tableCellColumnSpan =
				Sequence(
					AtLeast(1, Reference(() => Digit), match => match.String),
					ChoiceUnordered(Literal("c"), Literal("C")),
					match => match.Product.Of1);

			var tableCellAnnouncement =
				Sequence(
					Literal("|"),
					Optional(tableCellColumnSpan),
					Optional(tableCellRowSpan),
					match => new TableCellSpanningInfo(match.Product.Of2.ParseDefault(1), match.Product.Of3.ParseDefault(1)));

			var tableHeaderCellAnnouncement =
				Sequence(
					tableCellAnnouncement,
					Literal("="),
					Reference(() => SpaceChars),
					match => match.Product.Of1);

			var tableDataCellAnnouncemnt =
				Sequence(
					tableCellAnnouncement,
					Reference(() => SpaceChars),
					match => match.Product.Of1);

			var tableHeaderCell =
				Sequence(
					tableHeaderCellAnnouncement,
					tableCellContents,
					match =>
						new TableHeaderCellNode(
							match.Product.Of1.ColumnSpan,
							match.Product.Of1.RowSpan,
							ParseLines(Inlines, match.Product.Of2.InArray()),
							MarkdomSourceRange.FromMatch(match)));

			var tableDataCell =
				Sequence(
					tableDataCellAnnouncemnt,
					tableCellContents,
					match => new TableDataCellNode(
						match.Product.Of1.ColumnSpan,
						match.Product.Of1.RowSpan,
						ParseLines(Inlines, match.Product.Of2.InArray()),
						MarkdomSourceRange.FromMatch(match)));

			var tableRowEnd =
				Sequence(
					Optional(Literal("|")),
					Reference(() => BlankLine));

			var tableCell =
				Sequence(
					NotAhead(tableRowEnd),
					ChoiceOrdered<TableCellNode>(
						tableHeaderCell,
						tableDataCell),
					match => match.Product.Of2);

			var tableUnannouncedDataCell =
				Sequence(
					tableCellContents,
					Ahead(Literal("|")),
					match => new TableDataCellNode(1, 1, ParseLines(Inlines, match.Product.Of1.InArray()), MarkdomSourceRange.FromMatch(match)));

			Define(() => TableRow,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Reference(() => tableCell)),
					tableRowEnd,
					match => new TableRowNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => TableRowSeparator,
				Sequence(
					AtLeast(1,
						Sequence(
							Reference(() => SpaceChars),
							ChoiceUnordered(new string[] { "-", "=", "+" }.Select(Literal).ToArray()))),
					Reference(() => BlankLine),
					match => LineInfo.FromMatch(match)));

			#endregion

			#endregion

			#region Inline Rules

			Define(() => Inlines,
				AtLeast(0, Reference(() => Inline)));

			Define(() => Inline,
				ChoiceOrdered<IInlineNode>(
					ChoiceUnordered<IInlineNode>(
						Reference(() => Text),
						Reference(() => Space),
						ChoiceOrdered<IInlineNode>(
							Reference(() => Strong),
							Reference(() => Emphasis)),
						Reference(() => Quoted),
						Reference(() => Link),
						Reference(() => AutoLink),
						Reference(() => Entity),
						Reference(() => LineBreak)),
					Reference(() => Symbol)));

			#region Link

			Define(() => AutoLink,
				Sequence(
					Literal("<"),
					Reference(() => SpaceChars),
					Reference(() => UriExpression),
					Reference(() => SpaceChars),
					Literal(">"),
					Optional(
						Reference(() => ArgumentList),
						match => match.Product,
						noMatch => new Expression[0]),
					match => new AutoLinkNode(match.Product.Of3, match.Product.Of6, MarkdomSourceRange.FromMatch(match))));

			var linkLabel =
				Sequence(
					Literal("["),
					AtLeast(0, Sequence(NotAhead(Literal("]")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("]"),
					match => match.Product.Of2);

			Define(() => Link,
				Sequence(
					linkLabel,
					Reference(() => SpaceChars),
					Optional(Reference(() => ArgumentList)),
					match => new LinkNode(match.Product.Of1, null, MarkdomSourceRange.FromMatch(match))));

			#endregion

			Define(() => Strong,
				Sequence(
					Literal("**"),
					AtLeast(0, Sequence(NotAhead(Literal("**")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("**")),
					match => new StrongNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => Emphasis,
				Sequence(
					Literal("*"),
					AtLeast(0, Sequence(NotAhead(Literal("*")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("*")),
					match => new EmphasisNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			var singleQuoted =
				Sequence(
					Literal("'"),
					AtLeast(0, Sequence(NotAhead(Literal("'")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("'"),
					match => new QuotedNode(QuoteType.Single, match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			var doubleQuoted =
				Sequence(
					Literal("\""),
					AtLeast(0, Sequence(NotAhead(Literal("\"")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("\""),
					match => new QuotedNode(QuoteType.Double, match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			Define(() => Quoted,
				ChoiceOrdered(doubleQuoted, singleQuoted));

			Define(() => LineBreak,
				Sequence(
					new IParsingExpression[] { Literal(@"\\"), Reference(() => SpaceChars), Reference(() => NewLine) },
					match => new LineBreakNode(MarkdomSourceRange.FromMatch(match))));

			#region Entities

			var decimalHtmlEntity =
				Sequence(
					Literal("&#"),
					Between(1, 6, Reference(() => Digit), match => match.String),
					Literal(";"),
					match => new EntityNode(int.Parse(match.Product.Of2), MarkdomSourceRange.FromMatch(match)));

			var hexHtmlEntity =
				Sequence(
					Literal("&#x"),
					Between(1, 6, Reference(() => HexDigit), match => match.String),
					Literal(";"),
					match => new EntityNode(Convert.ToInt32(match.Product.Of2, 16), MarkdomSourceRange.FromMatch(match)));

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
						match => new EntityNode(EntityNode.GetNamedEntityValue(entityName), MarkdomSourceRange.FromMatch(match)));
				});

			Define(() => Entity,
				ChoiceOrdered(
					decimalHtmlEntity,
					hexHtmlEntity,
					namedHtmlEntity));

			#endregion 

			Define(() => Text,
				AtLeast(1,
					Reference(() => NormalChar),
					match => new TextNode(match.String, MarkdomSourceRange.FromMatch(match))));

			Define(() => Space,
				AtLeast(1, Reference(() => Whitespace), match => new SpaceNode(MarkdomSourceRange.FromMatch(match))));

			Define(() => Symbol,
				Reference(() => SpecialChar, match => new SymbolNode(match.String, MarkdomSourceRange.FromMatch(match))));

			#endregion

			#region Expressions
			// # Expressions
			//
			// markdom.cs uses an expression syntax which is *suspiciously similar* to Javascript (in fact
			// it is designed to emulate javascript for its succinctness, familiarity, and simplicity.
			// Given the primary use cases for markdom.cs, the addition of URI literals to the expression
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
				ChoiceOrdered<Expression>(
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
						AtLeast(0, Sequence( argumentSeparator, Reference(() => Expression), match => match.Product.Of2)),
						match => ArrayUtils.Prepend(match.Product.Of1, match.Product.Of2)));

			Define(() => ArgumentList,
				Sequence(
					Sequence(Literal("("), Reference(() => ExpressionWhitespace)),
					argumentListArguments,
					Sequence(Reference(() => ExpressionWhitespace), Literal(")")),
					match => match.Product.Of2 ?? new Expression[0]));

			#region StringExpression
			
			var stringExpressionEscapes =
				ChoiceUnordered(
					Literal(@"\n", match => "\n"),
					Literal(@"\r", match => "\r"),
					Literal(@"\t", match => "\t"),
					Literal(@"\\", match => "\\"));

			var singleQuotedStringExpressionContent =
				AtLeast(0,
					ChoiceUnordered(
						Literal(@"\'", match => "'"),
						stringExpressionEscapes,
						Sequence(
							NotAhead(ChoiceUnordered(Literal("'"), Reference(() => NewLine))),
							Wildcard(),
							match => match.Product.Of2)),
					match => match.Product.Join());

			var doubleQuotedStringExpressionContent =
				AtLeast(0,
					ChoiceUnordered(
						Literal("\\\"", match => "\""),
						stringExpressionEscapes,
						Sequence(
							NotAhead(ChoiceUnordered(Literal("\""), Reference(() => NewLine))),
							Wildcard(),
							match => match.Product.Of2)),
					match => match.Product.Join());

			var singleQuotedStringExpression =
				Sequence(
					Literal("'"), singleQuotedStringExpressionContent, Literal("'"),
					match => new StringExpression(match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			var doubleQuotedStringExpression =
				Sequence(
					Literal("\""), doubleQuotedStringExpressionContent, Literal("\""),
					match => new StringExpression(match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			Define(() => StringExpression,
				ChoiceUnordered(
					singleQuotedStringExpression,
					doubleQuotedStringExpression));

			#endregion

			#region ObjectExpression

			var objectPropertyAssignment =
				Sequence(
					Reference(() => StringExpression), // TODO: Identifier / String / Uri
					Reference(() => ExpressionWhitespace),
					Literal(":"),
					Reference(() => ExpressionWhitespace),
					Reference(() => Expression),
					match => new PropertyAssignment(match.Product.Of1, match.Product.Of5, MarkdomSourceRange.FromMatch(match)));

			var objectPropertyAssignments =
				Optional(
					Sequence(
						objectPropertyAssignment,
						AtLeast(0, Sequence(argumentSeparator, objectPropertyAssignment, match => match.Product.Of2)),
						match => ArrayUtils.Combine(match.Product.Of1.InArray(), match.Product.Of2)));

			var objectExpressionStart =
				Sequence( Literal("{"), Reference(() => ExpressionWhitespace));

			var objectExpressionEnd =
				Sequence( Reference(() => ExpressionWhitespace), Literal("}"));

			Define(() => ObjectExpression,
				Sequence(
					objectExpressionStart, objectPropertyAssignments, objectExpressionEnd,
					match => new ObjectExpression(match.Product.Of2 ?? new PropertyAssignment[0], MarkdomSourceRange.FromMatch(match))));

			Define(() => ObjectBodyExpression,
				Reference(() => objectPropertyAssignments, match => new ObjectExpression(match.Product, MarkdomSourceRange.FromMatch(match))));

			#endregion

			#region UriExpression
			//   * Cannot contain `,` (commas), which are ordinarily legal URI characters.
			//     This is to disambiguate URI expressions in argument lists
			//   * Parentheses inside of a URI expression must be balanced
			//   * Cannot start with `@`

			IParsingExpression<object[]> uriExpressionPart = null;
			IParsingExpression<object[]> uriExpressionRegularPart = null;
			IParsingExpression<object[]> uriExpressionParenthesizedPart = null;

			uriExpressionPart = 
				Named("UriExpressionPart",
					ChoiceOrdered(
						Reference(() => uriExpressionRegularPart),
						Reference(() => uriExpressionParenthesizedPart)));

			uriExpressionRegularPart =
				AtLeast(1, 
					ChoiceUnordered(
						Reference(() => EnglishAlpha),
						Reference(() => Digit),
						ChoiceUnordered(new string[] { "/", "?", ":", "@", "&", "=", "?", "+", "$", "-", "_", "!", "~", "*", "'", ".", ";" } .Select(Literal).ToArray()),
						Sequence(
							Literal("%"),
							Exactly(2, Reference(() => HexDigit)))));

			uriExpressionParenthesizedPart =
				Sequence(
					Literal("("),
					AtLeast(0, Reference(() => uriExpressionPart)),
					Literal(")"));

			var uriExpressionFirstPart =
				Sequence(
					NotAhead(Literal("@")),
					Reference(() => uriExpressionPart));

			Define(() => UriExpression,
				Sequence(
					uriExpressionFirstPart,
					AtLeast(0, uriExpressionPart),
					match => new UriExpression(match.String, MarkdomSourceRange.FromMatch(match))));

			#endregion

			Define(() => ExpressionWhitespace,
				AtLeast(0,
					ChoiceUnordered(new IParsingExpression[] {
						Reference(() => Whitespace),
						Reference(() => Comment)
					}),
					match => Nil.Value));

			#endregion

			#region Roman Numerals

			Define(() => UpperRomanNumeral,
				Sequence(
					Ahead(ChoiceUnordered("IVXLCDM".Select(Literal).ToArray())),
					Reference(() => RomanNumeral),
					match => match.Product.Of2));

			Define(() => LowerRomanNumeral,
				Sequence(
					Ahead(ChoiceUnordered("ivxlcdm".Select(Literal).ToArray())),
					Reference(() => RomanNumeral),
					match => match.Product.Of2));

			var romanNumeralI = ChoiceUnordered(Literal("i", m => 1), Literal("I", m => 1));
			var romanNumeralV = ChoiceUnordered(Literal("v", m => 5), Literal("V", m => 5));
			var romanNumeralX = ChoiceUnordered(Literal("x", m => 10), Literal("X", m => 10));
			var romanNumeralL = ChoiceUnordered(Literal("l", m => 50), Literal("L", m => 50));
			var romanNumeralC = ChoiceUnordered(Literal("c", m => 100), Literal("C", m => 100));
			var romanNumeralD = ChoiceUnordered(Literal("d", m => 500), Literal("D", m => 500));
			var romanNumeralM = ChoiceUnordered(Literal("m", m => 1000), Literal("M", m => 1000));

			Define(() => RomanNumeral,
				Sequence(
					AtMost(3, romanNumeralM, match => match.Product.Sum()),
					RomanNumeralDecade(romanNumeralM, romanNumeralD, romanNumeralC),
					RomanNumeralDecade(romanNumeralC, romanNumeralL, romanNumeralX),
					RomanNumeralDecade(romanNumeralX, romanNumeralV, romanNumeralI),
					match => match.Product.Of1 + match.Product.Of2 + match.Product.Of3 + match.Product.Of4));

			#endregion

			#region Text, Character Classes, etc

			Define(() => Line,
				Sequence(
					AtLeast(0,
						Sequence(
							NotAhead(Reference(() => NewLine)),
							Wildcard())),
					Optional(Reference(() => NewLine)),
					match => LineInfo.FromMatch(match)));

			Define(() => BlankLines,
				Sequence(
					AtLeast(0,
						Sequence(
							AtLeast(0, Reference(() => SpaceChar)),
							Reference(() => NewLine),
							match => LineInfo.FromMatch(match))),
					Optional(
						Sequence(
							AtLeast(0, Reference(() => SpaceChar)),
							EndOfInput(),
							match => LineInfo.FromMatch(match).InArray()),
						match => match.Product,
						noMatch => new LineInfo[0]),
					match => ArrayUtils.Combine(match.Product.Of1, match.Product.Of2)));

			// NEVER EVER EVER USE THIS IN A REPETITION CONTEXT
			Define(() => BlankLine,
				Sequence(
					new IParsingExpression[] {
						AtLeast(0, Reference(() => SpaceChar)),
						ChoiceUnordered(
							new IParsingExpression[] { Reference(() => NewLine), EndOfInput() }) },
					match => LineInfo.FromMatch(match)));

			Define(() => Indent,
				ChoiceUnordered(Literal("\t"), Literal("    ")));

			Define(() => NonIndentSpace,
				AtMost(3, Literal(" "), match => match.String));

			Define(() => SpaceChar,
				ChoiceOrdered(
					Literal(" "),
					Literal("\t")));

			Define(() => SpaceChars,
				AtLeast(0, Reference(() => SpaceChar), match => match.String));

			Define(() => NewLine,
				ChoiceUnordered(
					Literal("\n"),
					Literal("\r\n")));

			Define(() => Whitespace,
				ChoiceUnordered(
					Reference(() => SpaceChar),
					Reference(() => NewLine)));

			Define(() => Whitespaces,
				AtLeast(0, Reference(() => Whitespace), match => match.String));

			Define(() => Digit,
				CharacterInRange('0', '9'));

			Define(() => HexDigit,
				ChoiceOrdered(
					Reference(() => Digit),
					CharacterInRange('a', 'f'),
					CharacterInRange('A', 'F')));

			Define(() => EnglishLowerAlpha,
				CharacterInRange('a', 'z'));

			Define(() => EnglishUpperAlpha,
				CharacterInRange('A', 'Z'));

			Define(() => EnglishAlpha,
				ChoiceUnordered(
					Reference(() => EnglishLowerAlpha),
					Reference(() => EnglishUpperAlpha)));

			Define(() => SpecialChar,
				ChoiceUnordered(
					new string[] { "*", "&", "'", "\"", "/", "\\", "[", "]", "|", "(", ")" }
					.Select(Literal).ToArray()));

			Define(() => NormalChar,
				Sequence(
					NotAhead(
						ChoiceUnordered(
							Reference(() => Whitespace),
							Reference(() => SpecialChar))),
					Wildcard(),
					match => match.Product.Of2));
			#endregion
		}

		private T ParseLines<T>(IParsingExpression<T> expression, LineInfo[] lines) {
			var expressionMatchingContext =
				new MatchingContext(
					lines.Select(line => line.LineString).Join(),
					lines.Select(line => line.SourceRange).ToArray());
			
			var expressionMatchingResult = expression.Match(expressionMatchingContext);

			if(!expressionMatchingResult.Succeeded)
				return default(T);

			return (T)expressionMatchingResult.Product;
		}

		private IParsingExpression<int> RomanNumeralDecade(IParsingExpression<int> decem, IParsingExpression<int> quintum, IParsingExpression<int> unit) {
			return ChoiceOrdered(
				Sequence(unit, decem, match => match.Product.Of2 - match.Product.Of1),
				Sequence(unit, quintum, match => match.Product.Of2 - match.Product.Of1),
				Sequence(quintum, AtMost(3, unit), match => match.Product.Of1 + match.Product.Of2.Sum()),
				AtMost(3, unit, match => match.Product.Sum()));
		}
	}
}