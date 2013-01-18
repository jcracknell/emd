using emd.cs.Expressions;
using emd.cs.Nodes;
using emd.cs.Utils;
using pegleg.cs;
using pegleg.cs.Parsing;
using pegleg.cs.Unicode.Criteria;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace emd.cs.Grammar {
	public partial class EmdGrammar : ParsingExpressions {
		private static readonly EmdGrammar _instance = new EmdGrammar();

		public static EmdGrammar Instance { get { return _instance; } }

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

		private static IEnumerable<char>
		spaceCharValues { get { return new char[] { ' ', '\t' }; } }

		private static IEnumerable<char>
		lineTerminatorCharValues { get { return new char[] { '\n', '\r', '\u2028', '\u2029' }; } }

		private static IEnumerable<char>
		whitespaceCharValues { get { return spaceCharValues.Concat(lineTerminatorCharValues); } }

		private static IEnumerable<char>
		englishLowerAlphaCharValues { get { return CharUtils.Range('a','z'); } }

		private static IEnumerable<char>
		englishUpperAlphaCharValues { get { return CharUtils.Range('A','Z'); } }

		private static IEnumerable<char>
		englishAlphaCharValues { get { return englishLowerAlphaCharValues.Concat(englishUpperAlphaCharValues); } }

		private static IEnumerable<char>
		digitCharValues { get { return CharUtils.Range('0','9'); } }

		private static IEnumerable<char>
		hexadecimalCharValues { get { return digitCharValues.Concat(CharUtils.Range('A','F')).Concat(CharUtils.Range('a','f')); } }

		private static IEnumerable<char>
		specialCharValues { get {
			return new char[] {
				'*', // strong, emphasis
				'\'', '"', // quotes
				'`', // ticks
				'/', // single-line comment
				'\\', // escape sequence
				'[', ']', // labels
				'<', '>', // autolinks
				'|', // table cell delimiter
				'@' // expressions
			};
		} }

		#endregion

		public static readonly IParsingExpression<DocumentNode>
		Document =
			Named(() => Document,
				Reference(() => Blocks, match => new DocumentNode(match.Product.ToArray(), match.SourceRange)));

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
				GraphemeIn(spaceCharValues));

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
				LiteralIn("\n", "\u2028", "\u2029", "\r", "\r\n"));

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
					GraphemeIn(whitespaceCharValues)));

		public static readonly IParsingExpression<Nil>
		Digit =
			Named(() => Digit,
				GraphemeInRange('0', '9'));

		public static readonly IParsingExpression<Nil>
		NonZeroDigit =
			Named(() => NonZeroDigit,
				GraphemeInRange('1', '9'));

		public static readonly IParsingExpression<Nil>
		HexDigit =
			Named(() => HexDigit,
				GraphemeIn(hexadecimalCharValues));

		public static readonly IParsingExpression<Nil>
		EnglishLowerAlpha =
			Named(() => EnglishLowerAlpha,
				GraphemeInRange('a', 'z'));

		public static readonly IParsingExpression<Nil>
		EnglishUpperAlpha =
			Named(() => EnglishUpperAlpha,
				GraphemeInRange('A', 'Z'));

		public static readonly IParsingExpression<Nil>
		EnglishAlpha =
			Named(() => EnglishAlpha,
				GraphemeIn(englishAlphaCharValues));

		public static readonly IParsingExpression<Nil>
		UnicodeCharacter =
			Named(() => UnicodeCharacter, Grapheme());

		#endregion
	}
}