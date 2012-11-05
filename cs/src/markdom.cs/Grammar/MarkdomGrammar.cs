﻿using markdom.cs.Model;
using markdom.cs.Model.Expressions;
using markdom.cs.Model.Nodes;
using pegleg.cs;
using pegleg.cs.Parsing;
using pegleg.cs.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace markdom.cs.Grammar {
	public class MarkdomGrammar : Grammar<MarkdomDocumentNode> {
		public IParsingExpression<LineInfo> Comment { get; private set; }
		public IParsingExpression<LineInfo> MultiLineComment { get; private set; }
		public IParsingExpression<LineInfo> SingleLineComment { get; private set; }

		public IParsingExpression<MarkdomDocumentNode> Document { get; private set; }

		public IParsingExpression<IEnumerable<IBlockNode>> Blocks { get; private set; }
		public IParsingExpression<IBlockNode> Block { get; private set; }

		public IParsingExpression<LineInfo> CommentBlock { get; private set; }
		public IParsingExpression<BlockquoteNode> Blockquote { get; private set; }
		public IParsingExpression<TableNode> Table { get; private set; }
		public IParsingExpression<TableRowNode> TableRow { get; private set; }
		public IParsingExpression<LineInfo> TableRowSeparator { get; private set; }
		public IParsingExpression<HeadingNode> Heading { get; private set; }
		public IParsingExpression<int> HeadingAnnouncement { get; private set; }
		public IParsingExpression<OrderedListNode> OrderedList { get; private set; }
		public IParsingExpression<Nil> Enumerator { get; private set; }
		public IParsingExpression<Nil> EnumeratorishAhead { get; private set; }
		public IParsingExpression<int?> EnumeratorValue { get; private set; }
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

		public IParsingExpression<IEnumerable<IInlineNode>> Inlines { get; private set; }
		public IParsingExpression<IInlineNode> Inline { get; private set; }
		public IParsingExpression<AutoLinkNode> AutoLink { get; private set; }
		public IParsingExpression<LinkNode> Link { get; private set; }
		public IParsingExpression<IEnumerable<IInlineNode>> Label { get; private set; }
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
		public IParsingExpression<IEnumerable<IExpression>> ArgumentList { get; private set; }
		public IParsingExpression<ObjectLiteralExpression> ObjectLiteralExpression { get; private set; }
		public IParsingExpression<ObjectLiteralExpression> ObjectBodyExpression { get; private set; }
		public IParsingExpression<NumericLiteralExpression> NumericLiteralExpression { get; private set; }
		public IParsingExpression<StringLiteralExpression> StringLiteralExpression { get; private set; }
		public IParsingExpression<UriLiteralExpression> UriLiteralExpression { get; private set; }
		public IParsingExpression<DocumentLiteralExpression> DocumentLiteralExpression { get; private set; }
		public IParsingExpression<Nil> ExpressionWhitespace { get; private set; }

		/// <summary>
		/// A tab or a space.
		/// </summary>
		public IParsingExpression<Nil> SpaceChar { get; private set; }
		public IParsingExpression<string> SpaceChars { get; private set; }
		/// <summary>
		/// A whitespace character; space, tab or newline.
		/// </summary>
		public IParsingExpression<Nil> Whitespace { get; private set; }
		public IParsingExpression<IEnumerable<Nil>> Whitespaces { get; private set; }
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
		public IParsingExpression<IEnumerable<LineInfo>> BlankLines { get; private set; }
		public IParsingExpression<LineInfo> BlankLine { get; private set; }
		public IParsingExpression<LineInfo> NonTerminalBlankLine { get; private set; }
		public IParsingExpression<Nil> Digit { get; private set; }
		public IParsingExpression<Nil> NonZeroDigit { get; private set; }
		public IParsingExpression<Nil> HexDigit { get; private set; }
		public IParsingExpression<Nil> EnglishLowerAlpha { get; private set; }
		public IParsingExpression<Nil> EnglishUpperAlpha { get; private set; }
		public IParsingExpression<Nil> EnglishAlpha { get; private set; }

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
				Reference(() => Blocks, match => new MarkdomDocumentNode(match.Product.ToArray(), MarkdomSourceRange.FromMatch(match))));

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
						Reference(() => Blockquote),
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

			Define(() => EnumeratorValue,
				Optional(
					Sequence(
						Literal("@"),
						AtLeast(1, Reference(() => Digit), match => match.String.ParseDefault(1)),
						match => match.Product.Of2),
					match => new int?(match.Product),
					noMatch => new int?()));

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
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.LowerRoman, NumeralUtils.ParseRomanNumeral(match.String)))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.UpperRoman,
					Sequence(
						enumeratorCounterStyleUpperRomanChar,
						AtLeast(0, enumeratorCounterStyleRomanChar),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.UpperRoman, NumeralUtils.ParseRomanNumeral(match.String)))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.LowerAlpha,
					Sequence(
						Reference(() => EnglishLowerAlpha),
						AtLeast(0, Reference(() => EnglishAlpha)),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.LowerAlpha, NumeralUtils.ParseAlphaNumeral(match.String)))),
				new EnumeratorCounterStyleDefinition(
					OrderedListCounterStyle.UpperAlpha,
					Sequence(
						Reference(() => EnglishUpperAlpha),
						AtLeast(0, Reference(() => EnglishAlpha)),
						match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.UpperAlpha, NumeralUtils.ParseAlphaNumeral(match.String))))
			};

			var enumeratorSeparatorStyleDefinitions = new EnumeratorSeparatorStyleDefinition[] {
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Dot, Literal(""), Literal(".")),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Dash, Literal(""), Sequence(Reference(() => SpaceChars), Literal("-"))),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Parenthesis, Literal(""), Literal(")")),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Bracketed, Literal("["), Literal("]")),
				new EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle.Parenthesized, Literal("("), Literal(")")),
			};

			// Here we expand the separator style definitions with the counter style definitions into an
			// intermediate data structure containing the enumerator rules for each separator & counter
			// style combination. We will use this to build the ordered list rules without having to construct
			// duplicate rules.
			var enumeratorStyleDefinitions =
				enumeratorSeparatorStyleDefinitions.Select(sd => {
					var enumeratorPreamble = Sequence(Reference(() => NonIndentSpace), sd.Preceding);
					var enumeratorPostamble = Sequence(sd.Following, Reference(() => SpaceChars));
					return new {
						SeparatorStyle = sd.SeparatorStyle,
						Preceding = sd.Preceding,
						Following = sd.Following,
						EnumeratorPreamble = enumeratorPreamble,
						EnumeratorPostamble = enumeratorPostamble,
						Counters = // Enumerator rules for the current separator & counter style
							enumeratorCounterStyleDefinitions.Select(cd => new {
								CounterStyle = cd.CounterStyle,	
								Expression = cd.Expression,
								InitialEnumerator =
									Named("InitialEnumerator" + cd.CounterStyle.ToString() + sd.SeparatorStyle.ToString(),
										Sequence(
											enumeratorPreamble, cd.Expression, Reference(() => EnumeratorValue), enumeratorPostamble,
											match => new EnumeratorInfo(
												cd.CounterStyle, sd.SeparatorStyle,
												match.Product.Of3.ValueOr(match.Product.Of2.InterpretedValue.ValueOr(1))))),
								ContinuationEnumerator =
									Named("ContinuationEnumerator" + cd.CounterStyle.ToString() + sd.SeparatorStyle.ToString(),
										Sequence(
											enumeratorPreamble, cd.Expression, enumeratorPostamble))
							})
							.ToList()
					};
				})
				.ToList();

			// This works because the continuation enumerator is a special case of the initial
			Define(() => Enumerator,
				Sequence(
					Reference(() => EnumeratorishAhead),
					ChoiceUnordered(
						enumeratorStyleDefinitions.Select(sd =>
							Sequence(
								Ahead(sd.EnumeratorPreamble), 
								ChoiceOrdered(sd.Counters.Select(cd => cd.InitialEnumerator)))
						))));

			Define(() => EnumeratorishAhead,
				Ahead(
					Regex(@"\ {0,3}(\(|\[)?([0-9]+|[a-zA-Z]+)(@[0-9]+)?(\.|(\ *-)|\)|\])")));

			var orderedListBlockLine =
				Sequence(
					NotAhead(Reference(() => Enumerator)), listBlockLine,
					m => m.Product.Of2);

			var orderedListBlockLines = AtLeast(0, orderedListBlockLine);

			var orderedListItemSubsequentBlock =
				Sequence(
					listItemContinues,
					orderedListBlockLines,
					match => match.Product.Of1.Concat(match.Product.Of2));

			Define(() => OrderedList,
				Sequence(
					Reference(() => EnumeratorishAhead),
					// Separator style is unambiguous, so we can use unordered choice
					ChoiceUnordered(enumeratorStyleDefinitions.Select(ssd =>
						Sequence(
							// Perform early abort of match if the separator style will not match
							Ahead(ssd.EnumeratorPreamble),
							ChoiceOrdered(ssd.Counters.Select(csd => {
								var initialOrderedListItemTight =
									Sequence(
										csd.InitialEnumerator, Reference(() => BlockLine),
										orderedListBlockLines,
										match => new OrderedListItemLineInfo(
											match.Product.Of1,
											match.Product.Of2.InEnumerable().Concat(match.Product.Of3),
											match.SourceRange));

								var subsequentOrderedListItemTight =
									Sequence(
										csd.ContinuationEnumerator, Reference(() => BlockLine),
										orderedListBlockLines,
										match => new BlockLineInfo(
											match.Product.Of2.InArray().Concat(match.Product.Of3),
											match.SourceRange));

								var initialOrderedListItemLoose =
									Sequence(
										initialOrderedListItemTight,
										AtLeast(0, orderedListItemSubsequentBlock),
										match => new OrderedListItemLineInfo(
											match.Product.Of1.Enumerator,
											match.Product.Of1.Lines.Concat(match.Product.Of2.Flatten()),
											match.SourceRange));

								var subsequentOrderedListItemLoose = 
									Sequence(
										subsequentOrderedListItemTight,
										AtLeast(0, orderedListItemSubsequentBlock),
										match => new BlockLineInfo(
											match.Product.Of1.Lines.Concat(match.Product.Of2.Flatten()),
											match.SourceRange));

								var orderedListContinuesLoose =
									ChoiceUnordered(
										Sequence(Reference(() => BlankLines), csd.ContinuationEnumerator),
										listItemContinues);

								var orderedListTight =
									Sequence(
										initialOrderedListItemTight,
										AtLeast(0, subsequentOrderedListItemTight),
										NotAhead(orderedListContinuesLoose),
										match => {
											var items = match.Product.Of1.InEnumerable().Concat(match.Product.Of2)
												.Select(i => new OrderedListItemNode(ParseLinesAs(Inlines, i.Lines).ToArray(), MarkdomSourceRange.FromSource(i.SourceRange)))
												.ToArray();
											return new OrderedListNode(
												csd.CounterStyle, ssd.SeparatorStyle,
												match.Product.Of1.Enumerator.Value,
												items,
												MarkdomSourceRange.FromMatch(match));
										});
								
								var orderedListLoose =
									Sequence(
										initialOrderedListItemLoose,
										AtLeast(0,
											Sequence(
												Reference(() => BlankLines),
												subsequentOrderedListItemLoose,
												match => match.Product.Of2)),
										NotAhead(orderedListContinuesLoose),
										match => {
											var items = match.Product.Of1.InEnumerable().Concat(match.Product.Of2)
												.Select(i => new OrderedListItemNode(ParseLinesAs(Blocks, i.Lines).ToArray(), MarkdomSourceRange.FromSource(i.SourceRange)))
												.ToArray();
											return new OrderedListNode(
												csd.CounterStyle, ssd.SeparatorStyle,
												match.Product.Of1.Enumerator.Value,
												items,
												MarkdomSourceRange.FromMatch(match));
										});

								return
									Named(
										"OrderedList" + csd.CounterStyle.ToString() + ssd.SeparatorStyle.ToString(),
										ChoiceOrdered(orderedListTight, orderedListLoose));
						})),
						match => match.Product.Of2)
					)),
					match => match.Product.Of2
				)
			);
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
					CharacterIn(new char[] { '*', '-', '+' }),
					AtLeast(1, Reference(() => SpaceChar))));

			var unorderedListBlockLine =
				Sequence(
					NotAhead(Reference(() => Bullet)), listBlockLine,
					match => match.Product.Of2);

			var unorderedListBlockLines = AtLeast(0, unorderedListBlockLine);

			var unorderedListItemTight =
				Sequence(
					Reference(() => Bullet), Reference(() => BlockLine),
					unorderedListBlockLines,
					match => new BlockLineInfo(match.Product.Of2.InEnumerable().Concat(match.Product.Of3), match.SourceRange));

			var unorderedListItemSubsequentBlock =
				Sequence(
					listItemContinues,
					unorderedListBlockLines,
					match => match.Product.Of1.Concat(match.Product.Of2));

			var unorderedListItemLoose =
				Sequence(
					unorderedListItemTight,
					AtLeast(0, unorderedListItemSubsequentBlock),
					match => new BlockLineInfo(match.Product.Of1.Lines.Concat(match.Product.Of2.Flatten()), match.SourceRange));

			var unorderedListContinuesLoose =
				ChoiceUnordered(
					Sequence(Reference(() => BlankLines), Reference(() => Bullet)),
					listItemContinues);

			Define(() => UnorderedListTight,
				Sequence(
					AtLeast(1, unorderedListItemTight),
					NotAhead(unorderedListContinuesLoose),
					match => {
						var items = match.Product.Of1
							.Select(i => new UnorderedListItemNode(ParseLinesAs(Inlines, i.Lines.ToArray()).ToArray(), MarkdomSourceRange.FromSource(i.SourceRange)))
							.ToArray();
						return new UnorderedListNode(items, MarkdomSourceRange.FromMatch(match));
					}));

			Define(() => UnorderedListLoose,
				Sequence(
					unorderedListItemLoose,
					AtLeast(0,
						Sequence(
							Reference(() => BlankLines),
							unorderedListItemLoose,
							match => match.Product.Of2)),
					match => {
						var items = match.Product.Of1.InEnumerable().Concat(match.Product.Of2)
							.Select(i => new UnorderedListItemNode(ParseLinesAs(Blocks, i.Lines).ToArray(), MarkdomSourceRange.FromSource(i.SourceRange)))
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
					AtLeast(1, Literal("#"), match => match.String.Length),
					match => match.Product.Of2));
			#endregion

			#region Blockquote

			var blockquoteAnnouncement =
				Sequence(Literal(">"), Optional(Literal(" ")));

			var blockquoteAnnouncedLine =
				Sequence(
					blockquoteAnnouncement, Reference(() => BlockLine),
					match => match.Product.Of2);

			var blockquotePart =
				Sequence(
					blockquoteAnnouncedLine,
					AtLeast(0,
						ChoiceOrdered(
							blockquoteAnnouncedLine,
							Reference(() => NonEmptyBlockLine))),
					match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2));

			Define(() => Blockquote,
				Sequence(
					blockquotePart,
					AtLeast(0,
						Sequence(
							Reference(() => BlankLines),
							blockquotePart,
							match => match.Product.Of1.Concat(match.Product.Of2))),
					match => new BlockquoteNode(
						ParseLinesAs(Blocks, match.Product.Of1.Concat(match.Product.Of2.Flatten())).ToArray(),
						MarkdomSourceRange.FromMatch(match))));

			#endregion

			Define(() => ReferenceBlock,
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => ReferenceLabel),
					Reference(() => SpaceChars),
					Literal(":"),
					Reference(() => SpaceChars),
					ChoiceUnordered(
						Reference(() => UriLiteralExpression, match => match.Product.InArray()),
						Reference(() => ArgumentList)),
					match => new ReferenceNode(match.Product.Of2, match.Product.Of6.ToArray(), MarkdomSourceRange.FromMatch(match))));

			#region Paragraph

			Define(() => Paragraph,
				AtLeast(1,
					Reference(() => NonEmptyBlockLine),
					match => new ParagraphNode(ParseLinesAs(Inlines, match.Product.ToArray()).ToArray(), MarkdomSourceRange.FromMatch(match))));

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
					match => new TableNode(match.Product.ToArray(), MarkdomSourceRange.FromMatch(match))));

			var tableCellContents =
				AtLeast(0,
					Sequence(
						NotAhead(Literal("|")),
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
					Literal("="),
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
							ParseLinesAs(Inlines, match.Product.Of2.InArray()).ToArray(),
							MarkdomSourceRange.FromMatch(match)));

			var tableDataCell =
				Sequence(
					tableDataCellAnnouncement,
					tableCellContents,
					match => new TableDataCellNode(
						match.Product.Of1.ColumnSpan,
						match.Product.Of1.RowSpan,
						ParseLinesAs(Inlines, match.Product.Of2.InArray()).ToArray(),
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

			Define(() => TableRow,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Reference(() => tableCell)),
					tableRowEnd,
					match => new TableRowNode(match.Product.Of2.ToArray(), MarkdomSourceRange.FromMatch(match))));

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
					Literal("<"),
					Reference(() => SpaceChars),
					Reference(() => UriLiteralExpression),
					Reference(() => SpaceChars),
					Literal(">"),
					Optional(
						Reference(() => ArgumentList),
						match => match.Product,
						noMatch => new IExpression[0]),
					match => new AutoLinkNode(match.Product.Of3, match.Product.Of6.ToArray(), MarkdomSourceRange.FromMatch(match))));
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
							match => Tuple.Create(match.Product, Enumerable.Empty<IExpression>()))),
					match => new LinkNode(match.Product.Of1.ToArray(), match.Product.Of3.Item1, match.Product.Of3.Item2.ToArray(), MarkdomSourceRange.FromMatch(match))));

			Define(() => ReferenceLabel,
				Sequence(
					Literal("["),
					AtLeast(0, CharacterNotIn(new char[] { ']', '\n', '\r' }), match => match.String),
					Literal("]"),
					match => ReferenceId.FromText(match.Product.Of2)));

			Define(() => Label,
				Sequence(
					Literal("["),
					AtLeast(0, Sequence(NotAhead(Literal("]")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("]"),
					match => match.Product.Of2));

			#endregion

			Define(() => InlineExpression,
				Sequence(
					Literal("@"),
					Reference(() => Expression),
					Optional(Literal(";")),
					match => new InlineExpressionNode(match.Product.Of2, MarkdomSourceRange.FromMatch(match))));

			Define(() => Strong,
				Sequence(
					Literal("**"),
					AtLeast(0, Sequence(NotAhead(Literal("**")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("**")),
					match => new StrongNode(match.Product.Of2.ToArray(), MarkdomSourceRange.FromMatch(match))));

			Define(() => Emphasis,
				Sequence(
					Literal("*"),
					AtLeast(0, Sequence(NotAhead(Literal("*")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("*")),
					match => new EmphasisNode(match.Product.Of2.ToArray(), MarkdomSourceRange.FromMatch(match))));

			var singleQuoted =
				Sequence(
					Literal("'"),
					AtLeast(0, Sequence(NotAhead(Literal("'")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("'"),
					match => new QuotedNode(QuoteType.Single, match.Product.Of2.ToArray(), MarkdomSourceRange.FromMatch(match)));

			var doubleQuoted =
				Sequence(
					Literal("\""),
					AtLeast(0, Sequence(NotAhead(Literal("\"")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("\""),
					match => new QuotedNode(QuoteType.Double, match.Product.Of2.ToArray(), MarkdomSourceRange.FromMatch(match)));

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
						match => match.Product.Of1.InArray().Concat(match.Product.Of2)));

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
							Literal("+", match => 1d),
							Literal("-", match => -1d)),
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
						Literal("."),
						AtLeast(0, Reference(() => Digit))),
					match => double.Parse("0" + match.String),
					noMatch => 0d);

			var requiredDecimalPart =
				Sequence(
					Literal("."),
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
							NotAhead(ChoiceUnordered(Literal("'"), Reference(() => NewLine))),
							Wildcard(),
							match => match.String)),
					match => match.Product.Join());

			var doubleQuotedStringExpressionContent =
				AtLeast(0,
					ChoiceUnordered(
						Literal("\\\"", match => "\""),
						stringExpressionEscapes,
						Sequence(
							NotAhead(ChoiceUnordered(Literal("\""), Reference(() => NewLine))),
							Wildcard(),
							match => match.String)),
					match => match.Product.Join());

			var singleQuotedStringExpression =
				Sequence(
					Literal("'"), singleQuotedStringExpressionContent, Literal("'"),
					match => new StringLiteralExpression(match.Product.Of2, MarkdomSourceRange.FromMatch(match)));

			var doubleQuotedStringExpression =
				Sequence(
					Literal("\""), doubleQuotedStringExpressionContent, Literal("\""),
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
					Literal(":"),
					Reference(() => ExpressionWhitespace),
					Reference(() => Expression),
					match => new PropertyAssignment(match.Product.Of1, match.Product.Of5, MarkdomSourceRange.FromMatch(match)));

			var objectPropertyAssignments =
				Optional(
					Sequence(
						objectPropertyAssignment,
						AtLeast(0, Sequence(argumentSeparator, objectPropertyAssignment, match => match.Product.Of2)),
						match => match.Product.Of1.InArray().Concat(match.Product.Of2)),
					match => match.Product,
					noMatch => Enumerable.Empty<PropertyAssignment>());

			Define(() => ObjectLiteralExpression,
				Sequence(
					Literal("{"),
					Reference(() => ExpressionWhitespace),
					objectPropertyAssignments,
					Reference(() => ExpressionWhitespace),
					Literal("}"),
					match => new ObjectLiteralExpression(match.Product.Of3.ToArray(), MarkdomSourceRange.FromMatch(match))));

			Define(() => ObjectBodyExpression,
				Reference(() => objectPropertyAssignments, match => new ObjectLiteralExpression(match.Product.ToArray(), MarkdomSourceRange.FromMatch(match))));

			#endregion

			#region UriExpression
			//   * Cannot contain `,` (commas), which are ordinarily legal URI characters.
			//     This is to disambiguate URI expressions in argument lists
			//   * Parentheses inside of a URI expression must be balanced
			//   * Cannot start with `@`, `'`, or `"'

			IParsingExpression<object> uriExpressionPart = null;
			IParsingExpression<IEnumerable<Nil>> uriExpressionRegularPart = null;
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
					Literal("("),
					AtLeast(0, Reference(() => uriExpressionPart)),
					Literal(")"));

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
						AtLeast(2, Literal("{"), m => { endBraces = Literal("".PadLeft(m.Length, '}')); return Nil.Value; }),
						AtLeast(0,
							Sequence(NotAhead(Reference(() => endBraces)), Reference(() => Atomic)),
							match => LineInfo.FromMatch(match)),
						Reference(() => endBraces),
						match => new DocumentLiteralExpression(
							ParseLinesAs(Blocks, match.Product.Of2.InArray()).ToArray(),
							MarkdomSourceRange.FromMatch(match)));
				}));

			#endregion

			Define(() => ExpressionWhitespace,
				AtLeast(0,
					ChoiceUnordered(
						Reference(() => Whitespace),
						Reference(() => Comment)),
					match => Nil.Value));

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
							EndOfInput()),
						match => LineInfo.FromMatch(match).InArray(),
						noMatch => Enumerable.Empty<LineInfo>()),
					match => match.Product.Of1.Concat(match.Product.Of2)));

			// NEVER EVER EVER USE THIS IN A REPETITION CONTEXT
			Define(() => BlankLine,
				Sequence(
					Reference(() => SpaceChars),
					ChoiceUnordered(
						Reference(() => NewLine),
						EndOfInput()),
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

			#endregion
		}

		private int _parseLinesCount = 0;
		private T ParseLinesAs<T>(IParsingExpression<T> expression, IEnumerable<LineInfo> lines) {
			lines = lines.ToList();
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
	}
}