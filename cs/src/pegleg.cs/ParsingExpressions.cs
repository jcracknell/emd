using pegleg.cs.Parsing;
using pegleg.cs.Parsing.Expressions;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs {
	public class ParsingExpressions {
		public const RegexOptions DefaultRegexOptions = RegexOptions.Compiled | RegexOptions.Singleline;

		private static IMatch<TProduct> ShadowMatchProduct<TProduct>(IMatch match, TProduct product) {
			return new ProductShadowingMatch<TProduct>(match, product);
		}

		public static IParsingExpression<T> Named<T>(string name, IParsingExpression<T> expression) {
			return new NamedParsingExpression<T>(name, expression);
		}

		public static IParsingExpression<T> Named<T>(Expression<Func<IParsingExpression<T>>> name, IParsingExpression<T> expression) {
			var member = ReflectionUtils.GetMemberFrom(name);
			return Named(member.Name, expression);
		}

		#region Literal

		public static IParsingExpression<Nil> Literal(string literal) {
			return new NonCapturingLiteralParsingExpression(literal);
		}

		public static IParsingExpression<TProduct> Literal<TProduct>(string literal, Func<IMatch<string>, TProduct> matchAction) {
			return new CapturingLiteralParsingExpression<TProduct>(literal, matchAction);
		}

		public static IParsingExpression<Nil> Literal(char literal) {
			return Literal(literal.ToString());
		}

		public static IParsingExpression<TProduct> Literal<TProduct>(char literal, Func<IMatch<string>, TProduct> matchAction) {
			return Literal(literal.ToString(), matchAction);
		}

		#endregion
		
		#region LiteralIn

		public static IParsingExpression<Nil> LiteralIn(params IEnumerable<string>[] literals) {
			return new NonCapturingLiteralSetParsingExpression(literals.Flatten());
		}

		public static IParsingExpression<Nil> LiteralIn(params string[] literals) {
			return new NonCapturingLiteralSetParsingExpression(literals);
		}

		public static IParsingExpression<TProduct> LiteralIn<TProduct>(IEnumerable<string> literals, Func<IMatch<string>, TProduct> matchAction) {
			return new CapturingLiteralSetParsingExpression<TProduct>(literals, matchAction);
		}

		#endregion

		public static IParsingExpression<Nil> CharacterInRange(char rangeStart, char rangeEnd) {
			return CharacterIn(CharUtils.Range(rangeStart, rangeEnd));
		}

		public static IParsingExpression<Nil> CharacterIn(params char[] chars) {
			return CharacterIn(chars.AsEnumerable());
		}

		public static IParsingExpression<Nil> CharacterIn(params IEnumerable<char>[] chars) {
			return Character(UnicodeCriteria.NoCharacter.Except(chars));
		}

		public static IParsingExpression<Nil> CharacterNotIn(params char[] chars) {
			return Character(UnicodeCriteria.AnyCharacter.Except(chars));
		}

		public static IParsingExpression<Nil> CharacterNotIn(params IEnumerable<char>[] chars) {
			return Character(UnicodeCriteria.AnyCharacter.Except(chars));
		}

		public static IParsingExpression<Nil> Character(UnicodeCriteria criteria) {
			return new CharacterParsingExpression(criteria);
		}

		#region Regex

		private static string FixRegexString(string regex) {
			// This is here because the .NET regex class does not provide a method for matching
			// a regex at a specific position; you have to use this flag instead per:
			// http://msdn.microsoft.com/en-us/library/3583dcyh%28v=VS.100%29.aspx
			return @"\G" + regex;
		}

		public static IParsingExpression<Nil> Regex(string regex) {
			return Regex(regex, DefaultRegexOptions);
		}

		public static IParsingExpression<Nil> Regex(string regex, RegexOptions regexOptions) {
			return new NonCapturingRegexParsingExpression(new Regex(FixRegexString(regex), regexOptions));
		}

		public static IParsingExpression<TProduct> Regex<TProduct>(string regex, Func<IMatch<Match>, TProduct> matchAction) {
			return Regex(regex, DefaultRegexOptions, matchAction);
		}

		public static IParsingExpression<TProduct> Regex<TProduct>(string regex, RegexOptions regexOptions, Func<IMatch<Match>, TProduct> matchAction) {
			return new CapturingRegexParsingExpression<TProduct>(new Regex(FixRegexString(regex), regexOptions), matchAction);
		}

		#endregion

		public static IParsingExpression<Nil> EndOfInput() {
			return EndOfInputParsingExpression.Instance;
		}

		public static IParsingExpression<T> Reference<T>(Func<IParsingExpression<T>> reference) {
			return new NonCapturingReferenceParsingExpression<T>(reference);
		}

		public static IParsingExpression<TProduct> Reference<T, TProduct>(Func<IParsingExpression<T>> reference, Func<IMatch<T>, TProduct> matchAction) {
			return new CapturingReferenceParsingExpression<T, TProduct>(reference, matchAction);
		}

		public static IParsingExpression<T> Optional<T>(IParsingExpression<T> expression) {
			return new NonCapturingOptionalParsingExpression<T>(expression);
		}

		public static IParsingExpression<TProduct> Optional<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction) {
			return new CapturingOptionalParsingExpression<T, TProduct>(expression, matchAction, noMatchAction);
		}

		public static IParsingExpression<T> Dynamic<T>(Func<IParsingExpression<T>> closure) {
			return new DynamicParsingExpression<T>(closure);
		}

		public static IParsingExpression<Nil> Assert(Func<bool> predicate) {
			return new PredicateParsingExpression(predicate);
		}

		public static IParsingExpression<T> Ahead<T>(IParsingExpression<T> expression) {
			return new NonCapturingAheadParsingExpression<T>(expression);
		}

		public static IParsingExpression<TProduct> Ahead<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction) {
			return new CapturingAheadParsingExpression<T, TProduct>(expression, matchAction);
		}

		public static IParsingExpression<Nil> NotAhead<T>(IParsingExpression<T> expression) {
			return new NotAheadParsingExpression(expression);
		}

		#region Repetition

		public static IParsingExpression<IEnumerable<T>> AtLeast<T>(uint minOccurs, IParsingExpression<T> expression) {
			return new NonCapturingRepetitionParsingExpression<T>(minOccurs, 0, expression);
		}

		public static IParsingExpression<TProduct> AtLeast<T, TProduct>(uint minOccurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction) {
			return new CapturingRepetitionParsingExpression<T, TProduct>(minOccurs, 0, expression, matchAction);
		}

		public static IParsingExpression<IEnumerable<T>> AtMost<T>(uint maxOccurs, IParsingExpression<T> expression) {
			return new NonCapturingRepetitionParsingExpression<T>(0, maxOccurs, expression);
		}

		public static IParsingExpression<TProduct> AtMost<T, TProduct>(uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction) {
			return new CapturingRepetitionParsingExpression<T, TProduct>(0, maxOccurs, expression, matchAction);
		}

		public static IParsingExpression<IEnumerable<T>> Between<T>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression) {
			return new NonCapturingRepetitionParsingExpression<T>(minOccurs, maxOccurs, expression);
		}

		public static IParsingExpression<TProduct> Between<T, TProduct>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction) {
			return new CapturingRepetitionParsingExpression<T, TProduct>(minOccurs, maxOccurs, expression, matchAction);
		}

		public static IParsingExpression<IEnumerable<T>> Exactly<T>(uint occurs, IParsingExpression<T> expression) {
			return new NonCapturingRepetitionParsingExpression<T>(occurs, occurs, expression);
		}

		public static IParsingExpression<TProduct> Exactly<T, TProduct>(uint occurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction) {
			return new CapturingRepetitionParsingExpression<T, TProduct>(occurs, occurs, expression, matchAction);
		}

		#endregion

		#region ChoiceOrdered

		public static IParsingExpression<Nil> ChoiceOrdered(params IParsingExpression<object>[] choices) {
			return new NonCapturingOrderedChoiceParsingExpression(choices);
		}

		public static IParsingExpression<Nil> ChoiceOrdered(IEnumerable<IParsingExpression<object>> choices) {
			return ChoiceOrdered(choices.ToArray());
		}

		public static IParsingExpression<TChoice> ChoiceOrdered<TChoice>(params IParsingExpression<TChoice>[] choices) {
			return new NonCapturingOrderedChoiceParsingExpression<TChoice>(choices);
		}

		public static IParsingExpression<TChoice> ChoiceOrdered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices) {
			return ChoiceOrdered(choices.ToArray());
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction) {
			return new CapturingOrderedChoiceParsingExpression<TChoice, TProduct>(choices.ToArray(), matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, IParsingExpression<object> e16, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceOrdered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 }, matchAction);
		}

		#endregion

		#region ChoiceUnordered

		public static IParsingExpression<Nil> ChoiceUnordered(params IParsingExpression<object>[] choices) {
			return new NonCapturingUnorderedChoiceParsingExpression(choices);
		}

		public static IParsingExpression<Nil> ChoiceUnordered(IEnumerable<IParsingExpression<object>> choices) {
			return ChoiceUnordered(choices.ToArray());
		}

		public static IParsingExpression<TChoice> ChoiceUnordered<TChoice>(params IParsingExpression<TChoice>[] choices) {
			return new NonCapturingUnorderedChoiceParsingExpression<TChoice>(choices);
		}

		public static IParsingExpression<TChoice> ChoiceUnordered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices) {
			return ChoiceUnordered(choices.ToArray());
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction) {
			return new CapturingUnorderedChoiceParsingExpression<TChoice, TProduct>(choices.ToArray(), matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 }, matchAction);
		}

		public static IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, IParsingExpression<object> e16, Func<IMatch<object>, TProduct> matchAction) {
			return ChoiceUnordered(new IParsingExpression<object>[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 }, matchAction);
		}

		#endregion

		#region Sequence

		public static IParsingExpression<Nil> Sequence(params IParsingExpression[] sequence) {
			return new NonCapturingSequenceParsingExpression(sequence);
		}

		public static IParsingExpression<TProduct> Sequence<TProduct>(IParsingExpression[] sequence, Func<IMatch<SequenceProducts>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(sequence, matchAction);
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, Func<IMatch<SequenceProducts<T1, T2>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, Func<IMatch<SequenceProducts<T1, T2, T3>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, Func<IMatch<SequenceProducts<T1, T2, T3, T4>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, IParsingExpression<T15> e15, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>())));
		}

		public static IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, IParsingExpression<T15> e15, IParsingExpression<T16> e16, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>, TProduct> matchAction) {
			return new CapturingSequenceParsingExpression<TProduct>(
				new IParsingExpression[] { e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16 },
				match => matchAction(ShadowMatchProduct(match, match.Product.Annotate<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>())));
		}

		#endregion
	}
}