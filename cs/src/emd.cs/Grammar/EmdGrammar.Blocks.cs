using emd.cs.Nodes;
using emd.cs.Utils;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public partial class EmdGrammar {
		#region Block Rules

		public static readonly IParsingExpression<IEnumerable<IBlockNode>>
		Blocks =
			Named(() => Blocks,
				AtLeast(0, Reference(() => Block)));

			// Ordering notes:
			//   * Paragraph must come last, because it will sweep up just about anything
		public static readonly IParsingExpression<IBlockNode>
		Block =
			Named(() => Block,
				Sequence(
					Reference(() => InterBlock),
					ChoiceOrdered<IBlockNode>(
						ChoiceUnordered<IBlockNode>(
							Reference(() => Heading),
							Reference(() => ReferenceBlock),
							Reference(() => Blockquote),
							Reference(() => Table),
							Reference(() => UnorderedList),
							Reference(() => OrderedList),
							Reference(() => ExpressionBlock)),
						Reference(() => Paragraph)), 
					Reference(() => InterBlock),
					match => match.Product.Of2));

		public static readonly IParsingExpression<Nil>
		InterBlock =
			Named(() => InterBlock,
				Sequence(
					Reference(() => BlankLines),
					AtLeast(0,
						Sequence(
							AtLeast(1,
								Sequence(
									Reference(() => SpaceChars),
									Reference(() => Comment))),
							Reference(() => BlankLine), // following space-comment combo
							Reference(() => BlankLines)))));

		#region ExpressionBlock

		public static readonly IParsingExpression<ExpressionBlockNode>
		ExpressionBlock =
			Named(() => ExpressionBlock,
				Sequence(
					Reference(() => SpaceChars),
					Ahead(Literal("@")),
					Reference(() => LeftHandSideExpression),
					Optional(
						Sequence(Reference(() => ExpressionWhitespace), Literal(";"))),
					Reference(() => BlankLine), // no trailing content on same line as expression end
					Ahead(Reference(() => BlankLine)), // expression followed by content on next line is block continuation
					match => new ExpressionBlockNode(match.Product.Of3, match.SourceRange)));

		#endregion

		#region Lists
		// We define two types of lists, a *tight* list wherein no items are separated by blank
		// lines which causes each item to be parsed as a single block of inlines, and a
		// *loose* list, where items may contain multiple blocks separated by blank lines.

		// A list item continues if it is followed by any number of blank lines and an indented line.
		// The indented line is not empty because BlankLines would have consumed it if it was.
		private static readonly IParsingExpression<IEnumerable<LineInfo>>
		listItemContinues =
			Sequence(
				Reference(() => BlankLines),
				Ahead(Reference(() => Indent)),
				match => match.Product.Of1);

		// We define a custom BlockLine for lists which discards any indent at the beginning of a line
		private static readonly IParsingExpression<LineInfo>
		listBlockLine =
			Sequence(
				Optional(Reference(() => Indent)),
				Reference(() => NonEmptyBlockLine),
				match => match.Product.Of2);

		#region Ordered List

		private static readonly IEnumerable<char>
		enumeratorCounterStyleLowerRomanCharValues = new char[] { 'i', 'v', 'x', 'l', 'c', 'd', 'm' };

		private static readonly IEnumerable<char>
		enumeratorCounterStyleUpperRomanCharValues = enumeratorCounterStyleLowerRomanCharValues.Select(char.ToUpper);

		private static readonly IParsingExpression<Nil>
		enumeratorCounterStyleLowerRomanChar = CharacterIn(enumeratorCounterStyleLowerRomanCharValues);

		private static readonly IParsingExpression<Nil>
		enumeratorCounterStyleUpperRomanChar = CharacterIn(enumeratorCounterStyleUpperRomanCharValues);

		public static readonly IParsingExpression<int?>
		EnumeratorValue =
			Named(() => EnumeratorValue,
				Optional(
					Sequence(
						Literal("@"),
						AtLeast(1, Reference(() => Digit), match => match.String.ParseDefault(1)),
						match => match.Product.Of2),
					match => new int?(match.Product),
					noMatch => new int?()));

		private static readonly IEnumerable<EnumeratorCounterStyleDefinition>
		enumeratorCounterStyleDefinitions = new EnumeratorCounterStyleDefinition[] {
			new EnumeratorCounterStyleDefinition(
				OrderedListCounterStyle.Decimal,
				AtLeast(1,
					Reference(() => Digit),
					match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.Decimal, match.String.ParseDefault(1)))),
			new EnumeratorCounterStyleDefinition(
				OrderedListCounterStyle.LowerRoman,
				AtLeast(1,
					enumeratorCounterStyleLowerRomanChar,
					match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.LowerRoman, NumeralUtils.ParseRomanNumeral(match.String)))),
			new EnumeratorCounterStyleDefinition(
				OrderedListCounterStyle.UpperRoman,
				AtLeast(1,
					enumeratorCounterStyleUpperRomanChar,
					match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.UpperRoman, NumeralUtils.ParseRomanNumeral(match.String)))),
			new EnumeratorCounterStyleDefinition(
				OrderedListCounterStyle.LowerAlpha,
				AtLeast(1,
					Reference(() => EnglishLowerAlpha),
					match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.LowerAlpha, NumeralUtils.ParseAlphaNumeral(match.String)))),
			new EnumeratorCounterStyleDefinition(
				OrderedListCounterStyle.UpperAlpha,
				AtLeast(1,
					Reference(() => EnglishUpperAlpha),
					match => new EnumeratorCounterStyleInfo(OrderedListCounterStyle.UpperAlpha, NumeralUtils.ParseAlphaNumeral(match.String))))
		};

		private static readonly IEnumerable<EnumeratorSeparatorStyleDefinition>
		enumeratorSeparatorStyleDefinitions = new EnumeratorSeparatorStyleDefinition[] {
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
		private static readonly IEnumerable<EnumeratorStyleDefinition>
		enumeratorStyleDefinitions =
			enumeratorSeparatorStyleDefinitions.Select(sd => {
				var enumeratorPreamble = Sequence(Reference(() => NonIndentSpace), sd.Preceding);
				var enumeratorPostamble = Sequence(sd.Following, AtLeast(1, Reference(() => SpaceChar)));
				return new EnumeratorStyleDefinition {
					SeparatorStyle = sd.SeparatorStyle,
					Preceding = sd.Preceding,
					Following = sd.Following,
					EnumeratorPreamble = enumeratorPreamble,
					EnumeratorPostamble = enumeratorPostamble,
					Counters = // Enumerator rules for the current separator & counter style
						enumeratorCounterStyleDefinitions.Select(cd => new EnumeratorStyleDefinitionCounter {
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
		public static readonly IParsingExpression<Nil>
		Enumerator =
			Named(() => Enumerator,
				Sequence(
					Reference(() => EnumeratorishAhead),
					ChoiceUnordered(
						enumeratorStyleDefinitions.Select(sd =>
							Sequence(
								Ahead(sd.EnumeratorPreamble), 
								ChoiceOrdered(sd.Counters.Select(cd => cd.InitialEnumerator)))
						))));

		/// <summary>
		/// This rule performs a quick lookahead to ensure that the text ahead resembles an enumerator
		/// before trying to parse it as an enumerator.
		/// </summary>
		public static readonly IParsingExpression<Nil>
		EnumeratorishAhead =
			Named(() => EnumeratorishAhead,
				Ahead(
					Regex(@"\ {0,3}(\(|\[)?([0-9]+|[a-z]+|[A-Z]+)(@[0-9]+)?(\.|(\ *-)|\)|\])\s")));

		public static readonly IParsingExpression<LineInfo>
		orderedListBlockLine =
			Sequence(
				NotAhead(Reference(() => Enumerator)), listBlockLine,
				m => m.Product.Of2);

		public static readonly IParsingExpression<IEnumerable<LineInfo>>
		orderedListBlockLines = AtLeast(0, orderedListBlockLine);

		public static readonly IParsingExpression<IEnumerable<LineInfo>>
		orderedListItemSubsequentBlock =
			Sequence(
				listItemContinues,
				orderedListBlockLines,
				match => match.Product.Of1.Concat(match.Product.Of2));

		public static readonly IParsingExpression<OrderedListNode>
		OrderedList =
			Named(() => OrderedList,
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
											match.Product.Of2.InEnumerable().Concat(match.Product.Of3),
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
												.Select(i => new OrderedListItemNode(ParseLinesAs(BlockInlinesOptional, i.Lines).ToArray(), i.SourceRange))
												.ToArray();
											return new OrderedListNode(
												csd.CounterStyle, ssd.SeparatorStyle,
												match.Product.Of1.Enumerator.Value,
												items,
												match.SourceRange);
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
												.Select(i => new OrderedListItemNode(ParseLinesAs(Blocks, i.Lines).ToArray(), i.SourceRange))
												.ToArray();
											return new OrderedListNode(
												csd.CounterStyle, ssd.SeparatorStyle,
												match.Product.Of1.Enumerator.Value,
												items,
												match.SourceRange);
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
		public static readonly IParsingExpression<UnorderedListNode>
		UnorderedList =
			Named(() => UnorderedList,
				ChoiceOrdered(
					Reference(() => UnorderedListTight),
					Reference(() => UnorderedListLoose)));

		public static readonly IParsingExpression<Nil>
		Bullet =
			Named(() => Bullet,
				Sequence(
					Reference(() => NonIndentSpace),
					CharacterIn(new char[] { '*', '-', '+' }),
					AtLeast(1, Reference(() => SpaceChar))));

		public static readonly IParsingExpression<LineInfo>
		unorderedListBlockLine =
			Sequence(
				NotAhead(Reference(() => Bullet)), listBlockLine,
				match => match.Product.Of2);

		public static readonly IParsingExpression<IEnumerable<LineInfo>>
		unorderedListBlockLines = AtLeast(0, unorderedListBlockLine);

		public static readonly IParsingExpression<BlockLineInfo>
		unorderedListItemTight =
			Sequence(
				Reference(() => Bullet), Reference(() => BlockLine),
				unorderedListBlockLines,
				match => new BlockLineInfo(match.Product.Of2.InEnumerable().Concat(match.Product.Of3), match.SourceRange));

		public static readonly IParsingExpression<IEnumerable<LineInfo>>
		unorderedListItemSubsequentBlock =
			Sequence(
				listItemContinues,
				unorderedListBlockLines,
				match => match.Product.Of1.Concat(match.Product.Of2));

		public static readonly IParsingExpression<BlockLineInfo>
		unorderedListItemLoose =
			Sequence(
				unorderedListItemTight,
				AtLeast(0, unorderedListItemSubsequentBlock),
				match => new BlockLineInfo(match.Product.Of1.Lines.Concat(match.Product.Of2.Flatten()), match.SourceRange));

		public static readonly IParsingExpression<Nil>
		unorderedListContinuesLoose =
			ChoiceUnordered(
				Sequence(Reference(() => BlankLines), Reference(() => Bullet)),
				listItemContinues);

		public static readonly IParsingExpression<UnorderedListNode>
		UnorderedListTight =
			Named(() => UnorderedListTight,
				Sequence(
					AtLeast(1, unorderedListItemTight),
					NotAhead(unorderedListContinuesLoose),
					match => {
						var items = match.Product.Of1
							.Select(i => new UnorderedListItemNode(ParseLinesAs(BlockInlinesOptional, i.Lines.ToArray()).ToArray(), i.SourceRange))
							.ToArray();
						return new UnorderedListNode(items, match.SourceRange);
					}));

		public static readonly IParsingExpression<UnorderedListNode>
		UnorderedListLoose =
			Named(() => UnorderedListLoose,
				Sequence(
					unorderedListItemLoose,
					AtLeast(0,
						Sequence(
							Reference(() => BlankLines),
							unorderedListItemLoose,
							match => match.Product.Of2)),
					match => {
						var items = match.Product.Of1.InEnumerable().Concat(match.Product.Of2)
							.Select(i => new UnorderedListItemNode(ParseLinesAs(Blocks, i.Lines).ToArray(), i.SourceRange))
							.ToArray();
						return new UnorderedListNode(items, match.SourceRange);
					}));

		#endregion

		#endregion

		#region Headings

		public static readonly IParsingExpression<HeadingNode>
		Heading =
			Named(() => Heading,
				Sequence(
					Reference(() => HeadingAnnouncement),
					Reference(() => SpaceChars),
					Reference(() => BlockLine),
					match => new HeadingNode(match.Product.Of3.LineString, match.Product.Of1, match.SourceRange)));

		public static readonly IParsingExpression<int>
		HeadingAnnouncement =
			Named(() => HeadingAnnouncement,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Literal("#"), match => match.String.Length),
					match => match.Product.Of2));

		#endregion

		#region Blockquote

		private static readonly IParsingExpression<Nil>
		blockquoteAnnouncement =
			Sequence(Literal(">"), Optional(Literal(" ")));


		private static readonly IParsingExpression<LineInfo>
		blockquoteAnnouncedLine =
			Sequence(
				blockquoteAnnouncement, Reference(() => BlockLine),
				match => match.Product.Of2);

		private static readonly IParsingExpression<IEnumerable<LineInfo>>
		blockquotePart =
			Sequence(
				blockquoteAnnouncedLine,
				AtLeast(0,
					ChoiceOrdered(
						blockquoteAnnouncedLine,
						Reference(() => NonEmptyBlockLine))),
				match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2));

		public static readonly IParsingExpression<BlockquoteNode>
		Blockquote =
			Named(() => Blockquote,
				Sequence(
					blockquotePart,
					AtLeast(0,
						Sequence(
							Reference(() => BlankLines),
							blockquotePart,
							match => match.Product.Of1.Concat(match.Product.Of2))),
					match => new BlockquoteNode(
						ParseLinesAs(Blocks, match.Product.Of1.Concat(match.Product.Of2.Flatten())).ToArray(),
						match.SourceRange)));

		#endregion

		public static readonly IParsingExpression<ReferenceNode>
		ReferenceBlock =
			Named(() => ReferenceBlock,
				Sequence(
					Reference(() => SpaceChars),
					Reference(() => ReferenceLabel),
					Reference(() => SpaceChars),
					Literal(":"),
					Reference(() => SpaceChars),
					ChoiceUnordered(
						Reference(() => UriLiteralExpression, match => match.Product.InEnumerable()),
						Reference(() => ArgumentList)),
					match => new ReferenceNode(match.Product.Of2, match.Product.Of6.ToArray(), match.SourceRange)));

		#region Paragraph

		public static readonly IParsingExpression<ParagraphNode>
		Paragraph =
			Named(() => Paragraph,
				Reference(() => BlockInlinesRequired, match => new ParagraphNode(match.Product.ToArray(), match.SourceRange)));

		#endregion

		public static readonly IParsingExpression<LineInfo>
		NonEmptyBlockLine =
			Named(() => NonEmptyBlockLine,
				Sequence(
					NotAhead(Reference(() => BlankLine)),
					Reference(() => BlockLine),
					match => match.Product.Of2));

		public static readonly IParsingExpression<LineInfo>
		BlockLine =
			Named(() => BlockLine,
				Sequence(
					AtLeast(0, Reference(() => BlockLineAtomic)),
					Optional(Reference(() => NewLine)),
					match => LineInfo.FromMatch(match)));

		public static readonly IParsingExpression<Nil>
		BlockLineAtomic =
			Named(() => BlockLineAtomic,
				Sequence(
					NotAhead(Reference(() => NewLine)),
					Reference(() => Atomic)));


		public static readonly IParsingExpression<Nil>
		Atomic =
			Named(() => Atomic,
				ChoiceOrdered(
					Sequence(NotAhead(CharacterIn(specialCharValues)), Reference(() => UnicodeCharacter)),
					ChoiceUnordered(
						Reference(() => InlineExpression),
						Reference(() => Comment),
						Reference(() => Link),
						Reference(() => AutoLink),
						Reference(() => Code)),
					Reference(() => UnicodeCharacter)));

		#region Tables

		public static readonly IParsingExpression<TableNode>
		Table =
			Named(() => Table,
				AtLeast(1,
					Reference(() => TableRow),
					match => new TableNode(match.Product.ToArray(), match.SourceRange)));

		private static readonly IParsingExpression<LineInfo>
		tableCellContents =
			AtLeast(0,
				Sequence(
					NotAhead(Literal("|")),
					Reference(() => BlockLineAtomic)),
				match => LineInfo.FromMatch(match));

		private static readonly IParsingExpression<string>
		tableCellRowSpan =
			Sequence(
				AtLeast(1, Reference(() => Digit), match => match.String),
				ChoiceUnordered(Literal("r"), Literal("R")),
				match => match.Product.Of1);

		private static readonly IParsingExpression<string>
		tableCellColumnSpan =
			Sequence(
				AtLeast(1, Reference(() => Digit), match => match.String),
				ChoiceUnordered(Literal("c"), Literal("C")),
				match => match.Product.Of1);

		private static readonly IParsingExpression<TableCellSpanningInfo>
		tableCellAnnouncement =
			Sequence(
				Literal("|"),
				Optional(tableCellColumnSpan),
				Optional(tableCellRowSpan),
				match => new TableCellSpanningInfo(match.Product.Of2.ParseDefault(1), match.Product.Of3.ParseDefault(1)));

		private static readonly IParsingExpression<TableCellSpanningInfo>
		tableHeaderCellAnnouncement =
			Sequence(
				tableCellAnnouncement,
				Literal("="),
				Reference(() => SpaceChars),
				match => match.Product.Of1);

		private static readonly IParsingExpression<TableCellSpanningInfo>
		tableDataCellAnnouncement =
			Sequence(
				tableCellAnnouncement,
				Reference(() => SpaceChars),
				match => match.Product.Of1);

		private static readonly IParsingExpression<TableCellNode>
		tableHeaderCell =
			Sequence(
				tableHeaderCellAnnouncement,
				tableCellContents,
				match =>
					new TableCellNode(
						TableCellKind.Header,
						match.Product.Of1.ColumnSpan,
						match.Product.Of1.RowSpan,
						ParseLinesAs(BlockInlinesOptional, match.Product.Of2.InEnumerable()).ToArray(),
						match.SourceRange));

		private static readonly IParsingExpression<TableCellNode>
		tableDataCell =
			Sequence(
				tableDataCellAnnouncement,
				tableCellContents,
				match => new TableCellNode(
					TableCellKind.Data,
					match.Product.Of1.ColumnSpan,
					match.Product.Of1.RowSpan,
					ParseLinesAs(BlockInlinesOptional, match.Product.Of2.InEnumerable()).ToArray(),
					match.SourceRange));

		private static readonly IParsingExpression<Nil>
		tableRowEnd =
			Sequence(
				Optional(Literal("|")),
				Reference(() => BlankLine));

		private static readonly IParsingExpression<TableCellNode>
		tableCell =
			Sequence(
				NotAhead(tableRowEnd),
				ChoiceOrdered<TableCellNode>(
					tableHeaderCell,
					tableDataCell),
				match => match.Product.Of2);

		public static readonly IParsingExpression<TableRowNode>
		TableRow =
			Named(() => TableRow,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Reference(() => tableCell)),
					tableRowEnd,
					match => new TableRowNode(match.Product.Of2.ToArray(), match.SourceRange)));

		#endregion

		#endregion
	}
}
