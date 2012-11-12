﻿using markdom.cs.Expressions;
using markdom.cs.Nodes;
using markdom.cs.Utils;
using pegleg.cs;
using pegleg.cs.Parsing;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace markdom.cs.Grammar {
	public class MarkdomGrammar : Grammar<MarkdomDocumentNode> {
		public readonly IParsingExpression<LineInfo> Comment;
		public readonly IParsingExpression<LineInfo> MultiLineComment;
		public readonly IParsingExpression<LineInfo> SingleLineComment;

		public readonly IParsingExpression<MarkdomDocumentNode> Document;

		public readonly IParsingExpression<IEnumerable<IBlockNode>> Blocks;
		public readonly IParsingExpression<IBlockNode> Block;

		public readonly IParsingExpression<LineInfo> CommentBlock;
		public readonly IParsingExpression<BlockquoteNode> Blockquote;
		public readonly IParsingExpression<TableNode> Table;
		public readonly IParsingExpression<TableRowNode> TableRow;
		public readonly IParsingExpression<LineInfo> TableRowSeparator;
		public readonly IParsingExpression<HeadingNode> Heading;
		public readonly IParsingExpression<int> HeadingAnnouncement;
		public readonly IParsingExpression<OrderedListNode> OrderedList;
		public readonly IParsingExpression<Nil> Enumerator;
		public readonly IParsingExpression<Nil> EnumeratorishAhead;
		public readonly IParsingExpression<int?> EnumeratorValue;
		public readonly IParsingExpression<UnorderedListNode> UnorderedList;
		public readonly IParsingExpression<UnorderedListNode> UnorderedListTight;
		public readonly IParsingExpression<UnorderedListNode> UnorderedListLoose;
		public readonly IParsingExpression<Nil> Bullet;
		public readonly IParsingExpression<ParagraphNode> Paragraph;
		public readonly IParsingExpression<ReferenceNode> ReferenceBlock;
		public readonly IParsingExpression<LineInfo> NonEmptyBlockLine;
		public readonly IParsingExpression<LineInfo> BlockLine;
		public readonly IParsingExpression<Nil> BlockLineAtomic;
		public readonly IParsingExpression<Nil> Atomic;

		public readonly IParsingExpression<IEnumerable<IInlineNode>> Inlines;
		public readonly IParsingExpression<IInlineNode> Inline;
		public readonly IParsingExpression<AutoLinkNode> AutoLink;
		public readonly IParsingExpression<LinkNode> Link;
		public readonly IParsingExpression<IEnumerable<IInlineNode>> Label;
		public readonly IParsingExpression<StrongNode> Strong;
		public readonly IParsingExpression<EmphasisNode> Emphasis;
		public readonly IParsingExpression<InlineExpressionNode> InlineExpression;
		public readonly IParsingExpression<QuotedNode> Quoted;
		public readonly IParsingExpression<CodeNode> Code;
		public readonly IParsingExpression<LineBreakNode> LineBreak;
		public readonly IParsingExpression<TextNode> Text;
		public readonly IParsingExpression<SpaceNode> Space;
		public readonly IParsingExpression<EntityNode> Entity;
		public readonly IParsingExpression<ReferenceId> ReferenceLabel;
		public readonly IParsingExpression<SymbolNode> Symbol;

		public readonly IParsingExpression<IExpression> Expression;
		public readonly IParsingExpression<IExpression> LeftHandSideExpression;
		public readonly IParsingExpression<IExpression> AtExpression;
		public readonly IParsingExpression<IExpression> AtExpressionRequired;
		public readonly IParsingExpression<IdentifierExpression> IdentifierExpression;
		public readonly IParsingExpression<string> IdentifierName;
		public readonly IParsingExpression<IExpression> PrimaryExpression;
		public readonly IParsingExpression<IEnumerable<IExpression>> ArgumentList;
		public readonly IParsingExpression<ObjectLiteralExpression> ObjectLiteralExpression;
		public readonly IParsingExpression<ObjectLiteralExpression> ObjectBodyExpression;
		public readonly IParsingExpression<IExpression> LiteralExpression;
		public readonly IParsingExpression<NullLiteralExpression> NullLiteralExpression;
		public readonly IParsingExpression<BooleanLiteralExpression> BooleanLiteralExpression;
		public readonly IParsingExpression<NumericLiteralExpression> NumericLiteralExpression;
		public readonly IParsingExpression<StringLiteralExpression> StringLiteralExpression;
		public readonly IParsingExpression<UriLiteralExpression> UriLiteralExpression;
		public readonly IParsingExpression<DocumentLiteralExpression> DocumentLiteralExpression;
		public readonly IParsingExpression<Nil> ExpressionKeyword;
		public readonly IParsingExpression<Nil> ExpressionWhitespace;
		public readonly IParsingExpression<Nil> ExpressionUnicodeEscapeSequence;

		/// <summary>
		/// A tab or a space.
		/// </summary>
		public readonly IParsingExpression<Nil> SpaceChar;
		public readonly IParsingExpression<string> SpaceChars;
		/// <summary>
		/// A whitespace character; space, tab or newline.
		/// </summary>
		public readonly IParsingExpression<Nil> Whitespace;
		public readonly IParsingExpression<IEnumerable<Nil>> Whitespaces;
		public readonly IParsingExpression<IEnumerable<Nil>> WhitespaceNoBlankLines;
		public readonly IParsingExpression<Nil> WhitespaceNoBlankLine;
		/// <summary>
		/// A newline character.
		/// </summary>
		public readonly IParsingExpression<string> NewLine;
		public readonly IParsingExpression<Nil> SpecialChar;
		public readonly IParsingExpression<Nil> NormalChar;
		public readonly IParsingExpression<string> Indent;
		public readonly IParsingExpression<string> NonIndentSpace;
		/// <summary>
		/// A raw line of input, including the newline character.
		/// </summary>
		public readonly IParsingExpression<LineInfo> Line;
		/// <summary>
		/// A blank line; composed of any number of spaces followed by a line end.
		/// </summary>
		public readonly IParsingExpression<IEnumerable<LineInfo>> BlankLines;
		public readonly IParsingExpression<LineInfo> BlankLine;
		public readonly IParsingExpression<LineInfo> NonTerminalBlankLine;
		public readonly IParsingExpression<Nil> Digit;
		public readonly IParsingExpression<Nil> NonZeroDigit;
		public readonly IParsingExpression<Nil> HexDigit;
		public readonly IParsingExpression<Nil> EnglishLowerAlpha;
		public readonly IParsingExpression<Nil> EnglishUpperAlpha;
		public readonly IParsingExpression<Nil> EnglishAlpha;
		public readonly IParsingExpression<Nil> UnicodeCharacter;

		public MarkdomGrammar() {
			#region char sets
			var spaceCharValues = new char[] { ' ', '\t' };
			var whitespaceCharValues = spaceCharValues.Concat(new char[] { '\n', '\r' });
			var englishLowerAlphaCharValues = CharUtils.Range('a','z');
			var englishUpperAlphaCharValues = CharUtils.Range('A','Z');
			var englishAlphaCharValues = englishLowerAlphaCharValues.Concat(englishUpperAlphaCharValues);
			var digitCharValues = CharUtils.Range('0','9');
			var hexadecimalCharValues = digitCharValues.Concat(CharUtils.Range('A','F')).Concat(CharUtils.Range('a','f'));
			var specialCharValues = new char[] {
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

			Define(() => Document,
				Reference(() => Blocks, match => new MarkdomDocumentNode(match.Product.ToArray(), match.SourceRange)));

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
						Sequence( NotAhead(Literal("*/")), Reference(() => UnicodeCharacter)),
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
							.Select(i => new UnorderedListItemNode(ParseLinesAs(Inlines, i.Lines.ToArray()).ToArray(), i.SourceRange))
							.ToArray();
						return new UnorderedListNode(items, match.SourceRange);
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
							.Select(i => new UnorderedListItemNode(ParseLinesAs(Blocks, i.Lines).ToArray(), i.SourceRange))
							.ToArray();
						return new UnorderedListNode(items, match.SourceRange);
					}));
			#endregion

			#endregion

			#region Headings
			Define(() => Heading,
				Sequence(
					Reference(() => HeadingAnnouncement),
					Reference(() => SpaceChars),
					Reference(() => BlockLine),
					match => new HeadingNode(match.Product.Of3.LineString, match.Product.Of1, match.SourceRange)));

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
						match.SourceRange)));

			#endregion

			Define(() => ReferenceBlock,
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

			Define(() => Paragraph,
				Sequence(
					Reference(() => SpaceChars),
					AtLeast(1, Reference(() => Inline)),
					Reference(() => BlankLine),
					match => new ParagraphNode(match.Product.Of2.ToArray(), match.SourceRange)));

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
					Sequence(NotAhead(CharacterIn(specialCharValues)), Reference(() => UnicodeCharacter)),
					ChoiceUnordered(
						Reference(() => SingleLineComment),
						Reference(() => MultiLineComment),
						Reference(() => InlineExpression)),
					Reference(() => UnicodeCharacter)));

			#region Tables

			Define(() => Table,
				AtLeast(1,
					Sequence(
						AtLeast(0, Reference(() => TableRowSeparator)),
						Reference(() => TableRow),
						AtLeast(0, Reference(() => TableRowSeparator)),
						match => match.Product.Of2),
					match => new TableNode(match.Product.ToArray(), match.SourceRange)));

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
							ParseLinesAs(Inlines, match.Product.Of2.InEnumerable()).ToArray(),
							match.SourceRange));

			var tableDataCell =
				Sequence(
					tableDataCellAnnouncement,
					tableCellContents,
					match => new TableDataCellNode(
						match.Product.Of1.ColumnSpan,
						match.Product.Of1.RowSpan,
						ParseLinesAs(Inlines, match.Product.Of2.InEnumerable()).ToArray(),
						match.SourceRange));

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
					match => new TableRowNode(match.Product.Of2.ToArray(), match.SourceRange)));

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
						Reference(() => Code),
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
					match => new AutoLinkNode(match.Product.Of3, match.Product.Of6.ToArray(), match.SourceRange)));
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
					match => new LinkNode(match.Product.Of1.ToArray(), match.Product.Of3.Item1, match.Product.Of3.Item2.ToArray(), match.SourceRange)));

			Define(() => ReferenceLabel,
				Sequence(
					Literal("["),
					AtLeast(0, 
						Sequence(
							NotAhead(CharacterIn(']', '\n', '\r')),
							Reference(() => UnicodeCharacter)),
						match => match.String),
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
					Ahead(Literal("@")),
					Reference(() => Expression),
					Optional(Literal(";")),
					match => new InlineExpressionNode(match.Product.Of2, match.SourceRange)));

			Define(() => Strong,
				Sequence(
					Literal("**"),
					AtLeast(0, Sequence(NotAhead(Literal("**")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("**")),
					match => new StrongNode(match.Product.Of2.ToArray(), match.SourceRange)));

			Define(() => Emphasis,
				Sequence(
					Literal("*"),
					AtLeast(0, Sequence(NotAhead(Literal("*")), Reference(() => Inline), match => match.Product.Of2)),
					Optional(Literal("*")),
					match => new EmphasisNode(match.Product.Of2.ToArray(), match.SourceRange)));

			#region Quoted

			var singleQuoted =
				Sequence(
					Literal("'"),
					AtLeast(0, Sequence(NotAhead(Literal("'")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("'"),
					match => new QuotedNode(QuoteType.Single, match.Product.Of2.ToArray(), match.SourceRange));

			var doubleQuoted =
				Sequence(
					Literal("\""),
					AtLeast(0, Sequence(NotAhead(Literal("\"")), Reference(() => Inline), match => match.Product.Of2)),
					Literal("\""),
					match => new QuotedNode(QuoteType.Double, match.Product.Of2.ToArray(), match.SourceRange));

			Define(() => Quoted,
				ChoiceOrdered(doubleQuoted, singleQuoted));

			Define(() => LineBreak,
				Sequence(
					new IParsingExpression[] { Literal(@"\\"), Reference(() => SpaceChars), Reference(() => NewLine) },
					match => new LineBreakNode(match.SourceRange)));

			#endregion

			#region Code

			Define(() => Code,
				Sequence(
					Ahead(Literal("`")),
					ChoiceOrdered(
						Enumerable.Range(1,8).Reverse()
						.Select(i => Literal("".PadRight(i, '`')))
						.Select(ticks =>
							Sequence(
								ticks,
								Reference(() => WhitespaceNoBlankLines),
								AtLeast(0,
									Sequence(
										Reference(() => WhitespaceNoBlankLines),
										AtLeast(1,
											Sequence(
												NotAhead(ChoiceUnordered(ticks, Reference(() => Whitespace))),
												Reference(() => UnicodeCharacter)))),
									match => match.String),
								Reference(() => WhitespaceNoBlankLines),
								ticks,
								match => new CodeNode(match.Product.Of3, match.SourceRange)))),
					match => match.Product.Of2));

			#endregion

			#region Entities

			var decimalHtmlEntity =
				Sequence(
					Literal("&#"),
					Between(1, 6, Reference(() => Digit), match => match.String),
					Literal(";"),
					match => new EntityNode(int.Parse(match.Product.Of2), match.SourceRange));

			var hexHtmlEntity =
				Sequence(
					Literal("&#x"),
					Between(1, 6, Reference(() => HexDigit), match => match.String),
					Literal(";"),
					match => new EntityNode(Convert.ToInt32(match.Product.Of2, 16), match.SourceRange));

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
						match => new EntityNode(EntityNode.GetNamedEntityValue(entityName), match.SourceRange));
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
					match => new TextNode(match.String, match.SourceRange)));

			Define(() => Space,
				AtLeast(1, Reference(() => WhitespaceNoBlankLine), match => new SpaceNode(match.SourceRange)));

			Define(() => NormalChar,
				UnicodeCharacterNotIn(whitespaceCharValues, specialCharValues));

			Define(() => Symbol,
				Reference(() => SpecialChar, match => new SymbolNode(match.String, match.SourceRange)));

			Define(() => SpecialChar,
				CharacterIn(specialCharValues));

			#endregion

			#region Expressions

			Define(() => Expression,
				Reference(() => LeftHandSideExpression));

			#region LeftHandSideExpression
			
			var dynamicPropertyExpressionPart =
				Sequence(
					Literal("["), Reference(() => ExpressionWhitespace),
					Reference(() => Expression),
					Reference(() => ExpressionWhitespace), Literal("]"),
					match => {
						Func<IExpression, IExpression> asm = body =>
							new DynamicPropertyExpression(
								body, match.Product.Of3,
								new SourceRange(body.SourceRange.Index, match.SourceRange.Index - body.SourceRange.Index + match.SourceRange.Length, body.SourceRange.Line, body.SourceRange.LineIndex)
							);
						return asm;
					});

			var staticPropertyExpressionPart =
				Sequence(
					Literal("."), Reference(() => ExpressionWhitespace),
					Reference(() => IdentifierName),
					match => {
						Func<IExpression, IExpression> asm = body =>
							new StaticPropertyExpression(
								body, match.Product.Of3,
								new SourceRange(body.SourceRange.Index, match.SourceRange.Index - body.SourceRange.Index + match.SourceRange.Length, body.SourceRange.Line, body.SourceRange.LineIndex)
							);
						return asm;
					});

			var callExpressionPart =
				Reference(
					() => ArgumentList,
					match => {
						Func<IExpression, IExpression> asm = body =>
							new CallExpression(
								body, match.Product.ToArray(),
								new SourceRange(body.SourceRange.Index, match.SourceRange.Index - body.SourceRange.Index + match.SourceRange.Length, body.SourceRange.Line, body.SourceRange.LineIndex)
							);
						return asm;
					});

			Define(() => LeftHandSideExpression,
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

			var argumentSeparator =
				Sequence(
					Reference(() => ExpressionWhitespace),
					Literal(","),
					Reference(() => ExpressionWhitespace));

			var argumentListArguments =
				Optional(
					Sequence(
						Reference(() => Expression),
						AtLeast(0, Sequence( argumentSeparator, Reference(() => Expression), match => match.Product.Of2)),
						match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2)));

			Define(() => ArgumentList,
				Sequence(
					Sequence(Literal("("), Reference(() => ExpressionWhitespace)),
					argumentListArguments,
					Sequence(Reference(() => ExpressionWhitespace), Literal(")")),
					match => match.Product.Of2 ?? new IExpression[0]));

			#endregion

			#region AtExpression

			Define(() => AtExpression,
				ChoiceOrdered(
					Reference(() => AtExpressionRequired),
					Reference(() => PrimaryExpression)));

			Define(() => AtExpressionRequired,
				Sequence(
					Literal("@"),
					ChoiceOrdered(
						Reference(() => IdentifierExpression),
						Reference(() => PrimaryExpression)),
					match => match.Product.Of2));

			#endregion

			#region IdentifierExpression

			var identifierExpressionStart =
				ChoiceOrdered(
					UnicodeCharacterIn(
						UnicodeCategories.Lu,
						UnicodeCategories.Ll,
						UnicodeCategories.Lt,
						UnicodeCategories.Lm,
						UnicodeCategories.Nl),
					Literal("$"),
					Literal("_"),
					Sequence(Literal("\\"), Reference(() => ExpressionUnicodeEscapeSequence)));

			var identifierExpressionPart =
				ChoiceOrdered(
					identifierExpressionStart,
					UnicodeCharacterIn(
						UnicodeCategories.Mn,
						UnicodeCategories.Mc,
						UnicodeCategories.Nd,
						UnicodeCategories.Pc),
					CharacterIn(/*ZWNJ*/'\u200C', /*ZWJ*/'\u200D'));

			Define(() => IdentifierExpression,
				Reference(() => IdentifierName,
					match => new IdentifierExpression(match.Product, match.SourceRange)));

			Define(() => IdentifierName,
				Sequence(
					NotAhead(Reference(() => ExpressionKeyword)),
					identifierExpressionStart,
					AtLeast(0, identifierExpressionPart),
					match => match.String));

			#endregion

			Define(() => PrimaryExpression,
				ChoiceUnordered(
					Reference(() => LiteralExpression),
					Reference(() => ObjectLiteralExpression),
					Reference(() => DocumentLiteralExpression),
					Sequence(
						Literal("("), Reference(() => ExpressionWhitespace), Reference(() => Expression), Reference(() => ExpressionWhitespace), Literal(")"),
						match => match.Product.Of3)));
					
			#region ObjectExpression

			var objectPropertyAssignment =
				Sequence(
					ChoiceUnordered<IExpression>(
						Reference(() => IdentifierExpression),
						Reference(() => StringLiteralExpression),
						Reference(() => NumericLiteralExpression)),
					Reference(() => ExpressionWhitespace),
					Literal(":"),
					Reference(() => ExpressionWhitespace),
					Reference(() => Expression),
					match => new PropertyAssignment(match.Product.Of1, match.Product.Of5, match.SourceRange));

			var objectPropertyAssignments =
				Optional(
					Sequence(
						objectPropertyAssignment,
						AtLeast(0, Sequence(argumentSeparator, objectPropertyAssignment, match => match.Product.Of2)),
						match => match.Product.Of1.InEnumerable().Concat(match.Product.Of2)),
					match => match.Product,
					noMatch => Enumerable.Empty<PropertyAssignment>());

			Define(() => ObjectLiteralExpression,
				Sequence(
					Literal("{"),
					Reference(() => ExpressionWhitespace),
					objectPropertyAssignments,
					Reference(() => ExpressionWhitespace),
					Literal("}"),
					match => new ObjectLiteralExpression(match.Product.Of3.ToArray(), match.SourceRange)));

			Define(() => ObjectBodyExpression,
				Reference(() => objectPropertyAssignments, match => new ObjectLiteralExpression(match.Product.ToArray(), match.SourceRange)));

			#endregion

			#region DocumentLiteralExpression

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
							ParseLinesAs(Blocks, match.Product.Of2.InEnumerable()).ToArray(),
							match.SourceRange));
				}));

			#endregion

			Define(() => LiteralExpression,
				ChoiceOrdered<IExpression>(
					Reference(() => NullLiteralExpression),
					Reference(() => BooleanLiteralExpression),
					Reference(() => NumericLiteralExpression),
					Reference(() => StringLiteralExpression),
					Reference(() => UriLiteralExpression)));

			Define(() => NullLiteralExpression,
				Literal("null", match => new NullLiteralExpression(match.SourceRange)));

			#region BooleanLiteralExpression

			Define(() => BooleanLiteralExpression,
				ChoiceUnordered(
					Literal("true", match => new BooleanLiteralExpression(true, match.SourceRange)),
					Literal("false", match => new BooleanLiteralExpression(false, match.SourceRange))));

			#endregion

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
						match.SourceRange));

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
							m.SourceRange)),
					Sequence(
						requiredDecimalPart,
						optionalExponentPart,
						m => new NumericLiteralExpression(
							m.Product.Of1 * m.Product.Of2,
							m.SourceRange)));

			Define(() => NumericLiteralExpression,
				ChoiceOrdered<NumericLiteralExpression>(
					hexIntegerLiteral,
					decimalLiteral));

			#endregion

			#region StringLiteralExpression
			
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
							Reference(() => UnicodeCharacter),
							match => match.String)),
					match => match.Product.JoinStrings());

			var doubleQuotedStringExpressionContent =
				AtLeast(0,
					ChoiceUnordered(
						Literal("\\\"", match => "\""),
						stringExpressionEscapes,
						Sequence(
							NotAhead(ChoiceUnordered(Literal("\""), Reference(() => NewLine))),
							Reference(() => UnicodeCharacter),
							match => match.String)),
					match => match.Product.JoinStrings());

			var singleQuotedStringExpression =
				Sequence(
					Literal("'"), singleQuotedStringExpressionContent, Literal("'"),
					match => new StringLiteralExpression(match.Product.Of2, match.SourceRange));

			var doubleQuotedStringExpression =
				Sequence(
					Literal("\""), doubleQuotedStringExpressionContent, Literal("\""),
					match => new StringLiteralExpression(match.Product.Of2, match.SourceRange));

			var verbatimStringExpression =
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
							match => new StringLiteralExpression(match.Product.Of2, match.SourceRange))));

			Define(() => StringLiteralExpression,
				ChoiceOrdered(
					singleQuotedStringExpression,
					doubleQuotedStringExpression,
					verbatimStringExpression));

			#endregion

			#region UriLiteralExpression
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
						CharacterIn(
							englishAlphaCharValues,
							digitCharValues,
							new char[] { '/', '?', ':', '@', '&', '=', '+', '$', '-', '_', '!', '~', '*', '\'', '.', ';' }),
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
					match => new UriLiteralExpression(match.String, match.SourceRange)));

			#endregion

			#region ExpressionKeyword

			Define(() => ExpressionKeyword,
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
					NotAhead(identifierExpressionPart)));

			#endregion

			Define(() => ExpressionWhitespace,
				AtLeast(0,
					ChoiceUnordered(
						Reference(() => Whitespace),
						Reference(() => Comment)),
					match => Nil.Value));

			Define(() => ExpressionUnicodeEscapeSequence,
				Sequence(
					Literal("u"),
					Exactly(4, Reference(() => HexDigit))));

			#endregion

			#region Text, Character Classes, etc

			Define(() => WhitespaceNoBlankLines,
				AtLeast(0, Reference(() => WhitespaceNoBlankLine)));

			Define(() => WhitespaceNoBlankLine,
				ChoiceOrdered(
					Reference(() => SpaceChar),
					Sequence(
						Reference(() => NewLine),
						NotAhead(Reference(() => BlankLine)))));

			Define(() => Line,
				Sequence(
					AtLeast(0,
						Sequence(
							NotAhead(Reference(() => NewLine)),
							Reference(() => UnicodeCharacter))),
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
						match => LineInfo.FromMatch(match).InEnumerable(),
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
				CharacterIn(hexadecimalCharValues));

			Define(() => EnglishLowerAlpha,
				CharacterInRange('a', 'z'));

			Define(() => EnglishUpperAlpha,
				CharacterInRange('A', 'Z'));

			Define(() => EnglishAlpha,
				CharacterIn(englishAlphaCharValues));

			Define(() => UnicodeCharacter,
				UnicodeCharacterIn(UnicodeCategories.All));

			#endregion

		}
		
		protected IParsingExpression<Nil> UnicodeCharacterIn(params IEnumerable<UnicodeCodePoint>[] categories) {
			var normalCodePointDefinitions = categories.Flatten().Where(cp => !cp.IsSurrogatePair);
			var surrogatePairCodePointDefinitions = categories.Flatten().Where(cp => cp.IsSurrogatePair);

			var normalCodePoint =
				CharacterIn(normalCodePointDefinitions.Select(cp => cp.FirstCodeUnit));

			if(!surrogatePairCodePointDefinitions.Any())
				return normalCodePoint;

			// Define a rule which can be used to quickly match a valid first code unit for
			// a surrogate pair amongst the provided code points
			var surrogatePairLeadUnit =
				CharacterIn(
					categories.Flatten()
					.Where(cp => cp.IsSurrogatePair)
					.Select(cp => cp.FirstCodeUnit));

			var surrogatePair =
				ChoiceUnordered(
					categories.Flatten()
					.Where(cp => cp.IsSurrogatePair)
					.GroupBy(cp => cp.FirstCodeUnit)
					.Select(g =>
						Sequence(
							CharacterIn(g.Key),
							CharacterIn(g.Select(cp => cp.SecondCodeUnit))
						)
					)
				);

			return ChoiceOrdered(
				normalCodePoint,
				Sequence(
					Ahead(surrogatePairLeadUnit),
					surrogatePair));
		}

		protected IParsingExpression<Nil> UnicodeCharacterNotIn(params IEnumerable<char>[] chars) {
			bool[] forbiddenChars = new bool[char.MaxValue + 1];
			foreach(var forbiddenChar in chars.Flatten())
				forbiddenChars[forbiddenChar] = true;

			return UnicodeCharacterIn(UnicodeCategories.All.Where(cp => cp.IsSurrogatePair || !forbiddenChars[cp.FirstCodeUnit]));
		}

		private IParsingExpression<Nil> UnicodeCharacterNotIn(params char[] chars) {
			return UnicodeCharacterNotIn(chars.AsEnumerable());
		}

		private IParsingExpression<Nil> UnicodeCharacterNotIn(params IEnumerable<UnicodeCodePoint>[] categories) {
			var forbiddenCodepoints = new HashSet<UnicodeCodePoint>(categories.Flatten());
			return UnicodeCharacterIn(UnicodeCategories.All.Where(cp => !forbiddenCodepoints.Contains(cp)));
		}

		private int _parseLinesCount = 0;
		protected T ParseLinesAs<T>(IParsingExpression<T> expression, IEnumerable<LineInfo> lines) {
			lines = lines.ToList();
			_parseLinesCount++;
			var expressionMatchingContext =
				new MatchingContext(
					lines.Select(line => line.LineString).JoinStrings(),
					lines.Select(line => line.SourceRange).ToArray());
			
			var expressionMatchingResult = expression.Matches(expressionMatchingContext);

			if(!expressionMatchingResult.Succeeded)
				return default(T);

			return (T)expressionMatchingResult.Product;
		}
	}
}
