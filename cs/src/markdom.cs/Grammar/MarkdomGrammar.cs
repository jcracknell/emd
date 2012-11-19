using markdom.cs.Expressions;
using markdom.cs.Nodes;
using markdom.cs.Utils;
using pegleg.cs;
using pegleg.cs.Parsing;
using pegleg.cs.Unicode;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace markdom.cs.Grammar {
	public class MarkdomGrammar : ParsingExpression {
		private static readonly MarkdomGrammar _instance = new MarkdomGrammar();

		public static MarkdomGrammar Instance { get { return _instance; } }

		private static int _parseLinesCount = 0;
		protected static T ParseLinesAs<T>(IParsingExpression<T> expression, IEnumerable<LineInfo> lines) {
			lines = lines.ToList();
			_parseLinesCount++;
			var expressionMatchingContext =
				new MatchingContext(
					lines.Select(line => line.LineString).JoinStrings(),
					lines.Select(line => line.SourceRange).ToArray());
			
			var expressionMatchingResult = expression.Matches(expressionMatchingContext);

			if(!expressionMatchingResult.Succeeded)
				return default(T);

			return expressionMatchingResult.Product;
		}

		#region char sets
		private static readonly IEnumerable<char> spaceCharValues = new char[] { ' ', '\t' };
		private static readonly IEnumerable<char> whitespaceCharValues = spaceCharValues.Concat(new char[] { '\n', '\r' });
		private static readonly IEnumerable<char> englishLowerAlphaCharValues = CharUtils.Range('a','z');
		private static readonly IEnumerable<char> englishUpperAlphaCharValues = CharUtils.Range('A','Z');
		private static readonly IEnumerable<char> englishAlphaCharValues = englishLowerAlphaCharValues.Concat(englishUpperAlphaCharValues);
		private static readonly IEnumerable<char> digitCharValues = CharUtils.Range('0','9');
		private static readonly IEnumerable<char> hexadecimalCharValues = digitCharValues.Concat(CharUtils.Range('A','F')).Concat(CharUtils.Range('a','f'));
		private static readonly IEnumerable<char> specialCharValues = new char[] {
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
		#endregion

		public static readonly IParsingExpression<MarkdomDocumentNode>
		Document =
			Named(() => Document,
				Reference(() => Blocks, match => new MarkdomDocumentNode(match.Product.ToArray(), match.SourceRange)));

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
					Reference(() => Expression),
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

		private static readonly IParsingExpression<Nil>
		enumeratorCounterStyleRomanChar = CharacterIn(enumeratorCounterStyleLowerRomanCharValues.Concat(enumeratorCounterStyleUpperRomanCharValues));

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
				var enumeratorPostamble = Sequence(sd.Following, Reference(() => SpaceChars));
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
					Regex(@"\ {0,3}(\(|\[)?([0-9]+|[a-zA-Z]+)(@[0-9]+)?(\.|(\ *-)|\)|\])")));

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
												.Select(i => new OrderedListItemNode(ParseLinesAs(Inlines, i.Lines).ToArray(), i.SourceRange))
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
							.Select(i => new UnorderedListItemNode(ParseLinesAs(Inlines, i.Lines.ToArray()).ToArray(), i.SourceRange))
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
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Reference(() => Inline)),
					Reference(() => BlankLine),
					match => new ParagraphNode(match.Product.Of2.ToArray(), match.SourceRange)));

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

		private static readonly IParsingExpression<TableHeaderCellNode>
		tableHeaderCell =
			Sequence(
				tableHeaderCellAnnouncement,
				tableCellContents,
				match =>
					new TableHeaderCellNode(
						match.Product.Of1.ColumnSpan,
						match.Product.Of1.RowSpan,
						ParseLinesAs(Inlines, match.Product.Of2.InEnumerable()).ToArray(),
						match.SourceRange));

		private static readonly IParsingExpression<TableDataCellNode>
		tableDataCell =
			Sequence(
				tableDataCellAnnouncement,
				tableCellContents,
				match => new TableDataCellNode(
					match.Product.Of1.ColumnSpan,
					match.Product.Of1.RowSpan,
					ParseLinesAs(Inlines, match.Product.Of2.InEnumerable()).ToArray(),
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

		#region Inline Rules

		public static readonly IParsingExpression<IEnumerable<IInlineNode>>
		Inlines =
			Named(() => Inlines,
				AtLeast(0, Reference(() => Inline)));

		public static readonly IParsingExpression<IInlineNode>
		Inline =
			Named(() => Inline,
				ChoiceOrdered<IInlineNode>(
					ChoiceUnordered<IInlineNode>(
						Reference(() => Text),
						ChoiceOrdered<IInlineNode>(
							Reference(() => LineBreak),
							Reference(() => Space)),
						ChoiceOrdered<IInlineNode>(
							Reference(() => Strong),
							Reference(() => Emphasis)),
						Reference(() => Quoted),
						Reference(() => Link),
						Reference(() => AutoLink),
						Reference(() => Entity),
						Reference(() => Code),
						Reference(() => InlineExpression)),
					Reference(() => Symbol)));

		#region AutoLink

		public static readonly IParsingExpression<AutoLinkNode>
		AutoLink =
			Named(() => AutoLink,
				Sequence(
					Literal("<"),
					Reference(() => SpaceChars),
					Reference(() => UriLiteral),
					Reference(() => SpaceChars),
					Literal(">"),
					Optional(
						Reference(() => ArgumentList),
						match => match.Product,
						noMatch => new IExpression[0]),
					match => new AutoLinkNode(match.Product.Of3, match.Product.Of6.ToArray(), match.SourceRange)));

		#endregion

		#region Link

		public static readonly IParsingExpression<LinkNode>
		Link =
			Named(() => Link,
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
					match => new LinkNode(match.Product.Of1.ToArray(), match.Product.Of3.Item1, match.Product.Of3.Item2.ToArray(), match.SourceRange)));

		public static readonly IParsingExpression<ReferenceId>
		ReferenceLabel =
			Named(() => ReferenceLabel,
				Sequence(
					Literal("["),
					AtLeast(0, 
						Sequence(
							NotAhead(CharacterIn(']', '\n', '\r')),
							Reference(() => UnicodeCharacter)),
						match => match.String),
					Literal("]"),
					match => ReferenceId.FromText(match.Product.Of2)));

		public static readonly IParsingExpression<IEnumerable<IInlineNode>>
		Label =
			Named(() => Label,
				Sequence(
					Literal("["),
					AtLeast(0, Sequence(NotAhead(Literal("]")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("]"),
					match => match.Product.Of2));

		#endregion

		public static readonly IParsingExpression<InlineExpressionNode>
		InlineExpression =
			Named(() => InlineExpression,
				Sequence(
					Ahead(Literal("@")),
					Reference(() => Expression),
					Optional(Literal(";")),
					match => new InlineExpressionNode(match.Product.Of2, match.SourceRange)));

		public static readonly IParsingExpression<StrongNode>
		Strong =
			Named(() => Strong,
				Sequence(
					Literal("**"),
					AtLeast(0, Sequence(NotAhead(Literal("**")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("**")),
					match => new StrongNode(match.Product.Of2.ToArray(), match.SourceRange)));

		public static readonly IParsingExpression<EmphasisNode>
		Emphasis =
			Named(() => Emphasis,
				Sequence(
					Literal("*"),
					AtLeast(0, Sequence(NotAhead(Literal("*")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("*")),
					match => new EmphasisNode(match.Product.Of2.ToArray(), match.SourceRange)));

		#region Quoted

		private static readonly IParsingExpression<QuotedNode>
		singleQuoted =
			Sequence(
				Literal("'"),
				AtLeast(0, Sequence(NotAhead(Literal("'")), Reference(() => Inline), match => match.Product.Of2)),
				Literal("'"),
				match => new QuotedNode(QuoteType.Single, match.Product.Of2.ToArray(), match.SourceRange));

		private static readonly IParsingExpression<QuotedNode>
		doubleQuoted =
			Sequence(
				Literal("\""),
				AtLeast(0, Sequence(NotAhead(Literal("\"")), Reference(() => Inline), match => match.Product.Of2)),
				Literal("\""),
				match => new QuotedNode(QuoteType.Double, match.Product.Of2.ToArray(), match.SourceRange));

		public static readonly IParsingExpression<QuotedNode>
		Quoted =
			Named(() => Quoted,
				ChoiceOrdered(doubleQuoted, singleQuoted));

		#endregion

		public static readonly IParsingExpression<LineBreakNode>
		LineBreak =
			Named(() => LineBreak,
				Sequence(
					Optional(Reference(() => InlineSpace)),
					Literal("\\"),
					Ahead(Reference(() => BlankLine)),
					Optional(Reference(() => InlineSpace)),
					match => new LineBreakNode(match.SourceRange)));


		#region Code

		public static readonly IParsingExpression<CodeNode>
		Code =
			Named(() => Code,
				Sequence(
					Ahead(Literal("`")),
					ChoiceOrdered(
						Enumerable.Range(1,8).Reverse()
						.Select(i => Literal("".PadRight(i, '`')))
						.Select(ticks =>
							Sequence(
								ticks,
								Reference(() => OptionalBlockWhitespace),
								AtLeast(0,
									Sequence(
										Reference(() => OptionalBlockWhitespace),
										AtLeast(1,
											Sequence(
												NotAhead(ChoiceUnordered(ticks, Reference(() => Whitespace))),
												Reference(() => UnicodeCharacter)))),
									match => match.String),
								Reference(() => OptionalBlockWhitespace),
								ticks,
								match => new CodeNode(match.Product.Of3, match.SourceRange)))),
					match => match.Product.Of2));

			#endregion

		#region Entities

		private static readonly IParsingExpression<EntityNode>
		decimalHtmlEntity =
			Sequence(
				Literal("&#"),
				Between(1, 6, Reference(() => Digit), match => match.String),
				Literal(";"),
				match => new EntityNode(int.Parse(match.Product.Of2), match.SourceRange));

		private static readonly IParsingExpression<EntityNode>
		hexHtmlEntity =
			Sequence(
				Literal("&#x"),
				Between(1, 6, Reference(() => HexDigit), match => match.String),
				Literal(";"),
				match => new EntityNode(Convert.ToInt32(match.Product.Of2, 16), match.SourceRange));

			// Because of the large number of named entities it is much faster to use a dynamic
			// expression with an assertion to match valid entity names
		private static readonly IParsingExpression<EntityNode>
		namedHtmlEntity =
			Dynamic(() => {
				string entityName = null;
				return Sequence(
					Literal("&"),
					Between(1, 32, Reference(() => EnglishAlpha), match => { return entityName = match.String; }),
					Assert(() => EntityNode.IsEntityName(entityName)),
					Literal(";"),
					match => new EntityNode(EntityNode.GetNamedEntityValue(entityName), match.SourceRange));
			});

		public static readonly IParsingExpression<EntityNode>
		Entity =
			Named(() => Entity,
				ChoiceOrdered(
					decimalHtmlEntity,
					hexHtmlEntity,
					namedHtmlEntity));

		#endregion 

		public static readonly IParsingExpression<TextNode>
		Text =
			Named(() => Text,
				AtLeast(1,
					Reference(() => NormalChar),
					match => new TextNode(match.String, match.SourceRange)));

		public static readonly IParsingExpression<SpaceNode>
		Space =
			Named(() => Space,
				Reference(() => InlineSpace, match => new SpaceNode(match.SourceRange)));

		public static readonly IParsingExpression<Nil>
		InlineSpace =
			Named(() => InlineSpace,
				ChoiceOrdered(
					// At least one comment interleaved with whitespace in any combination
					Sequence(
						Reference(() => OptionalBlockWhitespace),
						AtLeast(1,
							Sequence(
								Reference(() => Comment),
								Reference(() => OptionalBlockWhitespace))),
						NotAhead(Reference(() => BlankLine))),
					// Or just one chunk of whitespace
					Reference(() => BlockWhitespace)));

		public static readonly IParsingExpression<Nil>
		NormalChar =
			Named(() => NormalChar,
				UnicodeParsingExpressions.UnicodeCharacterNotIn(whitespaceCharValues, specialCharValues));

		public static readonly IParsingExpression<SymbolNode>
		Symbol =
			Named(() => Symbol,
				Reference(() => SpecialChar, match => new SymbolNode(match.String, match.SourceRange)));

		public static readonly IParsingExpression<Nil>
		SpecialChar =
			Named(() => SpecialChar,
				CharacterIn(specialCharValues));

		#endregion

		#region Expressions

		public static readonly IParsingExpression<IExpression>
		Expression =
			Named(() => Expression,
				Reference(() => AdditiveExpression));

		#region AdditiveExpression

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		additionExpressionPart =
			Sequence(
				Literal("+"),
				Reference(() => ExpressionWhitespace),
				Reference(() => MultiplicativeExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new AdditionExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		subtractionExpressionPart =
			Sequence(
				Literal("-"),
				Reference(() => ExpressionWhitespace),
				Reference(() => MultiplicativeExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new SubtractionExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		public static readonly IParsingExpression<IExpression>
		AdditiveExpression =
			Named(() => AdditiveExpression,
				Sequence(
					Reference(() => MultiplicativeExpression),
					AtLeast(0,
						Sequence(
							Reference(() => ExpressionWhitespace),
							ChoiceUnordered(
								additionExpressionPart,
								subtractionExpressionPart),
							match => match.Product.Of2)),
					match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left))));

		#endregion

		#region MultiplicativeExpression

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		multiplicationExpressionPart =
			Sequence(
				Literal("*"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new MultiplicationExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		divisionExpressionPart =
			Sequence(
				Literal("/"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new DivisionExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		moduloExpressionPart =
			Sequence(
				Literal("%"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new ModuloExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		public static readonly IParsingExpression<IExpression>
		MultiplicativeExpression =
			Named(() => MultiplicativeExpression,
				Sequence(
					Reference(() => UnaryExpression),
					AtLeast(0,
						Sequence(
							Reference(() => ExpressionWhitespace),
							ChoiceUnordered(
								multiplicationExpressionPart,
								divisionExpressionPart,
								moduloExpressionPart),
							match => match.Product.Of2)),
					match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left))));

		#endregion

		#region UnaryExpression

		private static readonly IParsingExpression<DeleteExpression>
		deleteExpression = 
			Sequence(
				Literal("delete"),
				NotAhead(Reference(() => IdentifierPart)),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new DeleteExpression(match.Product.Of4, match.SourceRange));

		private static readonly IParsingExpression<VoidExpression>
		voidExpression =
			Sequence(
				Literal("void"),
				NotAhead(Reference(() => IdentifierPart)),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new VoidExpression(match.Product.Of4, match.SourceRange));

		private static readonly IParsingExpression<TypeofExpression>
		typeofExpression =
			Sequence(
				Literal("typeof"),
				NotAhead(Reference(() => IdentifierPart)),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new TypeofExpression(match.Product.Of4, match.SourceRange));

		private static readonly IParsingExpression<PrefixDecrementExpression>
		prefixDecrementExpression =
			Sequence(
				Literal("--"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new PrefixDecrementExpression(match.Product.Of3, match.SourceRange));

		private static readonly IParsingExpression<PrefixIncrementExpression>
		prefixIncrementExpression =
			Sequence(
				Literal("++"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new PrefixIncrementExpression(match.Product.Of3, match.SourceRange));

		private static readonly IParsingExpression<NegativeExpression>
		negativeExpression =
			Sequence(
				Literal("-"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new NegativeExpression(match.Product.Of3, match.SourceRange));

		private static readonly IParsingExpression<PositiveExpression>
		positiveExpression =
			Sequence(
				Literal("+"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new PositiveExpression(match.Product.Of3, match.SourceRange));

		private static readonly IParsingExpression<BitwiseNotExpression>
		bitwiseNotExpression =
			Sequence(
				Literal("~"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new BitwiseNotExpression(match.Product.Of3, match.SourceRange));

		private static readonly IParsingExpression<LogicalNotExpression>
		logicalNotExpression =
			Sequence(
				Literal("!"),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new LogicalNotExpression(match.Product.Of3, match.SourceRange));

		public static readonly IParsingExpression<IExpression>
		UnaryExpression =
			Named(() => UnaryExpression,
				ChoiceOrdered<IExpression>(
					ChoiceUnordered<IExpression>(
						logicalNotExpression,
						ChoiceOrdered<IExpression>(
							prefixDecrementExpression,
							negativeExpression),
						ChoiceOrdered<IExpression>(
							prefixIncrementExpression,
							positiveExpression),
						typeofExpression,
						deleteExpression,
						voidExpression,
						bitwiseNotExpression),
					Reference(() => PostfixExpression)));

		#endregion

		#region PostfixExpression

		public static readonly IParsingExpression<IExpression>
		PostfixExpression =
			Named(() => PostfixExpression,
				Sequence(
					Reference(() => LeftHandSideExpression),
					Optional(
						Sequence(
							Reference(() => ExpressionWhitespaceNoNewline),
							ChoiceUnordered(
								Literal("--", match => {
									Func<IExpression,IExpression> asm = body => 
										new PostfixDecrementExpression(body, body.SourceRange.Through(match.SourceRange));
									return asm;
								}),
								Literal("++", match => {
									Func<IExpression,IExpression> asm = body => 
										new PostfixIncrementExpression(body, body.SourceRange.Through(match.SourceRange));
									return asm;
								})),
							match => match.Product.Of2),
						match => match.Product,
						noMatch => e => e),
					match => match.Product.Of2(match.Product.Of1)));

		#endregion

		#region LeftHandSideExpression

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		dynamicPropertyExpressionPart =
			Sequence(
				Literal("["), Reference(() => ExpressionWhitespace),
				Reference(() => Expression),
				Reference(() => ExpressionWhitespace), Literal("]"),
				match => {
					Func<IExpression, IExpression> asm = body =>
						new DynamicPropertyExpression(body, match.Product.Of3, body.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		staticPropertyExpressionPart =
			Sequence(
				Literal("."), Reference(() => ExpressionWhitespace),
				Reference(() => Identifier),
				match => {
					Func<IExpression, IExpression> asm = body =>
						new StaticPropertyExpression(body, match.Product.Of3, body.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		callExpressionPart =
			Reference(
				() => ArgumentList,
				match => {
					Func<IExpression, IExpression> asm = body =>
						new CallExpression(body, match.Product.ToArray(), body.SourceRange.Through(match.SourceRange));
					return asm;
				});

		public static readonly IParsingExpression<IExpression>
		LeftHandSideExpression =
			Named(() => LeftHandSideExpression,
				Sequence(
					Reference(() => AtExpression),
					AtLeast(0,
						Sequence(
							Reference(() => ExpressionWhitespace),
							ChoiceOrdered(
								callExpressionPart,
								staticPropertyExpressionPart,
								dynamicPropertyExpressionPart),
							match => match.Product.Of2)),
					match => match.Product.Of2.Reduce(match.Product.Of1, (body, asm) => asm(body))));

		#endregion

		#region ArgumentList

		private static readonly IParsingExpression<Nil>
		argumentSeparator =
			Sequence(
				Reference(() => ExpressionWhitespace),
				Literal(","),
				Reference(() => ExpressionWhitespace));

		private static readonly IParsingExpression<IEnumerable<IExpression>>
		argumentListArguments =
			Optional(
				Sequence(
					Reference(() => Expression),
					AtLeast(0, Sequence( argumentSeparator, Reference(() => Expression), match => match.Product.Of2)),
					match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2)));

		public static readonly IParsingExpression<IEnumerable<IExpression>>
		ArgumentList =
			Named(() => ArgumentList,
				Sequence(
					Sequence(Literal("("), Reference(() => ExpressionWhitespace)),
					argumentListArguments,
					Sequence(Reference(() => ExpressionWhitespace), Literal(")")),
					match => match.Product.Of2 ?? new IExpression[0]));

		#endregion

		#region AtExpression

		public static readonly IParsingExpression<IExpression>
		AtExpression =
			Named(() => AtExpression,
				ChoiceOrdered(
					Reference(() => AtExpressionRequired),
					Reference(() => PrimaryExpression)));

		public static readonly IParsingExpression<IExpression>
		AtExpressionRequired =
			Named(() => AtExpressionRequired,
				Sequence(
					Literal("@"),
					ChoiceOrdered(
						Reference(() => IdentifierExpression),
						Reference(() => PrimaryExpression)),
					match => match.Product.Of2));

		#endregion

		#region IdentifierExpression

		private static readonly IParsingExpression<Nil>
		identifierExpressionStart =
			ChoiceOrdered(
				UnicodeParsingExpressions.UnicodeCharacterIn(
					UnicodeCategories.Lu,
					UnicodeCategories.Ll,
					UnicodeCategories.Lt,
					UnicodeCategories.Lm,
					UnicodeCategories.Nl),
				Literal("$"),
				Literal("_"),
				Sequence(Literal("\\"), Reference(() => ExpressionUnicodeEscapeSequence)));

		public static readonly IParsingExpression<Nil>
		IdentifierPart =
			Named(() => IdentifierPart,
				ChoiceOrdered(
					identifierExpressionStart,
					UnicodeParsingExpressions.UnicodeCharacterIn(
						UnicodeCategories.Mn,
						UnicodeCategories.Mc,
						UnicodeCategories.Nd,
						UnicodeCategories.Pc),
					CharacterIn(/*ZWNJ*/'\u200C', /*ZWJ*/'\u200D')));

		public static readonly IParsingExpression<IdentifierExpression>
		IdentifierExpression =
			Named(() => IdentifierExpression,
				Reference(() => Identifier,
					match => new IdentifierExpression(match.Product, match.SourceRange)));

		public static readonly IParsingExpression<string>
		Identifier =
			Named(() => Identifier,
				Sequence(
					NotAhead(Reference(() => ExpressionKeyword)),
					identifierExpressionStart,
					AtLeast(0, Reference(() => IdentifierPart)),
					match => match.String));

		#endregion

		public static readonly IParsingExpression<IExpression>
		PrimaryExpression =
			Named(() => PrimaryExpression,
				ChoiceUnordered(
					Reference(() => LiteralExpression),
					Reference(() => ArrayLiteralExpression),
					Reference(() => ObjectLiteralExpression),
					Reference(() => DocumentLiteralExpression),
					Sequence(
						Literal("("), Reference(() => ExpressionWhitespace), Reference(() => Expression), Reference(() => ExpressionWhitespace), Literal(")"),
						match => match.Product.Of3)));

		#region ArrayLiteralExpression

		public static readonly IParsingExpression<Nil>
		arrayElementSeparator =
			Sequence(
				Reference(() => ExpressionWhitespace),
				Literal(","),
				Reference(() => ExpressionWhitespace));

		public static readonly IParsingExpression<IEnumerable<IExpression>>
		elidedElements =
			AtLeast(0,
				arrayElementSeparator,
				match => match.Product.Select(elided => (IExpression)null));

			// A non-elided array element preceded by any number of elided elements
		public static readonly IParsingExpression<IEnumerable<IExpression>>
		subsequentArrayElement =
			Sequence(
				arrayElementSeparator,
				elidedElements,
				Reference(() => Expression),
				match => match.Product.Of2.Concat(match.Product.Of3.InEnumerable()));

		public static readonly IParsingExpression<IEnumerable<IExpression>>
		arrayElements =
			Sequence(
				ChoiceOrdered(
					// initial element non-elided
					Sequence(
						Reference(() => Expression),
						AtLeast(0, subsequentArrayElement),
						match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2.Flatten())),
					// initial element elided
					AtLeast(1,
						subsequentArrayElement,
						match => ((IExpression)null).InEnumerable().Concat(match.Product.Flatten())),
					// all elements elided
					Reference(() => elidedElements, match => Enumerable.Empty<IExpression>())),
				elidedElements, // trailing elided elements discarded
				match => match.Product.Of1);

		public static readonly IParsingExpression<ArrayLiteralExpression>
		ArrayLiteralExpression =
			Named(() => ArrayLiteralExpression,
				Sequence(
					Literal("["), Reference(() => ExpressionWhitespace),
					arrayElements,
					Reference(() => ExpressionWhitespace), Literal("]"),
					match => new ArrayLiteralExpression(match.Product.Of3.ToArray(), match.SourceRange)));

		#endregion

		#region ObjectExpression

		private static readonly IParsingExpression<PropertyAssignment>
		objectPropertyAssignment =
			Sequence(
				ChoiceUnordered(
					Reference(() => Identifier),
					Reference(() => StringLiteral),
					Reference(() => NumericLiteral, match => match.Product.ToString())),
				Reference(() => ExpressionWhitespace),
				Literal(":"),
				Reference(() => ExpressionWhitespace),
				Reference(() => Expression),
				match => new PropertyAssignment(match.Product.Of1, match.Product.Of5, match.SourceRange));

		private static readonly IParsingExpression<IEnumerable<PropertyAssignment>>
		objectPropertyAssignments =
			Optional(
				Sequence(
					objectPropertyAssignment,
					AtLeast(0, Sequence(argumentSeparator, objectPropertyAssignment, match => match.Product.Of2)),
					match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2)),
				match => match.Product,
				noMatch => Enumerable.Empty<PropertyAssignment>());

		public static readonly IParsingExpression<ObjectLiteralExpression>
		ObjectLiteralExpression =
			Named(() => ObjectLiteralExpression,
				Sequence(
					Literal("{"),
					Reference(() => ExpressionWhitespace),
					objectPropertyAssignments,
					Reference(() => ExpressionWhitespace),
					Literal("}"),
					match => new ObjectLiteralExpression(match.Product.Of3.ToArray(), match.SourceRange)));

		public static readonly IParsingExpression<ObjectLiteralExpression>
		ObjectBodyExpression =
			Named(() => ObjectBodyExpression,
				Reference(() => objectPropertyAssignments, match => new ObjectLiteralExpression(match.Product.ToArray(), match.SourceRange)));

		#endregion

		#region DocumentLiteralExpression

		public static readonly IParsingExpression<DocumentLiteralExpression>
		DocumentLiteralExpression =
			Named(() => DocumentLiteralExpression,
				Dynamic(() => {
					IParsingExpression<Nil> endBraces = null;
					return Sequence(
						AtLeast(2, Literal("{"), m => { endBraces = Literal("".PadLeft(m.Length, '}')); return Nil.Value; }),
						AtLeast(0,
							Sequence(NotAhead(Reference(() => endBraces)), Reference(() => Atomic)),
							match => LineInfo.FromMatch(match)),
						Reference(() => endBraces),
						match => new DocumentLiteralExpression(
							ParseLinesAs(Blocks, match.Product.Of2.InEnumerable()).ToArray(),
							match.SourceRange));
				}));

		#endregion

		public static readonly IParsingExpression<IExpression>
		LiteralExpression =
			Named(() => LiteralExpression,
				ChoiceOrdered<IExpression>(
					Reference(() => NullLiteralExpression),
					Reference(() => BooleanLiteralExpression),
					Reference(() => NumericLiteralExpression),
					Reference(() => StringLiteralExpression),
					Reference(() => UriLiteralExpression)));

		#region NullLiteralExpression

		public static readonly IParsingExpression<NullLiteralExpression>
		NullLiteralExpression =
			Named(() => NullLiteralExpression,
				Reference(() => NullLiteral, match => new NullLiteralExpression(match.SourceRange)));

		public static readonly IParsingExpression<Nil>
		NullLiteral =
			Named(() => NullLiteral, Literal("null"));

		#endregion

		#region BooleanLiteralExpression

		public static readonly IParsingExpression<BooleanLiteralExpression>
		BooleanLiteralExpression =
			Named(() => BooleanLiteralExpression,
				Reference(() => BooleanLiteral, match => new BooleanLiteralExpression(match.Product, match.SourceRange)));

		public static readonly IParsingExpression<bool>
		BooleanLiteral =
			Named(() => BooleanLiteral,
				ChoiceUnordered(
					Literal("true", match => true),
					Literal("false", match => false)));

		#endregion

		#region NumericLiteralExpression

		private static readonly IParsingExpression<double>
		decimalIntegerLiteral =
			ChoiceUnordered(
				Literal("0"),
				Sequence(
					Reference(() => NonZeroDigit),
					AtLeast(0, Reference(() => Digit))),
				match => double.Parse(match.String));

		private static readonly IParsingExpression<double>
		signedInteger =
			Sequence(
				Optional(
					ChoiceUnordered(
						Literal("+", match => 1d),
						Literal("-", match => -1d)),
					match => match.Product,
					noMatch => 1d),
				AtLeast(1, Reference(() => Digit), match => double.Parse(match.String)),
				match => match.Product.Of1 * match.Product.Of2);

		private static readonly IParsingExpression<double>
		hexIntegerLiteral =
			Sequence(
				Literal("0x"),
				AtLeast(1, Reference(() => HexDigit), match => match.String),
				match => (double)Convert.ToInt64(match.String, 16));

		private static readonly IParsingExpression<double>
		optionalExponentPart =
			Optional(
				Sequence(
					ChoiceUnordered(Literal("e"), Literal("E")),
					signedInteger,
					match => match.Product.Of2),
				match => Math.Pow(10d, match.Product),
				noMatch => 1d);

		private static readonly IParsingExpression<double>
		optionalDecimalPart =
			Optional(
				Sequence(
					Literal("."),
					AtLeast(0, Reference(() => Digit))),
				match => double.Parse("0" + match.String),
				noMatch => 0d);

		private static readonly IParsingExpression<double>
		requiredDecimalPart =
			Sequence(
				Literal("."),
				AtLeast(1, Reference(() => Digit)),
			match => double.Parse("0" + match.String));

		private static readonly IParsingExpression<double>
		decimalLiteral = 
			ChoiceUnordered(
				Sequence(
					decimalIntegerLiteral,
					optionalDecimalPart,
					optionalExponentPart,
					m => (m.Product.Of1 + m.Product.Of2) * m.Product.Of3),
				Sequence(
					requiredDecimalPart,
					optionalExponentPart,
					m => m.Product.Of1 * m.Product.Of2));

		public static readonly IParsingExpression<NumericLiteralExpression>
		NumericLiteralExpression =
			Named(() => NumericLiteralExpression,
				Reference(() => NumericLiteral, match => new NumericLiteralExpression(match.Product, match.SourceRange)));

		public static readonly IParsingExpression<double>
		NumericLiteral =
			Named(() => NumericLiteral,
				ChoiceOrdered(hexIntegerLiteral, decimalLiteral));

		#endregion

		#region StringLiteralExpression
			
		private static readonly IParsingExpression<string>
		stringExpressionEscapes =
			ChoiceUnordered(
				Literal(@"\n", match => "\n"),
				Literal(@"\r", match => "\r"),
				Literal(@"\t", match => "\t"),
				Literal(@"\\", match => "\\"));

		private static readonly IParsingExpression<string>
		singleQuotedStringExpressionContent =
			AtLeast(0,
				ChoiceUnordered(
					Literal(@"\'", match => "'"),
					stringExpressionEscapes,
					Sequence(
						NotAhead(ChoiceUnordered(Literal("'"), Reference(() => NewLine))),
						Reference(() => UnicodeCharacter),
						match => match.String)),
				match => match.Product.JoinStrings());

		private static readonly IParsingExpression<string>
		doubleQuotedStringExpressionContent =
			AtLeast(0,
				ChoiceUnordered(
					Literal("\\\"", match => "\""),
					stringExpressionEscapes,
					Sequence(
						NotAhead(ChoiceUnordered(Literal("\""), Reference(() => NewLine))),
						Reference(() => UnicodeCharacter),
						match => match.String)),
				match => match.Product.JoinStrings());

		private static readonly IParsingExpression<string>
		singleQuotedStringExpression =
			Sequence(
				Literal("'"), singleQuotedStringExpressionContent, Literal("'"),
				match => match.Product.Of2);

		private static readonly IParsingExpression<string>
		doubleQuotedStringExpression =
			Sequence(
				Literal("\""), doubleQuotedStringExpressionContent, Literal("\""),
				match => match.Product.Of2);

		private static readonly IParsingExpression<string>
		verbatimStringExpression =
			ChoiceOrdered(
				Enumerable.Range(1,16).Reverse()
				.Select(i => "".PadRight(i, '`'))
				.Select(ticks =>
					Sequence(
						Literal(ticks),
						AtLeast(0,
							Sequence(NotAhead(Literal(ticks)), Reference(() => UnicodeCharacter)),
							match => match.String),
						Literal(ticks),
						match => match.Product.Of2)));

		public static readonly IParsingExpression<StringLiteralExpression>
		StringLiteralExpression =
			Named(() => StringLiteralExpression,
				Reference(() => StringLiteral, match => new StringLiteralExpression(match.Product, match.SourceRange)));

		public static readonly IParsingExpression<string>
		StringLiteral =
			Named(() => StringLiteral,
				ChoiceOrdered(
					singleQuotedStringExpression,
					doubleQuotedStringExpression,
					verbatimStringExpression));

		#endregion

		#region UriLiteralExpression

		private static readonly IParsingExpression<object>
		uriExpressionPart = 
			Named("UriExpressionPart",
				ChoiceOrdered(
					Reference(() => uriExpressionRegularPart),
					Reference(() => uriExpressionParenthesizedPart)));

		private static readonly IParsingExpression<IEnumerable<Nil>>
		uriExpressionRegularPart =
			AtLeast(1, 
				ChoiceUnordered(
					CharacterIn(
						englishAlphaCharValues,
						digitCharValues,
						new char[] { '/', '?', ':', '@', '&', '=', '+', '$', '-', '_', '!', '~', '*', '\'', '.', ';' }),
					Sequence(
						Literal("%"),
						Exactly(2, Reference(() => HexDigit)))));

		private static readonly IParsingExpression<Nil>
		uriExpressionParenthesizedPart =
			Sequence(
				Literal("("),
				AtLeast(0, Reference(() => uriExpressionPart)),
				Literal(")"));

		public static readonly IParsingExpression<UriLiteralExpression>
		UriLiteralExpression =
			Named(() => UriLiteralExpression,
				Reference(() => UriLiteral, match => new UriLiteralExpression(match.Product, match.SourceRange)));

		public static readonly IParsingExpression<string>
		UriLiteral =
			Named(() => UriLiteral,
				AtLeast(1,
					ChoiceUnordered(
						Reference(() => uriExpressionRegularPart),
						Reference(() => uriExpressionParenthesizedPart)),
					match => match.String));

		#endregion

		#region ExpressionKeyword

		public static readonly IParsingExpression<Nil>
		ExpressionKeyword =
			Named(() => ExpressionKeyword,
				Sequence(
					ChoiceUnordered(new string[] {
						"break",
						"case",
						"catch",
						"class",
						"const",
						"continue",
						"debugger",
						"default",
						"delete",
						"do",
						"else",
						"enum",
						"export",
						"extends",
						"false",
						"finally",
						"for",
						"function",
						"if",
						"import",
						"instanceof",
						"in",
						"new",
						"null",
						"return",
						"super",
						"switch",
						"this",
						"throw",
						"true",
						"try",
						"typeof",
						"var",
						"void",
						"while",
						"with"
					}.Select(Literal)),
					NotAhead(Reference(() => IdentifierPart))));

			#endregion

		public static readonly IParsingExpression<IEnumerable<Nil>>
		ExpressionWhitespaceNoNewline =
			Named(() => ExpressionWhitespaceNoNewline,
				AtLeast(0,
					ChoiceUnordered(
						Reference(() => SpaceChar),
						Reference(() => Comment))));

		public static readonly IParsingExpression<Nil>
		ExpressionWhitespace =
			Named(() => ExpressionWhitespace,
				AtLeast(0,
					ChoiceUnordered(
						Reference(() => Whitespace),
						Reference(() => Comment)),
					match => Nil.Value));

		public static readonly IParsingExpression<Nil>
		ExpressionUnicodeEscapeSequence =
			Named(() => ExpressionUnicodeEscapeSequence,
				Sequence(
					Literal("u"),
					Exactly(4, Reference(() => HexDigit))));

		#endregion

		#region Comments

		public static readonly IParsingExpression<Nil>
		Comment =
			Named(() => Comment,
				ChoiceUnordered(
					Reference(() => SingleLineComment),
					Reference(() => MultiLineComment)));

		public static readonly IParsingExpression<Nil>
		SingleLineComment =
			Named(() => SingleLineComment,
				Sequence(
					Literal("//"),
					AtLeast(0, Sequence(NotAhead(Reference(() => NewLine)), Reference(() => UnicodeCharacter)))));

		public static readonly IParsingExpression<Nil>
		MultiLineComment =
			Named(() => MultiLineComment,
				Sequence(
					Literal("/*"),
					AtLeast(0, Sequence(NotAhead(Literal("*/")), Reference(() => UnicodeCharacter))),
					Literal("*/")));

		#endregion

		#region Text, Character Classes, etc

		public static readonly IParsingExpression<Nil>
		OptionalBlockWhitespace =
			Named(() => OptionalBlockWhitespace,
				Optional(Reference(() => BlockWhitespace)));

		public static readonly IParsingExpression<Nil>
		BlockWhitespace =
			Named(() => BlockWhitespace,
				Sequence(
					Ahead(Reference(() => Whitespace)),
					Reference(() => SpaceChars),
					Optional(Reference(() => NewLine)),
					Reference(() => SpaceChars),
					NotAhead(Reference(() => BlankLine))));

		/// <summary>
		/// A raw line of input, including the newline character.
		/// </summary>
		public static readonly IParsingExpression<LineInfo>
		Line =
			Named(() => Line,
				Sequence(
					AtLeast(0,
						Sequence(
							NotAhead(Reference(() => NewLine)),
							Reference(() => UnicodeCharacter))),
					Optional(Reference(() => NewLine)),
					match => LineInfo.FromMatch(match)));

		public static readonly IParsingExpression<IEnumerable<LineInfo>>
		BlankLines =
			Named(() => BlankLines,
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
						match => LineInfo.FromMatch(match).InEnumerable(),
						noMatch => Enumerable.Empty<LineInfo>()),
					match => match.Product.Of1.Concat(match.Product.Of2)));

		/// <summary>
		/// A blank line; composed of any number of spaces followed by a line end or the end of the input.
		/// </summary>
		public static readonly IParsingExpression<LineInfo>
		BlankLine =
			Named(() => BlankLine,
				Sequence(
					Reference(() => SpaceChars),
					ChoiceUnordered(
						Reference(() => NewLine),
						EndOfInput()),
					match => LineInfo.FromMatch(match)));

		public static readonly IParsingExpression<Nil>
		Indent =
			Named(() => Indent,
				ChoiceUnordered(Literal("\t"), Literal("    ")));

		public static readonly IParsingExpression<string>
		NonIndentSpace =
			Named(() => NonIndentSpace,
				AtMost(3, Literal(" "), match => match.String));

		/// <summary>
		/// A tab or a space.
		/// </summary>
		public static readonly IParsingExpression<Nil>
		SpaceChar =
			Named(() => SpaceChar,
				CharacterIn(spaceCharValues));

		/// <summary>
		/// Any number of tabs or spaces.
		/// </summary>
		public static readonly IParsingExpression<string>
		SpaceChars =
			Named(() => SpaceChars,
				AtLeast(0, Reference(() => SpaceChar), match => match.String));

		/// <summary>
		/// A newline character.
		/// </summary>
		public static readonly IParsingExpression<Nil>
		NewLine =
			Named(() => NewLine,
				ChoiceUnordered(
					Literal("\n"),
					Literal("\r\n")));

		/// <summary>
		/// A whitespace character; space, tab or newline.
		/// </summary>
		public static readonly IParsingExpression<Nil>
		Whitespace =
			Named(() => Whitespace,
				ChoiceUnordered(
					Reference(() => SpaceChar),
					Reference(() => NewLine)));

		public static readonly IParsingExpression<IEnumerable<Nil>>
		Whitespaces =
			Named(() => Whitespaces,
				AtLeast(0,
					CharacterIn(whitespaceCharValues)));

		public static readonly IParsingExpression<Nil>
		Digit =
			Named(() => Digit,
				CharacterInRange('0', '9'));

		public static readonly IParsingExpression<Nil>
		NonZeroDigit =
			Named(() => NonZeroDigit,
				CharacterInRange('1', '9'));

		public static readonly IParsingExpression<Nil>
		HexDigit =
			Named(() => HexDigit,
				CharacterIn(hexadecimalCharValues));

		public static readonly IParsingExpression<Nil>
		EnglishLowerAlpha =
			Named(() => EnglishLowerAlpha,
				CharacterInRange('a', 'z'));

		public static readonly IParsingExpression<Nil>
		EnglishUpperAlpha =
			Named(() => EnglishUpperAlpha,
				CharacterInRange('A', 'Z'));

		public static readonly IParsingExpression<Nil>
		EnglishAlpha =
			Named(() => EnglishAlpha,
				CharacterIn(englishAlphaCharValues));

		public static readonly IParsingExpression<Nil>
		UnicodeCharacter =
			Named(() => UnicodeCharacter,
				UnicodeParsingExpressions.UnicodeCharacterIn(UnicodeCategories.All));

		#endregion
	}
}