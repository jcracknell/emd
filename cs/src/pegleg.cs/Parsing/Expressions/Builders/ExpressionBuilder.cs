using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions.Builders {
	public class ExpressionBuilder : IExpressionBuilder {
		private IExpressionNamingConvention _namingConvention = new DefaultExpressionNamingConvention();

		public void SetExpressionNamingConvention(IExpressionNamingConvention namingConvention) {
			CodeContract.ArgumentIsNotNull(() => namingConvention, namingConvention);

			_namingConvention = namingConvention;
			RegexOptions = RegexOptions.Compiled | RegexOptions.Singleline;
		}

		public RegexOptions RegexOptions { get; set; }

		#region Helpers

		private TProduct DefaultMatchAction<TProduct>(IMatch<TProduct> match) {
			return match.Product;
		}

		private IMatch<TDest> UpcastExpressionMatch<TSource, TDest>(IMatch<TSource> expressionMatch, Func<TSource, TDest> cast) {
			return new UpcastedExpressionMatch<TSource, TDest>(expressionMatch, cast);
		}

		#endregion

		public IParsingExpression<T> Named<T>(string name, IParsingExpression<T> expression) {
			var conventionalName = _namingConvention.Apply(name, expression);
			return new NamedParsingExpression<T>(conventionalName, expression);
		}

		public IParsingExpression<string> Literal(string literal) {
			return new LiteralParsingExpression<string>(literal, null);
		}

		public IParsingExpression<TProduct> Literal<TProduct>(string literal, Func<IMatch<string>, TProduct> matchAction) {
			return new LiteralParsingExpression<TProduct>(literal, matchAction);
		}

		public IParsingExpression<string> Literal(char literal) {
			return Literal(literal.ToString());
		}

		public IParsingExpression<TProduct> Literal<TProduct>(char literal, Func<IMatch<string>, TProduct> matchAction) {
			return Literal(literal.ToString(), matchAction);
		}

		#region Regex

		public IParsingExpression<Nil> Regex(Regex regex) {
			return new NonCapturingRegexParsingExpression(regex);
		}

		public IParsingExpression<Nil> Regex(string regex) {
			return Regex(regex, RegexOptions);
		}

		public IParsingExpression<Nil> Regex(string regex, RegexOptions regexOptions) {
			return Regex(new Regex("^" + regex, regexOptions));
		}

		public IParsingExpression<TProduct> Regex<TProduct>(string regex, RegexOptions regexOptions, Func<IMatch<Match>, TProduct> matchAction) {
			return Regex(new Regex("^" + regex, regexOptions), matchAction);
		}

		public IParsingExpression<TProduct> Regex<TProduct>(Regex regex, Func<IMatch<Match>, TProduct> matchAction) {
			return new CapturingRegexParsingExpression<TProduct>(regex, matchAction);
		}

		public IParsingExpression<TProduct> Regex<TProduct>(string regex, Func<IMatch<Match>, TProduct> matchAction) {
			return Regex(regex, RegexOptions, matchAction);
		}

		#endregion

		public IParsingExpression<Nil> EndOfInput() {
			return new EndOfInputParsingExpression<Nil>(null);
		}

		public IParsingExpression<TProduct> EndOfInput<TProduct>(Func<IMatch<Nil>, TProduct> matchAction) {
			return new EndOfInputParsingExpression<TProduct>(matchAction);
		}

		public IParsingExpression<T> Dynamic<T>(Func<IParsingExpression<T>> closure) {
			return new DynamicParsingExpression<T>(closure);
		}

		public IParsingExpression<Nil> Assert(Func<bool> predicate) {
			return new PredicateParsingExpression(predicate);
		}

		public IParsingExpression<T> Ahead<T>(IParsingExpression<T> expression) {
			return new AheadExpression<T>(expression, null);
		}

		public IParsingExpression<TProduct> Ahead<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction) {
			return new AheadExpression<TProduct>(expression,
				match => matchAction(UpcastExpressionMatch(match, product => (T)product)));
		}

		public IParsingExpression<T[]> AtLeast<T>(uint n, IParsingExpression<T> expression) {
			return new RepetitionParsingExpression<T, T[]>(n, RepetitionParsingExpression.UNBOUNDED, expression, null);
		}

		public IParsingExpression<TProduct> AtLeast<T, TProduct>(uint n, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction) {
			return new RepetitionParsingExpression<T, TProduct>(n, 0, expression, matchAction);
		}

		public IParsingExpression<T[]> AtMost<T>(uint maxOccurs, IParsingExpression<T> expression) {
			return new RepetitionParsingExpression<T, T[]>(RepetitionParsingExpression.UNBOUNDED, maxOccurs, expression, null);
		}

		public IParsingExpression<TProduct> AtMost<T, TProduct>(uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction) {
			return new RepetitionParsingExpression<T, TProduct>(0, maxOccurs, expression, matchAction);
		}

		public IParsingExpression<T[]> Between<T>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression) {
			return new RepetitionParsingExpression<T, T[]>(minOccurs, maxOccurs, expression, null);
		}

		public IParsingExpression<TProduct> Between<T, TProduct>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction) {
			return new RepetitionParsingExpression<T, TProduct>(minOccurs, maxOccurs, expression, matchAction);
		}

		public IParsingExpression<T[]> Exactly<T>(uint occurs, IParsingExpression<T> expression) {
			return new RepetitionParsingExpression<T, T[]>(occurs, occurs, expression, null);
		}

		public IParsingExpression<TProduct> Exactly<T, TProduct>(uint occurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction) {
			return new RepetitionParsingExpression<T, TProduct>(occurs, occurs, expression, matchAction);
		}

		public IParsingExpression<Nil> NotAhead<T>(IParsingExpression<T> expression) {
			return new NotAheadParsingExpression<Nil>(expression, null);
		}

		public IParsingExpression<TProduct> NotAhead<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<Nil>, TProduct> matchAction) {
			return new NotAheadParsingExpression<TProduct>(expression, matchAction);
		}

		public IParsingExpression<T> Optional<T>(IParsingExpression<T> expression) {
			return new OptionalParsingExpression<T>(expression, null, null);
		}

		public IParsingExpression<TProduct> Optional<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction) {
			return new OptionalParsingExpression<TProduct>(expression, match => matchAction(UpcastExpressionMatch(match, product => (T)product)), null);
		}

		public IParsingExpression<TProduct> Optional<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction) {
			return new OptionalParsingExpression<TProduct>(expression, match => matchAction(UpcastExpressionMatch(match, product => (T)product)), noMatchAction);
		}

		public IParsingExpression<T> Reference<T>(Func<IParsingExpression<T>> reference) {
			return new NonCapturingReferenceParsingExpression<T>(reference);
		}

		public IParsingExpression<TProduct> Reference<T, TProduct>(Func<IParsingExpression<T>> reference, Func<IMatch<T>, TProduct> matchAction) {
			return new CapturingReferenceParsingExpression<T, TProduct>(reference, matchAction);
		}

		public IParsingExpression<Nil> Wildcard() {
			return new NonCapturingWildcardParsingExpression();
		}

		public IParsingExpression<TProduct> Wildcard<TProduct>(Func<IMatch<char>, TProduct> matchAction) {
			return new CapturingWildcardParsingExpression<TProduct>(matchAction);
		}

		public IParsingExpression<Nil> CharacterInRange(char rangeStart, char rangeEnd) {
			return new NonCapturingCharacterRangeParsingExpression(rangeStart, rangeEnd);
		}

		public IParsingExpression<Nil> CharacterIn(IEnumerable<char> chars) {
			var ranges = new List<IParsingExpression<Nil>>();
			var enumerator = chars.OrderBy(c => c).Distinct().GetEnumerator();
			if(enumerator.MoveNext()) {
				var rangeStart = enumerator.Current;
				var rangeEnd = rangeStart;

				while(enumerator.MoveNext()) {
					if(enumerator.Current == rangeEnd + 1) {
						rangeEnd = enumerator.Current;
					} else {
						ranges.Add(CharacterInRange(rangeStart, rangeEnd));
						rangeStart = rangeEnd = enumerator.Current;
					}
				}

				if(rangeStart == rangeEnd)
					ranges.Add(CharacterInRange(rangeStart, rangeEnd));
			}

			if(1 == ranges.Count) return ranges[0];

			return ChoiceUnordered(ranges);
		}

		public IParsingExpression<Nil> CharacterNotIn(IEnumerable<char> chars) {
			var ranges = new List<IParsingExpression<Nil>>();
			var enumerator = chars.OrderBy(c => c).Distinct().GetEnumerator();
			if(!enumerator.MoveNext()) {
				return CharacterInRange(char.MinValue, char.MaxValue);
			} else {
				if(char.MinValue != enumerator.Current)
					ranges.Add(CharacterInRange(char.MinValue, (char)(enumerator.Current - 1)));

				var rangeStart = enumerator.Current + 1;
				while(enumerator.MoveNext()) {
					if(enumerator.Current != rangeStart)
						ranges.Add(CharacterInRange((char)rangeStart, (char)(enumerator.Current - 1)));

					if(char.MaxValue != enumerator.Current)
						rangeStart = enumerator.Current + 1;
				}

				if(char.MaxValue != rangeStart)
					ranges.Add(CharacterInRange((char)rangeStart, char.MaxValue));
			}
			
			if(1 == ranges.Count) return ranges[0];

			return ChoiceUnordered(ranges);
		}

		#region ChoiceOrdered

		public IParsingExpression<Nil> ChoiceOrdered(params IParsingExpression[] choices) {
			return new NonCapturingOrderedChoiceParsingExpression(choices);
		}

		public IParsingExpression<Nil> ChoiceOrdered(IEnumerable<IParsingExpression> choices) {
			return ChoiceOrdered(choices.ToArray());
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IEnumerable<IParsingExpression> choices, Func<IMatch<object>, TProduct> matchAction) {
			return new CapturingOrderedChoiceParsingExpression<object, TProduct>(choices.ToArray(), matchAction);
		}

		public IParsingExpression<TChoice> ChoiceOrdered<TChoice>(params IParsingExpression<TChoice>[] choices) {
			return new NonCapturingOrderedChoiceParsingExpression<TChoice>(choices);
		}

		public IParsingExpression<TChoice> ChoiceOrdered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices) {
			return ChoiceOrdered(choices.ToArray());
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction) {
			return new CapturingOrderedChoiceParsingExpression<TChoice, TProduct>(choices.ToArray(), matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, IParsingExpression e16, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 }, matchAction);
		}

		#endregion

		#region ChoiceUnordered

		public IParsingExpression<Nil> ChoiceUnordered(params IParsingExpression[] choices) {
			return new NonCapturingUnorderedChoiceParsingExpression(choices);
		}

		public IParsingExpression<Nil> ChoiceUnordered(IEnumerable<IParsingExpression> choices) {
			return ChoiceUnordered(choices.ToArray());
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IEnumerable<IParsingExpression> choices, Func<IMatch<object>, TProduct> matchAction) {
			return new CapturingUnorderedChoiceParsingExpression<object, TProduct>(choices.ToArray(), matchAction);
		}

		public IParsingExpression<TChoice> ChoiceUnordered<TChoice>(params IParsingExpression<TChoice>[] choices) {
			return new NonCapturingUnorderedChoiceParsingExpression<TChoice>(choices);
		}

		public IParsingExpression<TChoice> ChoiceUnordered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices) {
			return ChoiceUnordered(choices.ToArray());
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction) {
			return new CapturingUnorderedChoiceParsingExpression<TChoice, TProduct>(choices.ToArray(), matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 }, matchAction);
		}

		public IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, IParsingExpression e16, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 }, matchAction);
		}

		#endregion

		#region Sequence

		public IParsingExpression<Nil> Sequence(params IParsingExpression[] sequence) {
			return new NonCapturingSequenceParsingExpression(sequence);
		}

		public IParsingExpression<TProduct> Sequence<TProduct>(IParsingExpression[] sequence, Func<IMatch<SequenceProducts>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(sequence, matchAction);
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, Func<IMatch<SequenceProducts<T1, T2>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, Func<IMatch<SequenceProducts<T1, T2, T3>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, Func<IMatch<SequenceProducts<T1, T2, T3, T4>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, IParsingExpression<T15> e15, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>())));
		}

		public IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, IParsingExpression<T15> e15, IParsingExpression<T16> e16, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 },
				match => matchAction(UpcastExpressionMatch(match, p => p.Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>())));
		}

		#endregion



	}
}