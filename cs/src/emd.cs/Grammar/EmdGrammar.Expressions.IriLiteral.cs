using emd.cs.Expressions;
using pegleg.cs;
using pegleg.cs.Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public partial class EmdGrammar {

		public static readonly IParsingExpression<StringLiteralExpression>
		IriLiteralExpression =
			Named(() => IriLiteralExpression,
				Reference(() => IriLiteral, match => new StringLiteralExpression(match.Product, match.SourceRange)));

		public static readonly IParsingExpression<string>
		IriLiteral =
			ChoiceOrdered(
				Sequence(
					Reference(() => IPv4Address),
					Optional(Reference(() => IriPort)),
					AtLeast(0, Reference(() => IriAtom))),
				Sequence(
					Reference(() => NumericLiteral),
					AtLeast(1, Reference(() => IriAtom))),
				Sequence(
					NotAhead(Reference(() => IriIllegalStart)),
					AtLeast(1, Reference(() => IriAtom))),
				match => match.String);

		public static readonly IParsingExpression<Nil>
		IriAtom =
			ChoiceOrdered(
				ChoiceOrdered(
					Reference(() => IriNormalChar),
					Reference(() => IriPctEncoded),
					Reference(() => IriBalanced),
					Reference(() => IriBracketedIpLiteral)),
				Sequence(
					Reference(() => IriNonTerminalChar),
					Reference(() => IriAtom)));

		public static readonly ICodePointCriteria
		IriIllegalStarCharCriteria = CodePointCriteria.In(
			'(', '\'', '@', ',', ';',
			'0','1','2','3','4','5','6','7','8','9'
		);

		public static readonly IParsingExpression<Nil>
		IriIllegalStart = Grapheme(GraphemeCriteria.SingleCodePoint(IriIllegalStarCharCriteria));

		public static readonly IParsingExpression<Nil>
		IriBalanced =
			Sequence(
				Literal("("),
				AtLeast(0, 
					ChoiceOrdered(
						Reference(() => IriNormalChar),
						Reference(() => IriNonTerminalChar),
						Reference(() => IriPctEncoded),
						Reference(() => IriBalanced))),
				Literal(")"));

		public static readonly IParsingExpression<Nil>
		IriBracketedIpLiteral =
			Sequence(
				Literal("["),
				ChoiceOrdered(
					Reference(() => IPv6Address),
					Reference(() => IPvFutureAddress)),
				Literal("]"),
				Optional(Reference(() => IriPort)));

		public static readonly IParsingExpression<Nil>
		IriPort = Sequence(Literal(":"), AtLeast(1, Reference(() => Digit)));

		public static readonly ICodePointCriteria
		IriCharCriteria =
			CodePointCriteria.Or(
				CodePointCriteria.In(
					'a','b','c','d','e','f','g','h','i','j','k','l','m',
					'n','o','p','q','r','s','t','u','v','w','x','y','z',
					'A','B','C','D','E','F','G','H','I','J','K','L','M',
					'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
					'0','1','2','3','4','5','6','7','8','9',
					'-', '.', '_', '~', // unreserved
					/*':',*/ '/', '?', '#', /*'[', ']',*/ '@', // gen-delims
					'!', '$', '&', '\'', /*'(', ')',*/ '*', '+', /*',', ';',*/ '=' // sub-delims
				),
				// ucs-char
				CodePointCriteria.InRange(0xA0, 0xD7FF),
				CodePointCriteria.InRange(0xF900, 0xFDCF),
				CodePointCriteria.InRange(0xFDF0, 0xFFEF),
				CodePointCriteria.InRange(0x10000, 0x1FFFD),
				CodePointCriteria.InRange(0x20000, 0x2FFFD),
				CodePointCriteria.InRange(0x30000, 0x3FFFD),
				CodePointCriteria.InRange(0x40000, 0x4FFFD),
				CodePointCriteria.InRange(0x50000, 0x5FFFD),
				CodePointCriteria.InRange(0x60000, 0x6FFFD),
				CodePointCriteria.InRange(0x70000, 0x7FFFD),
				CodePointCriteria.InRange(0x80000, 0x8FFFD),
				CodePointCriteria.InRange(0x90000, 0x9FFFD),
				CodePointCriteria.InRange(0xA0000, 0xAFFFD),
				CodePointCriteria.InRange(0xB0000, 0xBFFFD),
				CodePointCriteria.InRange(0xC0000, 0xCFFFD),
				CodePointCriteria.InRange(0xD0000, 0xDFFFD),
				CodePointCriteria.InRange(0xE0000, 0xEFFFD));

		public static readonly ICodePointCriteria
		IriNonTerminalCharCriteria = CodePointCriteria.In(',', ';', ':');

		public static readonly IParsingExpression<Nil>
		IriNormalChar = Grapheme(GraphemeCriteria.SingleCodePoint(IriCharCriteria));

		public static readonly IParsingExpression<Nil>
		IriNonTerminalChar = Grapheme(GraphemeCriteria.SingleCodePoint(IriNonTerminalCharCriteria));

		public static readonly IParsingExpression<Nil>
		IriPctEncoded = Sequence(Literal("%"), Exactly(2, Reference(() => HexDigit)));

		public static readonly IParsingExpression<Nil>
		IPvFutureAddress =
			Sequence(
				Literal("v"), AtLeast(1, Reference(() => HexDigit)), Literal("."),
				AtLeast(1,
					Grapheme(GraphemeCriteria.SingleCodePoint(CodePointCriteria.In(
						'a','b','c','d','e','f','g','h','i','j','k','l','m',
						'n','o','p','q','r','s','t','u','v','w','x','y','z',
						'A','B','C','D','E','F','G','H','I','J','K','L','M',
						'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
						'0','1','2','3','4','5','6','7','8','9',
						'-', '.', '_', '~', // unreserved
						'!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=', // sub-delims
						':'
					)))));

		public static readonly IParsingExpression<Nil>
		IPv6Address = Named(() => IPv6Address,
			ChoiceOrdered(
				Sequence(
					Reference(() => IPv6H16),
					Exactly(5, Reference(() => IPv6CH16)),
					Literal(":"), Reference(() => IPv6Ls32)),
				Sequence(
					Literal(":"), Exactly(5, Reference(() => IPv6CH16)),
					Literal(":"), Reference(() => IPv6Ls32)),
				Sequence(
					Optional(Reference(() => IPv6H16)),
					Literal(":"), Exactly(4, Reference(() => IPv6CH16)),
					Literal(":"), Reference(() => IPv6Ls32)),
				Sequence(
					Optional(Sequence(Reference(() => IPv6H16), Optional(Reference(() => IPv6CH16)))),
					Literal(":"), Exactly(3, Reference(() => IPv6CH16)),
					Literal(":"), Reference(() => IPv6Ls32)),
				Sequence(
					Optional(Sequence(Reference(() => IPv6H16), AtMost(2, Reference(() => IPv6CH16)))),
					Literal(":"), Exactly(2, Reference(() => IPv6CH16)),
					Literal(":"), Reference(() => IPv6Ls32)),
				Sequence(
					Optional(Sequence(Reference(() => IPv6H16), AtMost(3, Reference(() => IPv6CH16)))),
					Literal(":"), Reference(() => IPv6CH16),
					Literal(":"), Reference(() => IPv6Ls32)),
				Sequence(
					Optional(Sequence(Reference(() => IPv6H16), AtMost(4, Reference(() => IPv6CH16)))),
					Literal("::"), Reference(() => IPv6Ls32)),
				Sequence(
					Optional(Sequence(Reference(() => IPv6H16), AtMost(5, Reference(() => IPv6CH16)))),
					Literal("::"), Reference(() => IPv6H16)),
				Sequence(
					Optional(Sequence(Reference(() => IPv6H16), AtMost(6, Reference(() => IPv6CH16)))),
					Literal("::"))
			)
		);

		public static readonly IParsingExpression<Nil>
		IPv6CH16 = Sequence(Literal(":"), Reference(() => IPv6H16));

		public static readonly IParsingExpression<IEnumerable<Nil>>
		IPv6H16 = Between(1, 4, Reference(() => EmdGrammar.HexDigit));

		public static readonly IParsingExpression<Nil>
		IPv6Ls32 =
			ChoiceOrdered(
				Reference(() => IPv4Address),
				Sequence(Reference(() => IPv6H16), Literal(":"), Reference(() => IPv6H16)));

		public static readonly IParsingExpression<Nil>
		IPv4Address =
			Sequence(
				Reference(() => IPv4DecOctet),
				Exactly(3, Sequence(Literal("."), Reference(() => IPv4DecOctet))));

		public static readonly IParsingExpression<Nil>
		IPv4DecOctet =
			ChoiceOrdered(
				Sequence(Literal("25"), CodePoint(CodePointCriteria.InRange('0', '5'))),
				Sequence(Literal("2"), CodePoint(CodePointCriteria.InRange('0', '4')), Reference(() => Digit)),
				Sequence(Literal("1"), Reference(() => Digit), Reference(() => Digit)),
				Sequence(CodePoint(CodePointCriteria.InRange('1','9')), Reference(() => Digit)),
				Reference(() => Digit));
	}
}
