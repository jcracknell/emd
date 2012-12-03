using markdom.cs.Expressions;
using markdom.cs.Nodes;
using markdom.cs.Utils;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public partial class MarkdomGrammar {
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
					Optional(Reference(() => ReferenceLabel)),
					Optional(Reference(() => ArgumentList, match => match.Product.ToArray())),
					match => new LinkNode(match.Product.Of1.ToArray(), match.Product.Of2, match.Product.Of3, match.SourceRange)));

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
					Reference(() => LeftHandSideExpression),
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
			ChoiceOrdered(
				EntityInfo.EntityNames.OrderByDescending(name => name.Length).Select(Literal),
				match => EntityInfo.GetEntityValue(match.String))
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
				CharacterIn('u','x'),
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
	}
}
