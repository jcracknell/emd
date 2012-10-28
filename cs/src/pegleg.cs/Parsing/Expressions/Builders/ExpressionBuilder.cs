using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions.Builders {
	public class ExpressionBuilder : IExpressionBuilder {
		private const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.Singleline;
		private IExpressionNamingConvention _namingConvention = new DefaultExpressionNamingConvention();

		public void SetExpressionNamingConvention(IExpressionNamingConvention namingConvention) {
			CodeContract.ArgumentIsNotNull(() => namingConvention, namingConvention);

			_namingConvention = namingConvention;
		}

		#region Helpers

		private TProduct DefaultMatchAction<TProduct>(IExpressionMatch<TProduct> match) {
			return match.Product;
		}

		private IExpressionMatch<TDest> UpcastExpressionMatch<TSource, TDest>(IExpressionMatch<TSource> expressionMatch, Func<TSource, TDest> cast) {
			return new UpcastedExpressionMatch<TSource, TDest>(expressionMatch, cast);
		}

		private TDest[] CastArray<TDest>(object[] source) {
			var dest = new TDest[source.Length];
			for(var i = 0; i < source.Length; i++)
				dest[i] = (TDest)source[i];

			return dest;
		}

		#endregion

		public IExpression<T> Named<T>(string name, IExpression<T> expression) {
			var conventionalName = _namingConvention.Apply(name, expression);
			return new NamedExpression<T>(conventionalName, expression);
		}

		public IExpression<string> Literal(string literal) {
			return new LiteralExpression<string>(literal, null);
		}

		public IExpression<TProduct> Literal<TProduct>(string literal, Func<IExpressionMatch<string>, TProduct> matchAction) {
			return new LiteralExpression<TProduct>(literal, matchAction);
		}

		public IExpression<string> Literal(char literal) {
			return Literal(literal.ToString());
		}

		public IExpression<TProduct> Literal<TProduct>(char literal, Func<IExpressionMatch<string>, TProduct> matchAction) {
			return Literal(literal.ToString(), matchAction);
		}

		public IExpression<Match> Regex(Regex regex) {
			return new RegexExpression<Match>(regex, null);
		}

		public IExpression<Match> Regex(string regex) {
			return new RegexExpression<Match>(new Regex(regex, REGEX_OPTIONS), null);
		}

		public IExpression<TProduct> Regex<TProduct>(Regex regex, Func<IExpressionMatch<Match>, TProduct> matchAction) {
			return new RegexExpression<TProduct>(regex, matchAction);
		}

		public IExpression<TProduct> Regex<TProduct>(string regex, Func<IExpressionMatch<Match>, TProduct> matchAction) {
			return Regex(new Regex(regex, REGEX_OPTIONS), matchAction);
		}

		public IExpression<Nil> EndOfInput() {
			return EndOfInput(DefaultMatchAction);
		}

		public IExpression<TProduct> EndOfInput<TProduct>(Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			return new EndOfInputExpression<TProduct>(matchAction);
		}

		public IExpression<T> Dynamic<T>(Func<IExpression<T>> closure) {
			return new DynamicExpression<T>(closure);
		}

		public IExpression<Nil> Assert(Func<bool> predicate) {
			return new PredicateExpression(predicate);
		}

		public IExpression<T> Ahead<T>(IExpression<T> expression) {
			return Ahead(expression, DefaultMatchAction);
		}

		public IExpression<TProduct> Ahead<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<T>, TProduct> matchAction) {
			return new AheadExpression<TProduct>(expression,
				match => matchAction(UpcastExpressionMatch(match, product => (T)product)));
		}

		public IExpression<T[]> AtLeast<T>(uint n, IExpression<T> expression) {
			return AtLeast(n, expression, DefaultMatchAction);
		}

		public IExpression<TProduct> AtLeast<T, TProduct>(uint n, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction) {
			return new RepetitionExpression<TProduct>(n, 0, expression,
				match => matchAction(UpcastExpressionMatch(match, product => CastArray<T>(product))));
		}

		public IExpression<T[]> AtMost<T>(uint maxOccurs, IExpression<T> expression) {
			return AtMost(maxOccurs, expression, DefaultMatchAction);
		}

		public IExpression<TProduct> AtMost<T, TProduct>(uint maxOccurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction) {
			return new RepetitionExpression<TProduct>(0, maxOccurs, expression,
				match => matchAction(UpcastExpressionMatch(match, product => CastArray<T>(product))));
		}

		public IExpression<T[]> Between<T>(uint minOccurs, uint maxOccurs, IExpression<T> expression) {
			return Between(minOccurs, maxOccurs, expression, DefaultMatchAction);
		}

		public IExpression<TProduct> Between<T, TProduct>(uint minOccurs, uint maxOccurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction) {
			return new RepetitionExpression<TProduct>(minOccurs, maxOccurs, expression,
				match => matchAction(UpcastExpressionMatch(match, product => CastArray<T>(product))));
		}

		public IExpression<T[]> Exactly<T>(uint occurs, IExpression<T> expression) {
			return Exactly(occurs, expression, DefaultMatchAction);
		}

		public IExpression<TProduct> Exactly<T, TProduct>(uint occurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction) {
			return new RepetitionExpression<TProduct>(occurs, occurs, expression,
				match => matchAction(UpcastExpressionMatch(match, product => CastArray<T>(product))));
		}

		public IExpression<object> OrderedChoice(params IExpression[] choices) {
			return OrderedChoice(choices, DefaultMatchAction);
		}

		public IExpression<TProduct> OrderedChoice<TProduct>(IExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction) {
			return new ChoiceExpression<TProduct>(choices, matchAction);
		}

		public IExpression<TChoice> OrderedChoice<TChoice>(params IExpression<TChoice>[] choices) {
			return OrderedChoice(choices, DefaultMatchAction);
		}

		public IExpression<TProduct> OrderedChoice<TChoice, TProduct>(IExpression<TChoice>[] choices, Func<IExpressionMatch<TChoice>, TProduct> matchAction) {
			return new ChoiceExpression<TProduct>(choices, match => matchAction(UpcastExpressionMatch(match, product => (TChoice)product)));
		}

		public IExpression<object> Choice(params IExpression[] choices) {
			return OrderedChoice(choices);
		}

		public IExpression<TProduct> Choice<TProduct>(IExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction) {
			return OrderedChoice(choices, matchAction);
		}

		public IExpression<TChoice> Choice<TChoice>(params IExpression<TChoice>[] choices) {
			return OrderedChoice(choices);
		}

		public IExpression<TProduct> Choice<TChoice, TProduct>(IExpression<TChoice>[] choices, Func<IExpressionMatch<TChoice>, TProduct> matchAction) {
			return OrderedChoice(choices, matchAction);
		}

		public IExpression<Nil> NotAhead<T>(IExpression<T> expression) {
			return NotAhead(expression, DefaultMatchAction);	
		}

		public IExpression<TProduct> NotAhead<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			return new NotAheadExpression<TProduct>(expression, matchAction);
		}

		public IExpression<T> Optional<T>(IExpression<T> expression) {
			return Optional(expression, DefaultMatchAction);
		}

		public IExpression<TProduct> Optional<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<T>, TProduct> matchAction) {
			return new OptionalExpression<TProduct>(expression, match => matchAction(UpcastExpressionMatch(match, product => (T)product)));
		}

		public IExpression<T> Reference<T>(Func<IExpression<T>> reference) {
			return Reference(reference, DefaultMatchAction);
		}

		public IExpression<TProduct> Reference<T, TProduct>(Func<IExpression<T>> reference, Func<IExpressionMatch<T>, TProduct> matchAction) {
			return new ReferenceExpression<TProduct>(reference, match => matchAction(UpcastExpressionMatch(match, product => (T)product)));
		}

		public IExpression<Nil> Nothing() {
			return new NothingExpression<Nil>(match => global::pegleg.cs.Nil.Value);
		}

		public IExpression<TProduct> Nothing<TProduct>(Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			return new NothingExpression<TProduct>(matchAction);
		}

		public IExpression<string> Wildcard() {
			return Wildcard(DefaultMatchAction);
		}

		public IExpression<TProduct> Wildcard<TProduct>(Func<IExpressionMatch<string>, TProduct> matchAction) {
			return new WildcardExpression<TProduct>(matchAction);
		}

		public IExpression<string> CharacterInRange(char rangeStart, char rangeEnd) {
			return new CharacterRangeExpression<string>(rangeStart, rangeEnd, null);
		}

		public IExpression<TProduct> CharacterInRange<TProduct>(char rangeStart, char rangeEnd, Func<IExpressionMatch<string>, TProduct> matchAction) {
			return new CharacterRangeExpression<TProduct>(rangeStart, rangeEnd, matchAction);
		}

		public IExpression<object[]> Sequence(params IExpression[] sequence) {
			return new SequenceExpression<object[]>(sequence, match => match.Product.ToArray());
		}

		public IExpression<TProduct> Sequence<TProduct>(IExpression[] sequence, Func<IExpressionMatch<SequenceProducts>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(sequence, matchAction);
		}

		public IExpression<TProduct> Sequence<T1, T2, TProduct>(IExpression<T1> e1, IExpression<T2> e2, Func<IExpressionMatch<SequenceProducts<T1, T2>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, Func<IExpressionMatch<SequenceProducts<T1, T2, T3>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, IExpression<T14> e14, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, IExpression<T14> e14, IExpression<T15> e15, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>())));
		}

		public IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, IExpression<T14> e14, IExpression<T15> e15, IExpression<T16> e16, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>, TProduct> matchAction) {
			return new SequenceExpression<TProduct>(
				new IExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>())));
		}
	}
}