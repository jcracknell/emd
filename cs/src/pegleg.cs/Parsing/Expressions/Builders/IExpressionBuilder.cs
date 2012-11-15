using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions.Builders {
	public interface IExpressionBuilder {
		// # Design Decisions
		//
		// Avoid manual provision of type arguments, because that sucks.
		//
		// The action is always optional.
		//
		// Provide exactly one opportunity to capture match metadata. It is the user's
		// responsibility to record match metadata in the event that they want to store
		// it in in their AST.
		// 
		// Because the action is optional, every expression must provide match metadata.
		//
		// ...except for unary operators where there the match for the operand is the same
		// (e.g. optional expression, lookaround).
		//
		// If there is no user-provided action, the expression product type parameter is the
		// most detailed type which can be automatically inferred.


		IParsingExpression<T> Named<T>(string name, IParsingExpression<T> expression);

		// # Terminals

		IParsingExpression<Nil> Literal(string literal);
		IParsingExpression<Nil> Literal(char literal);
		IParsingExpression<Nil> Literal(string literal, StringComparison comparison);
		IParsingExpression<TProduct> Literal<TProduct>(string literal, Func<IMatch<string>, TProduct> matchAction);
		IParsingExpression<TProduct> Literal<TProduct>(char literal, Func<IMatch<string>, TProduct> matchAction);
		IParsingExpression<TProduct> Literal<TProduct>(string literal, StringComparison comparison, Func<IMatch<string>, TProduct> matchAction);

		IParsingExpression<Nil> CharacterInRange(char rangeStart, char rangeEnd);
		IParsingExpression<Nil> CharacterIn(params char[] chars);
		IParsingExpression<Nil> CharacterIn(params IEnumerable<char>[] chars);
		IParsingExpression<Nil> CharacterNotIn(params char[] chars);
		IParsingExpression<Nil> CharacterNotIn(params IEnumerable<char>[] chars);

		IParsingExpression<Nil> Regex(string regex);
		IParsingExpression<Nil> Regex(string regex, RegexOptions regexOptions);
		IParsingExpression<TProduct> Regex<TProduct>(string regex, Func<IMatch<Match>, TProduct> matchAction);
		IParsingExpression<TProduct> Regex<TProduct>(string regex, RegexOptions regexOptions, Func<IMatch<Match>, TProduct> matchAction);

		IParsingExpression<Nil> EndOfInput();

		IParsingExpression<Nil> Wildcard();

		// # Non-terminals

		IParsingExpression<T> Dynamic<T>(Func<IParsingExpression<T>> closure);
		IParsingExpression<Nil> Assert(Func<bool> predicate);

		IParsingExpression<T> Ahead<T>(IParsingExpression<T> expression);
		IParsingExpression<TProduct> Ahead<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction);

		IParsingExpression<IEnumerable<T>> AtLeast<T>(uint minOccurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> AtLeast<T, TProduct>(uint minOccurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction);
		IParsingExpression<IEnumerable<T>> AtMost<T>(uint maxOccurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> AtMost<T, TProduct>(uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction);
		IParsingExpression<IEnumerable<T>> Between<T>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> Between<T, TProduct>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction);
		IParsingExpression<IEnumerable<T>> Exactly<T>(uint occurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> Exactly<T, TProduct>(uint occurs, IParsingExpression<T> expression, Func<IMatch<IEnumerable<T>>, TProduct> matchAction);

		IParsingExpression<Nil> NotAhead<T>(IParsingExpression<T> expression);

		IParsingExpression<T> Optional<T>(IParsingExpression<T> expression);
		IParsingExpression<TProduct> Optional<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction);

		IParsingExpression<T> Reference<T>(Func<IParsingExpression<T>> reference);
		IParsingExpression<TProduct> Reference<T, TProduct>(Func<IParsingExpression<T>> reference, Func<IMatch<T>, TProduct> matchAction);

		IParsingExpression<Nil> ChoiceOrdered(params IParsingExpression<object>[] choices);
		IParsingExpression<Nil> ChoiceOrdered(IEnumerable<IParsingExpression<object>> choices);
		IParsingExpression<TChoice> ChoiceOrdered<TChoice>(params IParsingExpression<TChoice>[] choices);
		IParsingExpression<TChoice> ChoiceOrdered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices);
		IParsingExpression<TProduct> ChoiceOrdered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, IParsingExpression<object> e16, Func<IMatch<object>, TProduct> matchAction);

		IParsingExpression<Nil> ChoiceUnordered(params IParsingExpression<object>[] choices);
		IParsingExpression<Nil> ChoiceUnordered(IEnumerable<IParsingExpression<object>> choices);
		IParsingExpression<TChoice> ChoiceUnordered<TChoice>(params IParsingExpression<TChoice>[] choices);
		IParsingExpression<TChoice> ChoiceUnordered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices);
		IParsingExpression<TProduct> ChoiceUnordered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression<object> e1, IParsingExpression<object> e2, IParsingExpression<object> e3, IParsingExpression<object> e4, IParsingExpression<object> e5, IParsingExpression<object> e6, IParsingExpression<object> e7, IParsingExpression<object> e8, IParsingExpression<object> e9, IParsingExpression<object> e10, IParsingExpression<object> e11, IParsingExpression<object> e12, IParsingExpression<object> e13, IParsingExpression<object> e14, IParsingExpression<object> e15, IParsingExpression<object> e16, Func<IMatch<object>, TProduct> matchAction);

		IParsingExpression<Nil> Sequence(params IParsingExpression[] sequence);
		IParsingExpression<TProduct> Sequence<TProduct>(IParsingExpression[] sequence, Func<IMatch<SequenceProducts>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, Func<IMatch<SequenceProducts<T1, T2>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, Func<IMatch<SequenceProducts<T1, T2, T3>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, Func<IMatch<SequenceProducts<T1, T2, T3, T4>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, IParsingExpression<T15> e15, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>, TProduct> matchAction);
		IParsingExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TProduct>(IParsingExpression<T1> e1, IParsingExpression<T2> e2, IParsingExpression<T3> e3, IParsingExpression<T4> e4, IParsingExpression<T5> e5, IParsingExpression<T6> e6, IParsingExpression<T7> e7, IParsingExpression<T8> e8, IParsingExpression<T9> e9, IParsingExpression<T10> e10, IParsingExpression<T11> e11, IParsingExpression<T12> e12, IParsingExpression<T13> e13, IParsingExpression<T14> e14, IParsingExpression<T15> e15, IParsingExpression<T16> e16, Func<IMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>, TProduct> matchAction);
	}
}
