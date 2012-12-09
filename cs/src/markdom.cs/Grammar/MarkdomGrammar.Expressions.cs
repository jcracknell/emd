using markdom.cs.Expressions;
using markdom.cs.Utils;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public partial class MarkdomGrammar {

		#region Helpers

		protected static IParsingExpression<IExpression> BinaryOperatorExpression(IParsingExpression<IExpression> operand, IEnumerable<BinaryOperatorDefinition> ops) {
			return Sequence(
				operand,
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						ChoiceOrdered(ops.Select(op =>
							Sequence(
								op.Operator,
								Reference(() => ExpressionWhitespace),
								operand,
								match => {
									Func<IExpression, IExpression> asm = left =>
										op.Constructor(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
									return asm;
								}))),
						match => match.Product.Of2)),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)));
		}

		protected static IParsingExpression<IExpression> BinaryOperatorExpression(IParsingExpression<IExpression> operand, params BinaryOperatorDefinition[] ops) {
			return BinaryOperatorExpression(operand, ops.AsEnumerable());
		}

		#endregion

		#region Expressions

		public static readonly IParsingExpression<IExpression>
		Expression =
			Named(() => Expression,
				Reference(() => ConditionalExpression));

		#region ConditionalExpression

		public static readonly IParsingExpression<IExpression>
		ConditionalExpression = Named(() => ConditionalExpression,
			Sequence(
				Reference(() => LogicalOrExpression),
				Optional(
					Sequence(
						Reference(() => ExpressionWhitespace),
						Literal("?"),
						Reference(() => ExpressionWhitespace),
						Reference(() => ConditionalExpression),
						Reference(() => ExpressionWhitespace),
						Literal(":"),
						Reference(() => ExpressionWhitespace),
						Reference(() => ConditionalExpression),
						match => Tuple.Create(match.Product.Of4, match.Product.Of8))),
				match => null != match.Product.Of2
					? new ConditionalExpression(match.Product.Of1, match.Product.Of2.Item1, match.Product.Of2.Item2, match.SourceRange)
					: match.Product.Of1
			)
		);

		public static readonly IParsingExpression<IExpression>
		ConditionalExpressionNoIn = Named(() => ConditionalExpressionNoIn,
			Sequence(
				Reference(() => LogicalOrExpressionNoIn),
				Optional(
					Sequence(
						Reference(() => ExpressionWhitespace),
						Literal("?"),
						Reference(() => ExpressionWhitespace),
						Reference(() => ConditionalExpressionNoIn),
						Reference(() => ExpressionWhitespace),
						Literal(":"),
						Reference(() => ExpressionWhitespace),
						Reference(() => ConditionalExpressionNoIn),
						match => Tuple.Create(match.Product.Of4, match.Product.Of8))),
				match => null != match.Product.Of2
					? new ConditionalExpression(match.Product.Of1, match.Product.Of2.Item1, match.Product.Of2.Item2, match.SourceRange)
					: match.Product.Of1
			)
		);

		#endregion

		#region LogicalOrExpression

		public static readonly IParsingExpression<IExpression>
		LogicalOrExpression = Named(() => LogicalOrExpression,
			Sequence(
				Reference(() => LogicalAndExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => LogicalOrOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => LogicalAndExpression),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new LogicalOrExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<IExpression>
		LogicalOrExpressionNoIn = Named(() => LogicalOrExpressionNoIn,
			Sequence(
				Reference(() => LogicalAndExpressionNoIn),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => LogicalOrOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => LogicalAndExpressionNoIn),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new LogicalOrExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<Nil>
		LogicalOrOperator =
			Sequence(
				Literal("||"),
				NotAhead(Literal("=")));

		#endregion

		#region LogicalAndExpression

		public static readonly IParsingExpression<IExpression>
		LogicalAndExpression = Named(() => LogicalAndExpression,
			Sequence(
				Reference(() => BitwiseOrExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => LogicalAndOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseOrExpression),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new LogicalAndExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<IExpression>
		LogicalAndExpressionNoIn = Named(() => LogicalAndExpressionNoIn,
			Sequence(
				Reference(() => BitwiseOrExpressionNoIn),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => LogicalAndOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseOrExpressionNoIn),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new LogicalAndExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<Nil>
		LogicalAndOperator =
			Sequence(
				Literal("&&"),
				NotAhead(Literal("=")));

		#endregion

		#region BitwiseOrExpression

		public static readonly IParsingExpression<IExpression>
		BitwiseOrExpression = Named(() => BitwiseOrExpression,
			Sequence(
				Reference(() => BitwiseXOrExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseOrOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseXOrExpression),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new BitwiseOrExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<IExpression>
		BitwiseOrExpressionNoIn = Named(() => BitwiseOrExpressionNoIn,
			Sequence(
				Reference(() => BitwiseXOrExpressionNoIn),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseOrOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseXOrExpressionNoIn),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new BitwiseOrExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<Nil>
		BitwiseOrOperator =
			Sequence(
				Literal("|"),
				NotAhead(CharacterIn('|', '=')));

		#endregion

		#region BitwiseXOrExpression

		public static readonly IParsingExpression<IExpression>
		BitwiseXOrExpression = Named(() => BitwiseXOrExpression,
			Sequence(
				Reference(() => BitwiseAndExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseXOrOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseAndExpression),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new BitwiseXOrExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<IExpression>
		BitwiseXOrExpressionNoIn = Named(() => BitwiseXOrExpressionNoIn,
			Sequence(
				Reference(() => BitwiseAndExpressionNoIn),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseXOrOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseAndExpressionNoIn),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new BitwiseXOrExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<Nil>
		BitwiseXOrOperator =
			Sequence(
				Literal("^"),
				NotAhead(Literal("=")));

		#endregion

		#region BitwiseAndExpression

		public static readonly IParsingExpression<IExpression>
		BitwiseAndExpression = Named(() => BitwiseAndExpression,
			Sequence(
				Reference(() => EqualityExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseAndOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => EqualityExpression),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new BitwiseAndExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);

		public static readonly IParsingExpression<IExpression>
		BitwiseAndExpressionNoIn = Named(() => BitwiseAndExpressionNoIn,
			Sequence(
				Reference(() => EqualityExpressionNoIn),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						Reference(() => BitwiseAndOperator),
						Reference(() => ExpressionWhitespace),
						Reference(() => EqualityExpressionNoIn),
						match => {
							Func<IExpression, IExpression> asm = left =>
								new BitwiseAndExpression(left, match.Product.Of4, left.SourceRange.Through(match.SourceRange));
							return asm;
						})),
				match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left)))
		);
		
		public static readonly IParsingExpression<Nil>
		BitwiseAndOperator =
			Sequence(
				Literal("&"),
				NotAhead(CharacterIn('&', '=')));

		#endregion

		#region EqualityExpression

		private static readonly IEnumerable<BinaryOperatorDefinition>
		equalityOperators = new BinaryOperatorDefinition[] {
			new BinaryOperatorDefinition(
				Literal("==="),
				(left, right, range) => new StrictEqualsExpression(left, right, range)),
			new BinaryOperatorDefinition(
				Literal("=="),
				(left, right, range) => new EqualsExpression(left, right, range)),
			new BinaryOperatorDefinition(
				Literal("!=="),
				(left, right, range) => new StrictNotEqualsExpression(left, right, range)),
			new BinaryOperatorDefinition(
				Literal("!="),
				(left, right, range) => new NotEqualsExpression(left, right, range))
		};

		public static readonly IParsingExpression<IExpression>
		EqualityExpression = Named(() => EqualityExpression,
			BinaryOperatorExpression(
				Reference(() => RelationalExpression),
				equalityOperators));

		public static readonly IParsingExpression<IExpression>
		EqualityExpressionNoIn = Named(() => EqualityExpressionNoIn,
			BinaryOperatorExpression(
				Reference(() => RelationalExpressionNoIn),
				equalityOperators));

		#endregion

		#region RelationalExpression

		public static readonly IParsingExpression<IExpression>
		RelationalExpression = Named(() => RelationalExpression,
			Sequence(
				Reference(() => ShiftExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						ChoiceOrdered(
							Reference(() => greaterThanOrEqualToExpressionPart),
							Reference(() => lessThanOrEqualToExpressionPart),
							Reference(() => greaterThanExpressionPart),
							Reference(() => lessThanExpressionPart),
							Reference(() => instanceOfExpressionPart),
							Reference(() => inExpressionPart)),
							match => match.Product.Of2)),
					match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left))));

		public static readonly IParsingExpression<IExpression>
		RelationalExpressionNoIn = Named(() => RelationalExpressionNoIn,
			Sequence(
				Reference(() => ShiftExpression),
				AtLeast(0,
					Sequence(
						Reference(() => ExpressionWhitespace),
						ChoiceOrdered(
							Reference(() => greaterThanOrEqualToExpressionPart),
							Reference(() => lessThanOrEqualToExpressionPart),
							Reference(() => greaterThanExpressionPart),
							Reference(() => lessThanExpressionPart),
							Reference(() => instanceOfExpressionPart)),
							match => match.Product.Of2)),
					match => match.Product.Of2.Reduce(match.Product.Of1, (left, asm) => asm(left))));

		protected static readonly IParsingExpression<Func<IExpression, IExpression>>
		greaterThanOrEqualToExpressionPart =
			Sequence(
				Literal(">="),
				Reference(() => ExpressionWhitespace),
				Reference(() => ShiftExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new GreaterThanOrEqualToExpression(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
					return asm;
				});

		protected static readonly IParsingExpression<Func<IExpression, IExpression>>
		lessThanOrEqualToExpressionPart =
			Sequence(
				Literal("<="),
				Reference(() => ExpressionWhitespace),
				Reference(() => ShiftExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new LessThanOrEqualToExpression(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
					return asm;
				});

		protected static readonly IParsingExpression<Func<IExpression, IExpression>>
		greaterThanExpressionPart =
			Sequence(
				Literal(">"),
				Reference(() => ExpressionWhitespace),
				Reference(() => ShiftExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new GreaterThanExpression(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
					return asm;
				});

		protected static readonly IParsingExpression<Func<IExpression, IExpression>>
		lessThanExpressionPart =
			Sequence(
				Literal("<"),
				Reference(() => ExpressionWhitespace),
				Reference(() => ShiftExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new LessThanExpression(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
					return asm;
				});

		protected static readonly IParsingExpression<Func<IExpression, IExpression>>
		instanceOfExpressionPart =
			Sequence(
				Reference(() => InstanceOfKeyword),
				Reference(() => ExpressionWhitespace),
				Reference(() => ShiftExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new InstanceOfExpression(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
					return asm;
				});

		protected static readonly IParsingExpression<Func<IExpression, IExpression>>
		inExpressionPart =
			Sequence(
				Reference(() => InKeyword),
				Reference(() => ExpressionWhitespace),
				Reference(() => ShiftExpression),
				match => {
					Func<IExpression, IExpression> asm = left =>
						new InExpression(left, match.Product.Of3, left.SourceRange.Through(match.Product.Of3.SourceRange));
					return asm;
				});

		#endregion

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
				Reference(() => DeleteKeyword),
				NotAhead(Reference(() => IdentifierPart)),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new DeleteExpression(match.Product.Of4, match.SourceRange));

		private static readonly IParsingExpression<VoidExpression>
		voidExpression =
			Sequence(
				Reference(() => VoidKeyword),
				NotAhead(Reference(() => IdentifierPart)),
				Reference(() => ExpressionWhitespace),
				Reference(() => UnaryExpression),
				match => new VoidExpression(match.Product.Of4, match.SourceRange));

		private static readonly IParsingExpression<TypeofExpression>
		typeofExpression =
			Sequence(
				Reference(() => TypeOfKeyword),
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

		// v8 commit re: handling of unicode escapes in identifier names
		// https://code.google.com/p/v8/source/detail?r=8969
		// Invalid unicode escape is handled as literal u,
		// Invalid char added by unicode escape is checked by method call, parsing then returns
		// Token::Illegal

		private static readonly IParsingExpression<Nil>
		identifierExpressionStart =
			ChoiceOrdered(
				Character(
					UnicodeCriteria.NoCharacter
						.Except(
							UnicodeCategory.UppercaseLetter,
							UnicodeCategory.LowercaseLetter,
							UnicodeCategory.TitlecaseLetter,
							UnicodeCategory.ModifierLetter,
							UnicodeCategory.OtherLetter,
							UnicodeCategory.LetterNumber)
						.Except('$', '_')),
				Sequence(Literal("\\"), Reference(() => ExpressionUnicodeEscapeSequence)));

		public static readonly IParsingExpression<Nil>
		IdentifierPart =
			Named(() => IdentifierPart,
				ChoiceOrdered(
					identifierExpressionStart,
					Character(
						UnicodeCriteria.NoCharacter
							.Except(
								UnicodeCategory.NonSpacingMark,
								UnicodeCategory.SpacingCombiningMark,
								UnicodeCategory.DecimalDigitNumber,
								UnicodeCategory.ConnectorPunctuation)
							.Except(/*ZWNJ*/'\u200C', /*ZWJ*/'\u200D'))));

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
					match => JavaScriptUtils.IdentifierDecode(match.String)));

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

		public static readonly IParsingExpression<StringLiteralExpression>
		StringLiteralExpression = Named(() => StringLiteralExpression,
			Reference(() => StringLiteral, match => new StringLiteralExpression(match.Product, match.SourceRange))
		);

		public static readonly IParsingExpression<string>
		StringLiteral = Named(() => StringLiteral,
			ChoiceUnordered(
				Reference(() => SingleQuotedStringLiteral),
				Reference(() => DoubleQuotedStringLiteral),
				Reference(() => VerbatimStringLiteral))
		);

		public static readonly IParsingExpression<string>
		VerbatimStringLiteral = Named(() => VerbatimStringLiteral,
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
						match => match.Product.Of2)))
		);

		public static readonly IParsingExpression<string>
		DoubleQuotedStringLiteral = Named(() => DoubleQuotedStringLiteral,
			Sequence(
				Literal("\""),
				AtLeast(0,
					Sequence(NotAhead(Literal("\"")), Reference(() => StringLiteralCharacter)),
					match => JavaScriptUtils.StringDecode(match.String)),
				Literal("\""),
				match => match.Product.Of2)
		);

		public static readonly IParsingExpression<string>
		SingleQuotedStringLiteral = Named(() => SingleQuotedStringLiteral,
			Sequence(
				Literal("'"),
				AtLeast(0,
					Sequence(NotAhead(Literal("'")), Reference(() => StringLiteralCharacter)),
					match => JavaScriptUtils.StringDecode(match.String)),
				Literal("'"),
				match => match.Product.Of2)
		);

		public static readonly IParsingExpression<Nil>
		StringLiteralCharacter =
			ChoiceOrdered(
				Sequence(
					Literal("\\"),
					ChoiceOrdered(
						Reference(() => ExpressionHexadecimalEscapeSequence),
						Reference(() => ExpressionUnicodeEscapeSequence),
						Reference(() => NewLine),
						Reference(() => UnicodeCharacter))),
					Character(UnicodeCriteria.AnyCharacter.Except(lineTerminatorCharValues)));

		#endregion

		#region UriLiteralExpression

		public static readonly IParsingExpression<object>
		UriExpressionPart = Named(() => UriExpressionPart,
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
				AtLeast(0, Reference(() => UriExpressionPart)),
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
		DeleteKeyword =
			Sequence(Literal("delete"), NotAhead(Reference(() => IdentifierPart)));

		public static readonly IParsingExpression<Nil>
		InKeyword =
			Sequence(Literal("in"), NotAhead(Reference(() => IdentifierPart)));

		public static readonly IParsingExpression<Nil>
		InstanceOfKeyword =
			Sequence(Literal("instanceof"), NotAhead(Reference(() => IdentifierPart)));

		public static readonly IParsingExpression<Nil>
		TypeOfKeyword =
			Sequence(Literal("typeof"), NotAhead(Reference(() => IdentifierPart)));

		public static readonly IParsingExpression<Nil>
		VoidKeyword =
			Sequence(Literal("void"), NotAhead(Reference(() => IdentifierPart)));

		public static readonly IParsingExpression<Nil>
		ExpressionKeyword =
			Named(() => ExpressionKeyword,
				Sequence(
					LiteralIn(
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
						"with"),
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
		ExpressionHexadecimalEscapeSequence =
			Named(() => ExpressionHexadecimalEscapeSequence,
				Sequence(
					Literal("x"),
					Exactly(2, Reference(() => HexDigit))));

		public static readonly IParsingExpression<Nil>
		ExpressionUnicodeEscapeSequence =
			Named(() => ExpressionUnicodeEscapeSequence,
				Sequence(
					Literal("u"),
					Exactly(4, Reference(() => HexDigit))));

		#endregion
	}
}
