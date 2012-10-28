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
		public IExpression<LineInfo> NonTerminalBlankLine { get; private set; }
		public IExpression<string> Digit { get; private set; }
		public IExpression<string> HexDigit { get; private set; }
		public IExpression<string> EnglishLowerAlpha { get; private set; }
		public IExpression<string> EnglishUpperAlpha { get; private set; }
		public IExpression<string> EnglishAlpha { get; private set; }
		public IExpression<LineInfo> Comment { get; private set; }
		/// <summary>
		/// A C-style multi-line comment.
		/// </summary>
		public IExpression<LineInfo> MultiLineComment { get; private set; }
		/// <summary>
		/// A C-style single line comment. Does not consume the end of the line
		/// </summary>
		public IExpression<LineInfo> SingleLineComment { get; private set; }

		public IExpression<MarkdomDocument> Document { get; private set; }

		public IExpression<IBlockNode[]> Blocks { get; private set; }
		public IExpression<IBlockNode> Block { get; private set; }

		public IExpression<LineInfo> CommentBlock { get; private set; }
		public IExpression<TableNode> Table { get; private set; }
		public IExpression<TableRowNode> TableRow { get; private set; }
		public IExpression<LineInfo> TableRowSeparator { get; private set; }
		public IExpression<HeadingNode> Heading { get; private set; }
		public IExpression<UnorderedListNode> UnorderedList { get; private set; }
		public IExpression<UnorderedListNode> UnorderedListTight { get; private set; }
		public IExpression<UnorderedListNode> UnorderedListLoose { get; private set; }
		public IExpression<ParagraphNode> Paragraph { get; private set; }
		public IExpression<LineInfo> NonEmptyBlockLine { get; private set; }
		public IExpression<LineInfo> BlockLine { get; private set; }
		public IExpression<Nil> BlockLineAtomic { get; private set; }
		public IExpression<Nil> Bullet { get; private set; }
		public IExpression<EnumeratorInfo> Enumerator { get; private set; }

		public IExpression<IInlineNode[]> Inlines { get; private set; }
		public IExpression<IInlineNode> Inline { get; private set; }
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

		public IExpression<int> LowerRomanNumeral { get; private set; }
		public IExpression<int> UpperRomanNumeral { get; private set; }
		public IExpression<int> RomanNumeral { get; private set; }

		public MarkdomGrammar() {

			Define(() => Document,
				Reference(() => Blocks, match => new MarkdomDocument(match.Product)));

			#region Comments

			Define(() => Comment,
				Choice<LineInfo>(
					Reference(() => SingleLineComment),
					Reference(() => MultiLineComment)));

			Define(() => SingleLineComment,
				Sequence(
					Literal("//"),
					Reference(() => Line),
					match => new LineInfo(match.String, match.SourceRange)));

			Define(() => MultiLineComment,
				Sequence(
					Literal("/*"),
					AtLeast(0,
						Sequence( NotAhead(Literal("*/")), Wildcard() ),
						match => match.String),
					Literal("*/"),
					match => new LineInfo(match.String, match.SourceRange)));

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
					OrderedChoice<IBlockNode>(
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
				Choice<LineInfo>(
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
				OrderedChoice(
					Reference(() => UnorderedListTight),
					Reference(() => UnorderedListLoose)));

			Define(() => Bullet,
				Sequence(
					Reference(() => NonIndentSpace),
					Choice(new string[] { "*", "-", "+" }.Select(Literal).ToArray()),
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
				Choice(new IExpression[] {
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
					Choice(
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
					match => new LineInfo(match.String, match.SourceRange)));

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
					OrderedChoice(
						Literal(@"\|"),
						Sequence(
							NotAhead(Literal("|")),
							Reference(() => BlockLineAtomic))),
					match => new LineInfo(match.String, match.SourceRange));

			var tableCellRowSpan =
				Sequence(
					AtLeast(1, Reference(() => Digit), match => match.String),
					Choice(Literal("r"), Literal("R")),
					match => match.Product.Of1);

			var tableCellColumnSpan =
				Sequence(
					AtLeast(1, Reference(() => Digit), match => match.String),
					Choice(Literal("c"), Literal("C")),
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
					OrderedChoice<TableCellNode>(
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
							Choice(new string[] { "-", "=", "+" }.Select(Literal).ToArray()))),
					Reference(() => BlankLine),
					match => new LineInfo(match.String, match.SourceRange)));

			#endregion

			#endregion

			#region Inline Rules

			Define(() => Inlines,
				AtLeast(0, Reference(() => Inline)));

			Define(() => Inline,
				OrderedChoice<IInlineNode>(
					Reference(() => Text),
					Reference(() => Space),
					Reference(() => Strong),
					Reference(() => Emphasis),
					Reference(() => Quoted),
					Reference(() => Link),
					Reference(() => AutoLink),
					Reference(() => Entity),
					Reference(() => LineBreak),
					Reference(() => Symbol)));

			#region Link

			Define(() => AutoLink,
				Sequence(
					Sequence(
						Ahead(Literal("<")), // do not match undelimited UriExpression
						Reference(() => UriExpression),
						match => match.Product.Of2),
					Optional(
						Reference(() => ArgumentList),
						match => match.Product,
						noMatch => new Expression[0]),
					match => new AutoLinkNode(match.Product.Of1, match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

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
				OrderedChoice(doubleQuoted, singleQuoted));

			Define(() => LineBreak,
				Sequence(
					new IExpression[] { Literal(@"\\"), Reference(() => SpaceChars), Reference(() => NewLine) },
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
				OrderedChoice(
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

			Define(() => ReferenceId,
				Sequence(
					Literal("["),
					AtLeast(0,
						Sequence(
							NotAhead(OrderedChoice(Literal("]"), Reference(() => NewLine))),
							Wildcard()),
							match => match.String),
					Literal("]"),
					match => new ReferenceId(match.Product.Of2)));

			#region Roman Numerals

			Define(() => UpperRomanNumeral,
				Sequence(
					Ahead(Choice("IVXLCDM".Select(Literal).ToArray())),
					Reference(() => RomanNumeral),
					match => match.Product.Of2));

			Define(() => LowerRomanNumeral,
				Sequence(
					Ahead(Choice("ivxlcdm".Select(Literal).ToArray())),
					Reference(() => RomanNumeral),
					match => match.Product.Of2));

			var romanNumeralI = Choice(Literal("i", m => 1), Literal("I", m => 1));
			var romanNumeralV = Choice(Literal("v", m => 5), Literal("V", m => 5));
			var romanNumeralX = Choice(Literal("x", m => 10), Literal("X", m => 10));
			var romanNumeralL = Choice(Literal("l", m => 50), Literal("L", m => 50));
			var romanNumeralC = Choice(Literal("c", m => 100), Literal("C", m => 100));
			var romanNumeralD = Choice(Literal("d", m => 500), Literal("D", m => 500));
			var romanNumeralM = Choice(Literal("m", m => 1000), Literal("M", m => 1000));

			Define(() => RomanNumeral,
				Sequence(
					AtMost(3, romanNumeralM, match => match.Product.Sum()),
					RomanNumeralDecade(romanNumeralM, romanNumeralD, romanNumeralC),
					RomanNumeralDecade(romanNumeralC, romanNumeralL, romanNumeralX),
					RomanNumeralDecade(romanNumeralX, romanNumeralV, romanNumeralI),
					match => match.Product.Of1 + match.Product.Of2 + match.Product.Of3 + match.Product.Of4));

			#endregion

			Define(() => Line,
				Sequence(
					AtLeast(0,
						Sequence(
							NotAhead(Reference(() => NewLine)),
							Wildcard())),
					Optional(Reference(() => NewLine)),
					match => new LineInfo(match.String, match.SourceRange)));

			Define(() => BlankLines,
				Sequence(
					AtLeast(0,
						Sequence(
							AtLeast(0, Reference(() => SpaceChar)),
							Reference(() => NewLine),
							match => new LineInfo(match.String, match.SourceRange))),
					Optional(
						Sequence(
							AtLeast(0, Reference(() => SpaceChar)),
							EndOfInput(),
							match => new LineInfo(match.String, match.SourceRange).InArray()),
						match => match.Product,
						noMatch => new LineInfo[0]),
					match => ArrayUtils.Combine(match.Product.Of1, match.Product.Of2)));

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
					new string[] { "*", "&", "'", "\"", "/", "\\", "[", "]", "|", "(", ")" }
					.Select(Literal).ToArray()));

			Define(() => NormalChar,
				Sequence(
					NotAhead(
						Choice(
							Reference(() => Whitespace),
							Reference(() => SpecialChar))),
					Wildcard(),
					match => match.Product.Of2));

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
							match => match.Product.Of2)),
					match => match.Product.Join());

			var doubleQuotedStringExpressionContent =
				AtLeast(0,
					Choice(
						Literal("\\\"", match => "\""),
						stringExpressionEscapes,
						Sequence(
							NotAhead(Choice(Literal("\""), Reference(() => NewLine))),
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
					match => new PropertyAssignment(match.Product.Of1, match.Product.Of3, MarkdomSourceRange.FromMatch(match)));

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
					AtLeast(1, bareUriExpressionCharacter, match => new UriExpression(match.String, MarkdomSourceRange.FromMatch(match))),
					match => match.Product.Of2);

			var delimitedUriExpressionCharacter =
				Choice(
					bareUriExpressionCharacter,
					Choice(new string[] { ",", "(", ")" }.Select(Literal).ToArray()));

			var delimitedUriExpression =
				Sequence(
					Literal("<"), AtLeast(1, delimitedUriExpressionCharacter, match => match.String), Literal(">"),
					match => new UriExpression(match.Product.Of2, MarkdomSourceRange.FromMatch(match)));


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

		private int ParseDefault(string s, int d) {
			int value;
			return int.TryParse(s, out value)
				? value
				: d;
		}

		private IExpression<int> RomanNumeralDecade(IExpression<int> decem, IExpression<int> quintum, IExpression<int> unit) {
			return OrderedChoice(
				Sequence(unit, decem, match => match.Product.Of2 - match.Product.Of1),
				Sequence(unit, quintum, match => match.Product.Of2 - match.Product.Of1),
				Sequence(quintum, AtMost(3, unit), match => match.Product.Of1 + match.Product.Of2.Sum()),
				AtMost(3, unit, match => match.Product.Sum()));
		}
	}
}