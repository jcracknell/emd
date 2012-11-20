using markdom.cs.Expressions;
using pegleg.cs;
using pegleg.cs.Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public partial class MarkdomGrammar {
		#region Expressions

		public static readonly IParsingExpression<IExpression>
		Expression =
			Named(() => Expression,
				Reference(() => AdditiveExpression));

		#region ShiftExpression

		public static readonly IParsingExpression<IExpression>
		ShiftExpression =
			Sequence(
				Reference(() => AdditiveExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						ChoiceOrdered(
							Reference(() => leftShiftExpressionPart),
							Reference(() => unsignedRightShiftExpressionPart),
							Reference(() => rightShiftExpressionPart)),
						match => match.Product.Of2)),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)));

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		leftShiftExpressionPart =
			Sequence(
				Literal("<<"),
				Reference(() => ExpressionWhitespace),
				Reference(() => AdditiveExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new LeftShiftExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		rightShiftExpressionPart =
			Sequence(
				Literal(">>"),
				Reference(() => ExpressionWhitespace),
				Reference(() => AdditiveExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new RightShiftExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		private static readonly IParsingExpression<Func<IExpression, IExpression>>
		unsignedRightShiftExpressionPart =
			Sequence(
				Literal(">>>"),
				Reference(() => ExpressionWhitespace),
				Reference(() => AdditiveExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new UnsignedRightShiftExpression(left, match.Product.Of3, left.SourceRange.Through(match.SourceRange));
					return asm;
				});

		#endregion

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
	}
}
