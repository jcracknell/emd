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
	public class MarkdomGrammar : Grammar<MarkdomDocumentNode> {
		#region Intermediate data structures

		public class LineInfo {
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

		public class BlockLineInfo {
			public readonly LineInfo[] Lines;
			public readonly SourceRange SourceRange;

			public BlockLineInfo(LineInfo[] lines, SourceRange sourceRange) {
				Lines = lines;
				SourceRange = sourceRange;
			}

			public static BlockLineInfo FromMatch(IMatch<LineInfo[]> match) {
				return new BlockLineInfo(match.Product, match.SourceRange);
			}

			public static BlockLineInfo FromMatch(IMatch<LineInfo> match) {
				return new BlockLineInfo(match.Product.InArray(), match.SourceRange);
			}
		}

		public class EnumeratorCounterStyleInfo {
			public readonly OrderedListCounterStyle Style;
			public readonly int SuggestedValue;

			public EnumeratorCounterStyleInfo(OrderedListCounterStyle style, int suggestedValue) {
				Style = style;
				SuggestedValue = suggestedValue;
			}
		}

		public class EnumeratorCounterStyleDefinition {
			public readonly OrderedListCounterStyle CounterStyle;
			public IParsingExpression<EnumeratorCounterStyleInfo> Expression;

			public EnumeratorCounterStyleDefinition(OrderedListCounterStyle style, IParsingExpression<EnumeratorCounterStyleInfo> expression) {
				CounterStyle = style;
				Expression = expression;
			}
		}

		public class EnumeratorSeparatorStyleDefinition {
			public readonly OrderedListSeparatorStyle SeparatorStyle;
			public readonly IParsingExpression<object> Preceding;
			public readonly IParsingExpression<object> Following;
			public EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle style, IParsingExpression<object> preceding, IParsingExpression<object> following) {
				SeparatorStyle = style;
				Preceding = preceding;
				Following = following;
			}
		}

		public class OrderedListStyleDefinition {
			public readonly OrderedListCounterStyle CounterStyle;
			public readonly OrderedListSeparatorStyle SeparatorStyle;
			public readonly IParsingExpression<InitialEnumeratorInfo> InitialEnumerator;
			public readonly IParsingExpression<ContinuationEnumeratorInfo> ContinuationEnumerator;

			public OrderedListStyleDefinition(OrderedListCounterStyle counterStyle, OrderedListSeparatorStyle separatorStyle, IParsingExpression<InitialEnumeratorInfo> initialEnumerator, IParsingExpression<ContinuationEnumeratorInfo> continuationEnumerator) {
				CounterStyle = counterStyle;
				SeparatorStyle = separatorStyle;
				InitialEnumerator = initialEnumerator;
				ContinuationEnumerator = continuationEnumerator;
			}
		}

		public class InitialEnumeratorInfo {
			public readonly OrderedListCounterStyle CounterStyle;
			public readonly OrderedListSeparatorStyle SeparatorStyle;
			public readonly int Value;

			public InitialEnumeratorInfo(OrderedListCounterStyle counterStyle, OrderedListSeparatorStyle separatorStyle, int value) {
				CounterStyle = counterStyle;
				SeparatorStyle = separatorStyle;
				Value = value;
			}
		}

		public class ContinuationEnumeratorInfo {
			public readonly OrderedListCounterStyle CounterStyle;
			public readonly OrderedListSeparatorStyle SeparatorStyle;

			public ContinuationEnumeratorInfo(OrderedListCounterStyle counterStyle, OrderedListSeparatorStyle separatorStyle) {
				CounterStyle = counterStyle;
				SeparatorStyle = separatorStyle;
			}
		}

		public class TableCellSpanningInfo {
			public readonly int ColumnSpan;
			public readonly int RowSpan;

			public TableCellSpanningInfo(int columnSpan, int rowSpan) {
				ColumnSpan = columnSpan;
				RowSpan = rowSpan;
			}
		}

		#endregion
		
		public IParsingExpression<LineInfo> Comment { get; private set; }
		public IParsingExpression<LineInfo> MultiLineComment { get; private set; }
		public IParsingExpression<LineInfo> SingleLineComment { get; private set; }

		public IParsingExpression<MarkdomDocumentNode> Document { get; private set; }

		public IParsingExpression<IBlockNode[]> Blocks { get; private set; }
		public IParsingExpression<IBlockNode> Block { get; private set; }

		public IParsingExpression<LineInfo> CommentBlock { get; private set; }
		public IParsingExpression<TableNode> Table { get; private set; }
		public IParsingExpression<TableRowNode> TableRow { get; private set; }
		public IParsingExpression<LineInfo> TableRowSeparator { get; private set; }
		public IParsingExpression<HeadingNode> Heading { get; private set; }
		public IParsingExpression<int> HeadingAnnouncement { get; private set; }
		public IParsingExpression<OrderedListNode> OrderedList { get; private set; }
		public IParsingExpression<InitialEnumeratorInfo> Enumerator { get; private set; }
		public IParsingExpression<UnorderedListNode> UnorderedList { get; private set; }
		public IParsingExpression<UnorderedListNode> UnorderedListTight { get; private set; }
		public IParsingExpression<UnorderedListNode> UnorderedListLoose { get; private set; }
		public IParsingExpression<Nil> Bullet { get; private set; }
		public IParsingExpression<ParagraphNode> Paragraph { get; private set; }
		public IParsingExpression<ReferenceNode> ReferenceBlock { get; private set; }
		public IParsingExpression<LineInfo> NonEmptyBlockLine { get; private set; }
		public IParsingExpression<LineInfo> BlockLine { get; private set; }
		public IParsingExpression<Nil> BlockLineAtomic { get; private set; }
		public IParsingExpression<Nil> Atomic { get; private set; }

		public IParsingExpression<IInlineNode[]> Inlines { get; private set; }
		public IParsingExpression<IInlineNode> Inline { get; private set; }
		public IParsingExpression<AutoLinkNode> AutoLink { get; private set; }
		public IParsingExpression<LinkNode> Link { get; private set; }
		public IParsingExpression<IInlineNode[]> Label { get; private set; }
		public IParsingExpression<StrongNode> Strong { get; private set; }
		public IParsingExpression<EmphasisNode> Emphasis { get; private set; }
		public IParsingExpression<InlineExpressionNode> InlineExpression { get; private set; }
		public IParsingExpression<QuotedNode> Quoted { get; private set; }
		public IParsingExpression<LineBreakNode> LineBreak { get; private set; }
		public IParsingExpression<TextNode> Text { get; private set; }
		public IParsingExpression<SpaceNode> Space { get; private set; }
		public IParsingExpression<EntityNode> Entity { get; private set; }
		public IParsingExpression<ReferenceId> ReferenceLabel { get; private set; }
		public IParsingExpression<SymbolNode> Symbol { get; private set; }

		public IParsingExpression<IExpression> Expression { get; private set; }
		public IParsingExpression<IExpression> LiteralExpression { get; private set; }
		public IParsingExpression<IExpression[]> ArgumentList { get; private set; }
		public IParsingExpression<ObjectLiteralExpression> ObjectLiteralExpression { get; private set; }
		public IParsingExpression<ObjectLiteralExpression> ObjectBodyExpression { get; private set; }
		public IParsingExpression<NumericLiteralExpression> NumericLiteralExpression { get; private set; }
		public IParsingExpression<StringLiteralExpression> StringLiteralExpression { get; private set; }
		public IParsingExpression<UriLiteralExpression> UriLiteralExpression { get; private set; }
		public IParsingExpression<DocumentLiteralExpression> DocumentLiteralExpression { get; private set; }
		public IParsingExpression<Nil> ExpressionWhitespace { get; private set; }

		public IParsingExpression<int> LowerRomanNumeral { get; private set; }
		public IParsingExpression<int> UpperRomanNumeral { get; private set; }
		public IParsingExpression<int> RomanNumeral { get; private set; }

		/// <summary>
		/// A tab or a space.
		/// </summary>
		public IParsingExpression<Nil> SpaceChar { get; private set; }
		public IParsingExpression<string> SpaceChars { get; private set; }
		/// <summary>
		/// A whitespace character; space, tab or newline.
		/// </summary>
		public IParsingExpression<Nil> Whitespace { get; private set; }
		public IParsingExpression<Nil[]> Whitespaces { get; private set; }
		/// <summary>
		/// A newline character.
		/// </summary>
		public IParsingExpression<string> NewLine { get; private set; }
		public IParsingExpression<Nil> SpecialChar { get; private set; }
		public IParsingExpression<Nil> NormalChar { get; private set; }
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
		public IParsingExpression<Nil> Digit { get; private set; }
		public IParsingExpression<Nil> NonZeroDigit { get; private set; }
		public IParsingExpression<Nil> HexDigit { get; private set; }
		public IParsingExpression<Nil> EnglishLowerAlpha { get; private set; }
		public IParsingExpression<Nil> EnglishUpperAlpha { get; private set; }
		public IParsingExpression<Nil> EnglishAlpha { get; private set; }

		public IParsingExpression<string> CAmpersand { get; private set; }
		public IParsingExpression<string> CAsterix { get; private set; }
		public IParsingExpression<string> CAt { get; private set; }
		public IParsingExpression<string> CBackSlash { get; private set; }
		public IParsingExpression<string> CColon { get; private set; }
		public IParsingExpression<string> CComma { get; private set; }
		public IParsingExpression<string> CDoubleQuote { get; private set; }
		public IParsingExpression<string> CEquals { get; private set; }
		public IParsingExpression<string> CForwardSlash { get; private set; }
		public IParsingExpression<string> CGreaterThan { get; private set; }
		public IParsingExpression<string> CHash { get; private set; }
		public IParsingExpression<string> CLBrace { get; private set; }
		public IParsingExpression<string> CLessThan { get; private set; }
		public IParsingExpression<string> CLParen { get; private set; }
		public IParsingExpression<string> CSemiColon { get; private set; }
		public IParsingExpression<string> CLSquareBracket { get; private set; }
		public IParsingExpression<string> CMinus { get; private set; }
		public IParsingExpression<string> CPeriod { get; private set; }
		public IParsingExpression<string> CPipe { get; private set; }
		public IParsingExpression<string> CPlus { get; private set; }
		public IParsingExpression<string> CRBrace { get; private set; }
		public IParsingExpression<string> CRParen { get; private set; }
		public IParsingExpression<string> CRSquareBracket { get; private set; }
		public IParsingExpression<string> CSingleQuote { get; private set; }

		#region char sets

		private static readonly char[] specialCharValues =
			new char[] {
				'*', // strong, emphasis
				'&', // entities
				'\'', '"', // quotes
				'`', // ticks
				'/', // single-line comment
				'\\', // escape sequence
				'[', ']', // labels
				'<', '>', // autolinks
				'|', // table cell delimiter
				'@' // expressions
			};

		private static readonly char[] spaceCharValues =
			new char[] { ' ', '\t' };

		private static readonly char[] whitespaceCharValues =
			spaceCharValues
			.Concat(new char[] { '\n', '\r' })
			.ToArray();

		#endregion


		public MarkdomGrammar() {

			Define(() => Document,
				Reference(() => Blocks, match => new MarkdomDocumentNode(match.Product, MarkdomSourceRange.FromMatch(match))));

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
						Reference(() => ReferenceBlock),
						Reference(() => UnorderedList),
						Reference(() => Table),
						Reference(() => OrderedList),
						Reference(() => Paragraph)), 
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


			#region Ordered List

			var enumeratorCounterStyleLowerRomanCharValues = new char[] { 'i', 'v', 'x', 'l', 'c', 'd', 'm' };
			var enumeratorCounterStyleUpperRomanCharValues = enumeratorCounterStyleLowerRomanCharValues.Select(char.ToUpper);

			var enumeratorCounterStyleLowerRomanChar = CharacterIn(enumeratorCounterStyleLowerRomanCharValues);
			var enumeratorCounterStyleUpperRomanChar = CharacterIn(enumeratorCounterStyleUpperRomanCharValues);
			var enumeratorCounterStyleRomanChar = CharacterIn(enumeratorCounterStyleLowerRomanCharValues.Concat(enumeratorCounterStyleUpperRomanCharValues));

			var enumeratorValue =
				Optional(
					Sequence(
						Reference(() => CAt),
						AtLeast(1, Reference(() => Digit), match => match.String.ParseDefault(1)),
						match => match.Product.Of2),
					match => match.Product,
					noMatch => 1);

			var enumeratorCounterStyleDefinitions = new EnumeratorCounterStyleDefinition[] {
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.Decimal,
					AtLeast(1,
						Reference(() => Digit),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.Decimal, match.String.ParseDefault(1)))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.LowerRoman,
					Sequence(
						enumeratorCounterStyleLowerRomanChar,
						AtLeast(0, enumeratorCounterStyleRomanChar),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.LowerRoman, 1))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.UpperRoman,
					Sequence(
						enumeratorCounterStyleUpperRomanChar,
						AtLeast(0, enumeratorCounterStyleRomanChar),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.UpperRoman, 1))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.LowerAlpha,
					Sequence(
						Reference(() => EnglishLowerAlpha),
						AtLeast(0, Reference(() => EnglishAlpha)),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.LowerAlpha, 1))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.UpperAlpha,
					Sequence(
						Reference(() => EnglishUpperAlpha),
						AtLeast(0, Reference(() => EnglishAlpha)),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.UpperAlpha, 1)))
			};

			var enumeratorSeparatorStyleDefinitions = new EnumeratorSeparatorStyleDefinition[] {
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Dot, Literal(""), Literal(".")),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Dash, Literal(""), Sequence(Reference(() => SpaceChars), Literal("-"))),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Parenthesis, Literal(""), Literal(")")),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Bracketed, Literal("["), Literal("]")),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Parenthesized, Literal("("), Literal(")")),
			};

			var orderedListStyleDefinitions =
				enumeratorCounterStyleDefinitions.SelectMany(cd =>
				enumeratorSeparatorStyleDefinitions.Select(sd =>
					new OrderedListStyleDefinition(cd.CounterStyle, sd.SeparatorStyle,
						Sequence(
							Reference(() => NonIndentSpace), sd.Preceding, cd.Expression, enumeratorValue, sd.Following, Reference(() => SpaceChars),
							match => new InitialEnumeratorInfo(cd.CounterStyle, sd.SeparatorStyle,
								1 == match.Product.Of4 ? match.Product.Of3.SuggestedValue : match.Product.Of4)),
						Sequence(
							Reference(() => NonIndentSpace), sd.Preceding, cd.Expression, sd.Following, Reference(() => SpaceChars),
						match => new ContinuationEnumeratorInfo(cd.CounterStyle, sd.SeparatorStyle)))
				))
				.ToList();

			// This works because the continuation enumerator is a special case of the initial
			Define(() => Enumerator,
				ChoiceUnordered(orderedListStyleDefinitions.Select(ls => ls.InitialEnumerator)));

			Define(() => OrderedList,
				ChoiceOrdered(orderedListStyleDefinitions.Select(listStyle => {
					var orderedListBlockLine =
						Sequence(
							NotAhead(Reference(() => Enumerator)), listBlockLine,
							m => m.Product.Of2);

					var orderedListBlockLines = AtLeast(0, orderedListBlockLine);

					var initialOrderedListItemInitialBlock =
						Sequence(
							listStyle.InitialEnumerator,
							Reference(() => BlockLine),
							orderedListBlockLines,
							match => ArrayUtils.Prepend(match.Product.Of2, match.Product.Of3));

					var subsequentOrderedListItemInitialBlock =
						Sequence(
							listStyle.ContinuationEnumerator,
							Reference(() => BlockLine),
							orderedListBlockLines,
							match => ArrayUtils.Prepend(match.Product.Of2, match.Product.Of3));

					var orderedListItemSubsequentBlock =
						Sequence(
							listItemContinues,
							orderedListBlockLines,
							Reference(() => BlankLines),
							match => ArrayUtils.Combine(match.Product.Of1, match.Product.Of2, match.Product.Of3));

					var initialOrderedListItemTight =
						Reference( () => initialOrderedListItemInitialBlock, BlockLineInfo.FromMatch);

					var subsequentOrderedListItemTight =
						Reference(() => subsequentOrderedListItemInitialBlock, BlockLineInfo.FromMatch);

					var initialOrderedListItemLoose =
						Sequence(
							Sequence(
								initialOrderedListItemInitialBlock,
								AtLeast(0, orderedListItemSubsequentBlock, match => ArrayUtils.Flatten(match.Product)),
								match => new BlockLineInfo(ArrayUtils.Combine(match.Product.Of1, match.Product.Of2), match.SourceRange)),
							Reference(() => BlankLines), // chew up any blank lines after an initial block with no subsequent
							match => match.Product.Of1);

					var subsequentOrderedListItemLoose = 
						Sequence(
							Sequence(
								subsequentOrderedListItemInitialBlock,
								AtLeast(0, orderedListItemSubsequentBlock, match => ArrayUtils.Flatten(match.Product)),
								match => new BlockLineInfo(ArrayUtils.Combine(match.Product.Of1, match.Product.Of2), match.SourceRange)),
							Reference(() => BlankLines), // chew up any blank lines after an initial block with no subsequent
							match => match.Product.Of1);

					var orderedListContinuesLoose =
						ChoiceUnordered(
							Sequence(Reference(() => BlankLines), listStyle.ContinuationEnumerator),
							listItemContinues);

					var orderedListTight =
						Sequence(
							Reference(() => BlankLines),
							initialOrderedListItemTight,
							AtLeast(0, subsequentOrderedListItemTight),
							NotAhead(orderedListContinuesLoose),
							match => {
								var items = match.Product.Of2.InArray().Concat(match.Product.Of3)
									.Select(itemBlockInfo =>
										new OrderedListItemNode(
											ParseLinesAs(Inlines, itemBlockInfo.Lines),
											new MarkdomSourceRange(itemBlockInfo.SourceRange.Index, itemBlockInfo.SourceRange.Length, itemBlockInfo.SourceRange.Line, itemBlockInfo.SourceRange.LineIndex)))
									.ToArray();
								return new OrderedListNode(listStyle.CounterStyle, listStyle.SeparatorStyle, 1, items, MarkdomSourceRange.FromMatch(match));
							});

					var orderedListLoose =
						Sequence(
							Reference(() => BlankLines),
							initialOrderedListItemLoose,
							AtLeast(0, subsequentOrderedListItemLoose),
							match => {
								var items = match.Product.Of2.InArray().Concat(match.Product.Of3)
									.Select(itemBlockInfo =>
										new OrderedListItemNode(
											ParseLinesAs(Blocks, itemBlockInfo.Lines),
											new MarkdomSourceRange(itemBlockInfo.SourceRange.Index, itemBlockInfo.SourceRange.Length, itemBlockInfo.SourceRange.Line, itemBlockInfo.SourceRange.LineIndex)))
									.ToArray();
								return new OrderedListNode(listStyle.CounterStyle, listStyle.SeparatorStyle, 1, items, MarkdomSourceRange.FromMatch(match));
							});

					return ChoiceOrdered(orderedListTight, orderedListLoose);
			})));
		#endregion

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
					ChoiceUnordered(
						Reference(() => CAsterix),
						Reference(() => CMinus),
						Reference(() => CPlus)),
					AtLeast(1, Reference(() => SpaceChar))));

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
					BlockLineInfo.FromMatch);

			var unorderedListItemLoose =
				Sequence(
					Sequence(
						unorderedListItemInitialBlock,
						AtLeast(0, unorderedListItemSubsequentBlock, match => ArrayUtils.Flatten(match.Product)),
						match => new BlockLineInfo(ArrayUtils.Combine(match.Product.Of1, match.Product.Of2), match.SourceRange)),
					Reference(() => BlankLines), // chew up any blank lines after an initial block with no subsequent
					match => match.Product.Of1);

			var unorderedListContinuesLoose =
				ChoiceUnordered(
					Sequence(Reference(() => BlankLines), Reference(() => Bullet)),
					listItemContinues);

			Define(() => UnorderedListTight,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(1, unorderedListItemTight),
					NotAhead(unorderedListContinuesLoose),
					match => {
						var items = match.Product.Of2
							.Select(itemBlockInfo =>
								new UnorderedListItemNode(
									ParseLinesAs(Inlines, itemBlockInfo.Lines),
									new MarkdomSourceRange(itemBlockInfo.SourceRange.Index, itemBlockInfo.SourceRange.Length, itemBlockInfo.SourceRange.Line, itemBlockInfo.SourceRange.LineIndex)))
							.ToArray();
						return new UnorderedListNode(items, MarkdomSourceRange.FromMatch(match));
					}));

			Define(() => UnorderedListLoose,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(1, unorderedListItemLoose),
					match => {
						var items = match.Product.Of2
							.Select(itemBlockInfo =>
								new UnorderedListItemNode(
									ParseLinesAs(Blocks, itemBlockInfo.Lines),
									new MarkdomSourceRange(itemBlockInfo.SourceRange.Index, itemBlockInfo.SourceRange.Length, itemBlockInfo.SourceRange.Line, itemBlockInfo.SourceRange.LineIndex)))
							.ToArray();
						return new UnorderedListNode(items, MarkdomSourceRange.FromMatch(match));
					}));

			#endregion

			#endregion

			#region Headings
			Define(() => Heading,
				Sequence(
					Reference(() => HeadingAnnouncement),
					Reference(() => SpaceChars),
					Reference(() => BlockLine),
					match => new HeadingNode(match.Product.Of3.LineString, match.Product.Of1, MarkdomSourceRange.FromMatch(match))));

			Define(() => HeadingAnnouncement,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Literal("#"), match => match.Product.Length),
					match => match.Product.Of2));
			#endregion

			Define(() => ReferenceBlock,
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => ReferenceLabel),
					Reference(() => SpaceChars),
					Reference(() => CColon),
					Reference(() => SpaceChars),
					ChoiceUnordered(
						Reference(() => UriLiteralExpression, match => match.Product.InArray()),
						Reference(() => ArgumentList)),
					match => new ReferenceNode(match.Product.Of2, match.Product.Of6, MarkdomSourceRange.FromMatch(match))));

			#region Paragraph

			Define(() => Paragraph,
				AtLeast(1,
					Reference(() => NonEmptyBlockLine),
					match => new ParagraphNode(ParseLinesAs(Inlines, match.Product), MarkdomSourceRange.FromMatch(match))));

			#endregion

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

			Define(() => BlockLineAtomic,
				Sequence(
					NotAhead(Reference(() => NewLine)),
					Reference(() => Atomic)));

			Define(() => Atomic,
				ChoiceOrdered(
					CharacterNotIn(specialCharValues),
					ChoiceUnordered(
						Reference(() => SingleLineComment),
						Reference(() => MultiLineComment),
						Reference(() => InlineExpression)),
					Wildcard()));

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
					Sequence(
						NotAhead(Reference(() => CPipe)),
						Reference(() => BlockLineAtomic)),
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
					Reference(() => CEquals),
					Reference(() => SpaceChars),
					match => match.Product.Of1);

			var tableDataCellAnnouncement =
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
							ParseLinesAs(Inlines, match.Product.Of2.InArray()),
							MarkdomSourceRange.FromMatch(match)));

			var tableDataCell =
				Sequence(
					tableDataCellAnnouncement,
					tableCellContents,
					match => new TableDataCellNode(
						match.Product.Of1.ColumnSpan,
						match.Product.Of1.RowSpan,
						ParseLinesAs(Inlines, match.Product.Of2.InArray()),
						MarkdomSourceRange.FromMatch(match)));

			var tableRowEnd =
				Sequence(
					Optional(Reference(() => CPipe)),
					Reference(() => BlankLine));

			var tableCell =
				Sequence(
					NotAhead(tableRowEnd),
					ChoiceOrdered<TableCellNode>(
						tableHeaderCell,
						tableDataCell),
					match => match.Product.Of2);

			Define(() => TableRow,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Reference(() => tableCell)),
					tableRowEnd,
					match => new TableRowNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => TableRowSeparator,
				Sequence(
					Reference(() => SpaceChars),
					CharacterIn(new char[] { '+', '-', '=' }),
					AtLeast(0, CharacterIn(new char[] { '+', '-', '=', ' ', '\t' })),
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
						Reference(() => InlineExpression),
						Reference(() => LineBreak)),
					Reference(() => Symbol)));


			#region AutoLink

			Define(() => AutoLink,
				Sequence(
					Reference(() => CLessThan),
					Reference(() => SpaceChars),
					Reference(() => UriLiteralExpression),
					Reference(() => SpaceChars),
					Reference(() => CGreaterThan),
					Optional(
						Reference(() => ArgumentList),
						match => match.Product,
						noMatch => new IExpression[0]),
					match => new AutoLinkNode(match.Product.Of3, match.Product.Of6, MarkdomSourceRange.FromMatch(match))));
			#endregion

			#region Link

			Define(() => Link,
				Sequence(
					Reference(() => Label),
					Reference(() => SpaceChars),
					ChoiceOrdered(
						Sequence(
							Optional(Reference(() => ReferenceLabel)),
							Reference(() => SpaceChars),
							Reference(() => ArgumentList),
							match => Tuple.Create(match.Product.Of1, match.Product.Of3)),
						Reference(
							() => ReferenceLabel,
							match => Tuple.Create(match.Product, new IExpression[0]))),
					match => new LinkNode(match.Product.Of1, match.Product.Of3.Item1, match.Product.Of3.Item2, MarkdomSourceRange.FromMatch(match))));

			Define(() => ReferenceLabel,
				Sequence(
					Reference(() => CLSquareBracket),
					AtLeast(0, CharacterNotIn(new char[] { ']', '\n', '\r' }), match => match.String),
					Reference(() => CRSquareBracket),
					match => ReferenceId.FromText(match.Product.Of2)));

			Define(() => Label,
				Sequence(
					Reference(() => CLSquareBracket),
					AtLeast(0, Sequence(NotAhead(Reference(() => CRSquareBracket)), Reference(() => Inline), match => match.Product.Of2)),
					Reference(() => CRSquareBracket),
					match => match.Product.Of2));

			#endregion

			Define(() => InlineExpression,
				Sequence(
					Reference(() => CAt),
					Reference(() => Expression),
					Optional(Reference(() => CSemiColon)),
					match => new InlineExpressionNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => Strong,
				Sequence(
					Literal("**"),
					AtLeast(0, Sequence(NotAhead(Literal("**")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("**")),
					match => new StrongNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => Emphasis,
				Sequence(
					Reference(() => CAsterix),
					AtLeast(0, Sequence(NotAhead(Reference(() => CAsterix)), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Reference(() => CAsterix)),
					match => new EmphasisNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			var singleQuoted =
				Sequence(
					Reference(() => CSingleQuote),
					AtLeast(0, Sequence(NotAhead(Reference(() => CSingleQuote)), Reference(() => Inline), match => match.Product.Of2)),
					Reference(() => CSingleQuote),
					match => new QuotedNode(QuoteType.Single, match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			var doubleQuoted =
				Sequence(
					Reference(() => CDoubleQuote),
					AtLeast(0, Sequence(NotAhead(Reference(() => CDoubleQuote)), Reference(() => Inline), match => match.Product.Of2)),
					Reference(() => CDoubleQuote),
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
					Reference(() => CSemiColon),
					match => new EntityNode(int.Parse(match.Product.Of2), MarkdomSourceRange.FromMatch(match)));

			var hexHtmlEntity =
				Sequence(
					Literal("&#x"),
					Between(1, 6, Reference(() => HexDigit), match => match.String),
					Reference(() => CSemiColon),
					match => new EntityNode(Convert.ToInt32(match.Product.Of2, 16), MarkdomSourceRange.FromMatch(match)));

			// Because of the large number of named entities it is much faster to use a dynamic
			// expression with an assertion to match valid entity names
			var namedHtmlEntity =
				Dynamic(() => {
					string entityName = null;
					return Sequence(
						Reference(() => CAmpersand),
						Between(1, 32, Reference(() => EnglishAlpha), match => { return entityName = match.String; }),
						Assert(() => EntityNode.IsEntityName(entityName)),
						Reference(() => CSemiColon),
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

			Define(() => NormalChar,
				CharacterNotIn(whitespaceCharValues.Concat(specialCharValues)));

			Define(() => Symbol,
				Reference(() => SpecialChar, match => new SymbolNode(match.String, MarkdomSourceRange.FromMatch(match))));

			Define(() => SpecialChar,
				CharacterIn(specialCharValues));

			#endregion

			#region Expressions

			Define(() => Expression,
				Reference(() => LiteralExpression));

			Define(() => LiteralExpression,
				ChoiceOrdered<IExpression>(
					Reference(() => NumericLiteralExpression),
					Reference(() => StringLiteralExpression),
					Reference(() => ObjectLiteralExpression),
					Reference(() => UriLiteralExpression),
					Reference(() => DocumentLiteralExpression)));
			
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
					match => match.Product.Of2 ?? new IExpression[0]));

			#region NumberExpression

			var decimalIntegerLiteral =
				ChoiceUnordered(
					Literal("0"),
					Sequence(
						Reference(() => NonZeroDigit),
						AtLeast(0, Reference(() => Digit))),
					match => double.Parse(match.String));

			var signedInteger =
				Sequence(
					Optional(
						ChoiceUnordered(
							Reference(() => CPlus, match => 1d),
							Reference(() => CMinus, match => -1d)),
						match => match.Product,
						noMatch => 1d),
					AtLeast(1, Reference(() => Digit), match => double.Parse(match.String)),
					match => match.Product.Of1 * match.Product.Of2);

			var hexIntegerLiteral =
				Sequence(
					Literal("0x"),
					AtLeast(1, Reference(() => HexDigit), match => match.String),
					match => new NumericLiteralExpression(
						(double)Convert.ToInt64(match.String, 16),
						MarkdomSourceRange.FromMatch(match)));

			var optionalExponentPart =
				Optional(
					Sequence(
						ChoiceUnordered(Literal("e"), Literal("E")),
						signedInteger,
						match => match.Product.Of2),
					match => Math.Pow(10d, match.Product),
					noMatch => 1d);

			var optionalDecimalPart =
				Optional(
					Sequence(
						Reference(() => CPeriod),
						AtLeast(0, Reference(() => Digit))),
					match => double.Parse("0" + match.String),
					noMatch => 0d);

			var requiredDecimalPart =
				Sequence(
					Reference(() => CPeriod),
					AtLeast(1, Reference(() => Digit)),
				match => double.Parse("0" + match.String));

			var decimalLiteral = 
				ChoiceUnordered(
					Sequence(
						decimalIntegerLiteral,
						optionalDecimalPart,
						optionalExponentPart,
						m => new NumericLiteralExpression(
							(m.Product.Of1 + m.Product.Of2) * m.Product.Of3,
							MarkdomSourceRange.FromMatch(m))),
					Sequence(
						requiredDecimalPart,
						optionalExponentPart,
						m => new NumericLiteralExpression(
							m.Product.Of1 * m.Product.Of2,
							MarkdomSourceRange.FromMatch(m))));

			Define(() => NumericLiteralExpression,
				ChoiceOrdered<NumericLiteralExpression>(
					hexIntegerLiteral,
					decimalLiteral));

			#endregion

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
							NotAhead(ChoiceUnordered(Reference(() => CSingleQuote), Reference(() => NewLine))),
							Wildcard(),
							match => match.String)),
					match => match.Product.Join());

			var doubleQuotedStringExpressionContent =
				AtLeast(0,
					ChoiceUnordered(
						Literal("\\\"", match => "\""),
						stringExpressionEscapes,
						Sequence(
							NotAhead(ChoiceUnordered(Reference(() => CDoubleQuote), Reference(() => NewLine))),
							Wildcard(),
							match => match.String)),
					match => match.Product.Join());

			var singleQuotedStringExpression =
				Sequence(
					Reference(() => CSingleQuote), singleQuotedStringExpressionContent, Reference(() => CSingleQuote),
					match => new StringLiteralExpression(match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			var doubleQuotedStringExpression =
				Sequence(
					Reference(() => CDoubleQuote), doubleQuotedStringExpressionContent, Reference(() => CDoubleQuote),
					match => new StringLiteralExpression(match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			Define(() => StringLiteralExpression,
				ChoiceUnordered(
					singleQuotedStringExpression,
					doubleQuotedStringExpression));

			#endregion

			#region ObjectExpression

			var objectPropertyAssignment =
				Sequence(
					Reference(() => StringLiteralExpression), // TODO: Identifier / String / Uri
					Reference(() => ExpressionWhitespace),
					Reference(() => CColon),
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
				Sequence( Reference(() => CLBrace), Reference(() => ExpressionWhitespace));

			var objectExpressionEnd =
				Sequence( Reference(() => ExpressionWhitespace), Reference(() => CRBrace));

			Define(() => ObjectLiteralExpression,
				Sequence(
					objectExpressionStart, objectPropertyAssignments, objectExpressionEnd,
					match => new ObjectLiteralExpression(match.Product.Of2 ?? new PropertyAssignment[0], MarkdomSourceRange.FromMatch(match))));

			Define(() => ObjectBodyExpression,
				Reference(() => objectPropertyAssignments, match => new ObjectLiteralExpression(match.Product, MarkdomSourceRange.FromMatch(match))));

			#endregion

			#region UriExpression
			//   * Cannot contain `,` (commas), which are ordinarily legal URI characters.
			//     This is to disambiguate URI expressions in argument lists
			//   * Parentheses inside of a URI expression must be balanced
			//   * Cannot start with `@`, `'`, or `"'

			IParsingExpression<object> uriExpressionPart = null;
			IParsingExpression<object[]> uriExpressionRegularPart = null;
			IParsingExpression<Nil> uriExpressionParenthesizedPart = null;

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
						CharacterIn(new char[] { '/', '?', ':', '@', '&', '=', '+', '$', '-', '_', '!', '~', '*', '\'', '.', ';' }),
						Sequence(
							Literal("%"),
							Exactly(2, Reference(() => HexDigit)))));

			uriExpressionParenthesizedPart =
				Sequence(
					Reference(() => CLParen),
					AtLeast(0, Reference(() => uriExpressionPart)),
					Reference(() => CRParen));

			Define(() => UriLiteralExpression,
				AtLeast(1,
					ChoiceUnordered(
						Reference(() => uriExpressionRegularPart),
						Reference(() => uriExpressionParenthesizedPart)),
					match => new UriLiteralExpression(match.String, MarkdomSourceRange.FromMatch(match))));

			#endregion

			#region MarkdomExpression

			Define(() => DocumentLiteralExpression,
				Dynamic(() => {
					IParsingExpression<string> endBraces = null;
					return Sequence(
						AtLeast(2, Reference(() => CLBrace), m => { endBraces = Literal("".PadLeft(m.Length, '}')); return Nil.Value; }),
						AtLeast(0,
							Sequence(NotAhead(Reference(() => endBraces)), Reference(() => Atomic)),
							match => LineInfo.FromMatch(match)),
						Reference(() => endBraces),
						match => new DocumentLiteralExpression(
							ParseLinesAs(Blocks, match.Product.Of2.InArray()),
							MarkdomSourceRange.FromMatch(match)));
				}));

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
					Ahead(ChoiceUnordered("IVXLCDM".Select(Literal))),
					Reference(() => RomanNumeral),
					match => match.Product.Of2));

			Define(() => LowerRomanNumeral,
				Sequence(
					Ahead(ChoiceUnordered("ivxlcdm".Select(Literal))),
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
				CharacterIn(spaceCharValues));

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
				AtLeast(0,
					CharacterIn(whitespaceCharValues)));

			Define(() => Digit,
				CharacterInRange('0', '9'));

			Define(() => NonZeroDigit,
				CharacterInRange('1', '9'));

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

			#region Characters

			Define(() => CAmpersand, Literal("&"));
			Define(() => CAsterix, Literal("*"));
			Define(() => CAt, Literal("@"));
			Define(() => CBackSlash, Literal("\\"));
			Define(() => CColon, Literal(":"));
			Define(() => CComma, Literal(","));
			Define(() => CDoubleQuote, Literal("\""));
			Define(() => CEquals, Literal("="));
			Define(() => CForwardSlash, Literal("/"));
			Define(() => CGreaterThan, Literal(">"));
			Define(() => CHash, Literal("#"));
			Define(() => CLBrace, Literal("{"));
			Define(() => CLessThan, Literal("<"));
			Define(() => CLParen, Literal("("));
			Define(() => CPeriod, Literal("."));
			Define(() => CLSquareBracket, Literal("["));
			Define(() => CMinus, Literal("-"));
			Define(() => CPipe, Literal("|"));
			Define(() => CPlus, Literal("+"));
			Define(() => CRBrace, Literal("}"));
			Define(() => CRParen, Literal(")"));
			Define(() => CSemiColon, Literal(";"));
			Define(() => CRSquareBracket, Literal("]"));
			Define(() => CSingleQuote, Literal("'"));

			#endregion

			#endregion
		}

		private int _parseLinesCount = 0;
		private T ParseLinesAs<T>(IParsingExpression<T> expression, LineInfo[] lines) {
			_parseLinesCount++;
			var expressionMatchingContext =
				new MatchingContext(
					lines.Select(line => line.LineString).Join(),
					lines.Select(line => line.SourceRange).ToArray());
			
			var expressionMatchingResult = expression.Matches(expressionMatchingContext);

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
