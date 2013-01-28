using emd.cs.Expressions;
using emd.cs.Nodes;
using emd.cs.Utils;
using pegleg.cs;
using pegleg.cs.Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public partial class EmdGrammar {

		/// <summary>
		/// Determines the extent of blocks containing rich inlines.
		/// A block is composed of atomics and newlines, thus a newline consumed by this production is invisible to the block.
		/// </summary>
		public static readonly IParsingExpression<Nil>
		RichAtomic =
			Named(() => RichAtomic,
				ChoiceOrdered(
					Sequence(NotAhead(GraphemeIn(specialCharValues)), Reference(() => UnicodeCharacter)),
					ChoiceUnordered(
						Reference(() => InlineExpression),
						Reference(() => Comment),
						Reference(() => RichLink),
						Reference(() => AutoLink),
						Reference(() => Code)),
					Reference(() => UnicodeCharacter)));

		/// <summary>
		/// Determines the extent of blocks containing formatted inlines.
		/// A block is composed of atomics and newlines, thus a newline consumed by this production is invisible to the block.
		/// </summary>
		public static readonly IParsingExpression<Nil>
		FormattedAtomic =
			Named(() => FormattedAtomic,
				ChoiceOrdered(
					Sequence(NotAhead(GraphemeIn(specialCharValues)), Reference(() => UnicodeCharacter)),
					ChoiceUnordered(
						Reference(() => Comment),
						Reference(() => Code)),
					Reference(() => UnicodeCharacter)));

		#region BlockInlines

		// These rules are used to parse block content.

		/// <summary>
		/// Parses rich inline elements within a block.
		/// </summary>
		public static readonly IParsingExpression<IEnumerable<IRichInlineNode>>
		RichBlockInlines = TBlockInlinesOptional(Reference(() => RichInline));

		/// <summary>
		/// Parses rich inline elements within a block.
		/// Only succeeds if at least one inline is matched; this distinction is important in the handling
		/// of empty blocks - for example paragraphs.
		/// </summary>
		public static readonly IParsingExpression<IEnumerable<IRichInlineNode>>
		RichBlockInlinesRequired =
			Sequence(
				Optional(Reference(() => BlockWhitespaceOrComments)),
				AtLeast(1, Reference(() => RichInline)),
				Optional(Reference(() => BlockWhitespaceOrComments)),
				match => match.Product.Of2);

		/// <summary>
		/// Parses formatted inline elements within a block.
		/// </summary>
		public static readonly IParsingExpression<IEnumerable<IFormattedInlineNode>>
		FormattedBlockInlines = TBlockInlinesOptional(Reference(() => FormattedInline));

		public static IParsingExpression<IEnumerable<TInline>> TBlockInlinesOptional<TInline>(IParsingExpression<TInline> inline) {
			return Sequence(
				Optional(Reference(() => BlockWhitespaceOrComments)),
				Optional(
					Sequence(
						AtLeast(1, inline),
						Optional(Reference(() => BlockWhitespaceOrComments)),
						match => match.Product.Of1),
					match => match.Product, noMatch => Enumerable.Empty<TInline>()),
				match => match.Product.Of2);
		}

		#endregion

		#region Rich

		public static readonly IParsingExpression<IRichInlineNode>
		RichInline =
			Named(() => RichInline,
				ChoiceOrdered<IRichInlineNode>(
					Reference(() => Text),
					Reference(() => LineBreak),
					Reference(() => Space),
					Reference(() => RichStrong),
					Reference(() => RichEmphasis),
					TQuoted(Reference(() => RichInline)),
					Reference(() => Quoted),
					Reference(() => RichLink),
					Reference(() => AutoLink),
					Reference(() => Code),
					Reference(() => Entity),
					Reference(() => InlineExpression),
					Reference(() => Symbol)));

		public static readonly IParsingExpression<LinkNode>
		RichLink = TLink(Reference(() => RichInline));

		public static readonly IParsingExpression<StrongNode>
		RichStrong = TStrong(Reference(() => RichInline));

		public static readonly IParsingExpression<EmphasisNode>
		RichEmphasis = TEmphasis(Reference(() => RichInline), Reference(() => RichStrong));

		#endregion

		#region Formatted

		public static readonly IParsingExpression<IFormattedInlineNode>
		FormattedInline =
			Named(() => FormattedInline,
				ChoiceOrdered<IFormattedInlineNode>(
					Reference(() => Text),
					Reference(() => LineBreak),
					Reference(() => Space),
					Reference(() => FormattedStrong),
					Reference(() => FormattedEmphasis),
					TQuoted(Reference(() => FormattedInline)),
					Reference(() => Quoted),
					Reference(() => Code),
					Reference(() => Entity),
					Reference(() => Symbol)));

		public static readonly IParsingExpression<StrongNode>
		FormattedStrong = TStrong(Reference(() => FormattedInline));

		public static readonly IParsingExpression<EmphasisNode>
		FormattedEmphasis = TEmphasis(Reference(() => FormattedInline), Reference(() => FormattedStrong));

		#endregion


		#region AutoLink

		public static readonly IParsingExpression<AutoLinkNode>
		AutoLink =
			Named(() => AutoLink,
				Sequence(
					Literal("<"),
					Reference(() => SpaceChars),
					Reference(() => IriLiteral),
					Reference(() => SpaceChars),
					Literal(">"),
					Optional(
						Reference(() => ArgumentList),
						match => match.Product,
						noMatch => new IExpression[0]),
					match => new AutoLinkNode(match.Product.Of3, match.Product.Of6.ToArray(), match.SourceRange)));

		#endregion

		#region Link

		public static IParsingExpression<LinkNode> TLink(IParsingExpression<IInlineNode> recurse) {
			return Sequence(
				TLabel(recurse),
				Optional(Reference(() => ReferenceLabel)),
				Optional(Reference(() => ArgumentList, match => match.Product.ToArray())),
				match => new LinkNode(match.Product.Of1.ToArray(), match.Product.Of2, match.Product.Of3, match.SourceRange));
		}

		public static IParsingExpression<IEnumerable<IInlineNode>> TLabel(IParsingExpression<IInlineNode> recurse) {
			return Sequence(
				Literal("["),
				AtLeast(0, Sequence(NotAhead(Literal("]")), recurse, match => match.Product.Of2)),
				Literal("]"),
				match => match.Product.Of2);
		}

		public static readonly IParsingExpression<ReferenceId>
		ReferenceLabel =
			Named(() => ReferenceLabel,
				Sequence(
					Literal("["),
					AtLeast(0, 
						Sequence(
							NotAhead(GraphemeIn(']', '\n', '\r')),
							Reference(() => UnicodeCharacter)),
						match => match.String),
					Literal("]"),
					match => ReferenceId.FromText(match.Product.Of2)));

		#endregion

		public static readonly IParsingExpression<InlineExpressionNode>
		InlineExpression =
			Named(() => InlineExpression,
				Sequence(
					Ahead(Literal("@")),
					Reference(() => LeftHandSideExpression),
					Optional(Literal(";")),
					match => new InlineExpressionNode(match.Product.Of2, match.SourceRange)));

		public static IParsingExpression<StrongNode> TStrong(IParsingExpression<IInlineNode> recurse) {
			return Sequence(
				Reference(() => StrongDelimiter),
				AtLeast(1,
					Sequence(
						NotAhead(Reference(() => StrongDelimiter)),
						Reference(() => RichInline),
						match => match.Product.Of2)),
				Reference(() => StrongDelimiter),
				match => new StrongNode(match.Product.Of2.ToArray(), match.SourceRange));
		}

		public static readonly IParsingExpression<Nil>
		StrongDelimiter = Literal("**");

		public static IParsingExpression<EmphasisNode> TEmphasis(IParsingExpression<IInlineNode> recurse, IParsingExpression<StrongNode> recurseStrong) {
			return Sequence(
				Reference(() => EmphasisDelimiter),
				AtLeast(1,
					ChoiceOrdered(
						Sequence(
							NotAhead(Reference(() => EmphasisDelimiter)),
							recurse,
							match => match.Product.Of2),
						recurseStrong)),
				Reference(() => EmphasisDelimiter),
				match => new EmphasisNode(match.Product.Of2.ToArray(), match.SourceRange));
		}

		public static readonly IParsingExpression<Nil>
		EmphasisDelimiter = Literal("*");

		#region Quoted

		public static readonly IParsingExpression<QuotedNode>
		Quoted =
			Named(() => Quoted,
				TQuoted(Reference(() => RichInline)));

		public static IParsingExpression<QuotedNode> TQuoted(IParsingExpression<IInlineNode> recurse) {
			return ChoiceOrdered(
					Sequence(
						Literal("'"),
						AtLeast(0, Sequence(NotAhead(Literal("'")), recurse, match => match.Product.Of2)),
						Literal("'"),
						match => new QuotedNode(QuoteType.Single, match.Product.Of2.ToArray(), match.SourceRange)),
					Sequence(
						Literal("\""),
						AtLeast(0, Sequence(NotAhead(Literal("\"")), recurse, match => match.Product.Of2)),
						Literal("\""),
						match => new QuotedNode(QuoteType.Double, match.Product.Of2.ToArray(), match.SourceRange)));
		}

		#endregion

		public static readonly IParsingExpression<LineBreakNode>
		LineBreak =
			Named(() => LineBreak,
				Sequence(
					Optional(Reference(() => BlockWhitespaceOrComments)),
					Literal("\\"),
					Ahead(Reference(() => BlankLine)),
					Reference(() => BlockWhitespaceOrComments),
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
								Optional(Reference(() => BlockWhitespace)),
								AtLeast(0,
									Sequence(
										AtLeast(1,
											Sequence(
												NotAhead(ChoiceUnordered(ticks, Reference(() => Whitespace))),
												Reference(() => UnicodeCharacter))),
										Optional(Reference(() => BlockWhitespace))),
									match => match.String),
								ticks,
								match => new CodeNode(match.Product.Of3, match.SourceRange)))),
					match => match.Product.Of2));

		#endregion

		#region Entities

		public static readonly IParsingExpression<EntityNode>
		Entity = Named(() => Entity,
			Sequence(
				Literal("\\"),
				ChoiceOrdered(
					Reference(() => NamedEntity),
					Reference(() => DecimalEntity),
					Reference(() => HexadecimalEntity)),
				Optional(Literal(";")),
				match => new EntityNode(match.Product.Of2, match.SourceRange))
		);

		public static readonly IParsingExpression<string>
		NamedEntity = Named(() => NamedEntity,
			LiteralIn(EntityInfo.EntityNames, match => EntityInfo.GetEntityValue(match.Product))
		);

		public static readonly IParsingExpression<string>
		DecimalEntity = Named(() => DecimalEntity,
			Sequence(
				Literal("#"),
				Between(1, 6, Reference(() => Digit), match => match.String),
				match => char.ConvertFromUtf32(int.Parse(match.Product.Of2)))
		);

		public static readonly IParsingExpression<string>
		HexadecimalEntity = Named(() => HexadecimalEntity,
			Sequence(
				Optional(Literal("#")),
				GraphemeIn('u','x'),
				Between(1, 6, Reference(() => HexDigit), match => match.String),
				match => char.ConvertFromUtf32(Convert.ToInt32(match.Product.Of3, 16)))
		);

		#endregion 

		public static readonly IParsingExpression<TextNode>
		Text =
			Named(() => Text,
				AtLeast(1,
					Reference(() => NormalChar),
					match => new TextNode(match.String, match.SourceRange)));

		/// <summary>
		/// Any non-empty combination of comments and whitespace not at the end of a block.
		/// </summary>
		public static readonly IParsingExpression<SpaceNode>
		Space =
			Named(() => Space,
				Sequence(
					Reference(() => BlockWhitespaceOrComments),
					NotAhead(Reference(() => BlankLine)), // No match at end of block.
					match => new SpaceNode(match.SourceRange)));

		/// <summary>
		/// Any non-empty combination of comments and whitespace not traversing a blank line (not leaving the current block).
		/// </summary>
		public static readonly IParsingExpression<Nil>
		BlockWhitespaceOrComments =
			Named(() => BlockWhitespaceOrComments,
				ChoiceOrdered(
					// whitespace followed by any number of comments interleaved with whitespace
					Sequence(
						Reference(() => BlockWhitespace),
						AtLeast(0,
							Sequence(
								Reference(() => Comment),
								Optional(Reference(() => BlockWhitespace))))),
					// at least one comment optionally interleaved with whitespace
					AtLeast(1,
						Sequence(
							Reference(() => Comment),
							Optional(Reference(() => BlockWhitespace))))));

		/// <summary>
		/// Any non-empty amount of whitespace not traversing a blank line (not leaving the current block).
		/// </summary>
		public static readonly IParsingExpression<Nil>
		BlockWhitespace =
			Named(() => BlockWhitespace,
				ChoiceOrdered(
					Sequence(
						AtLeast(1, Reference(() => SpaceChar)),
						Optional(
							Sequence(
								Reference(() => NewLine),
								Reference(() => SpaceChars),
								NotAhead(Reference(() => BlankLine))))),
					Sequence(
						Reference(() => NewLine),
						Reference(() => SpaceChars),
						NotAhead(Reference(() => BlankLine)))));

		public static readonly IParsingExpression<Nil>
		NormalChar =
			Named(() => NormalChar,
				Grapheme(GraphemeCriteria.Not(GraphemeCriteria.InValues(whitespaceCharValues, specialCharValues))));

		public static readonly IParsingExpression<SymbolNode>
		Symbol =
			Named(() => Symbol,
				Reference(() => SpecialChar, match => new SymbolNode(match.String, match.SourceRange)));

		public static readonly IParsingExpression<Nil>
		SpecialChar =
			Named(() => SpecialChar,
				GraphemeIn(specialCharValues));
	}
}
