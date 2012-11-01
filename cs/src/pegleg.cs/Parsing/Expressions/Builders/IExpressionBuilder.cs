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

		IParsingExpression<string> Literal(string literal);
		IParsingExpression<string> Literal(char literal);
		IParsingExpression<TProduct> Literal<TProduct>(string literal, Func<IMatch<string>, TProduct> matchAction);
		IParsingExpression<TProduct> Literal<TProduct>(char literal, Func<IMatch<string>, TProduct> matchAction);

		IParsingExpression<Nil> CharacterInRange(char rangeStart, char rangeEnd);

		IParsingExpression<Match> Regex(Regex regex);
		IParsingExpression<Match> Regex(string regex);
		IParsingExpression<TProduct> Regex<TProduct>(Regex regex, Func<IMatch<Match>, TProduct> matchAction);
		IParsingExpression<TProduct> Regex<TProduct>(string regex, Func<IMatch<Match>, TProduct> matchAction);

		IParsingExpression<Nil> EndOfInput();
		IParsingExpression<TProduct> EndOfInput<TProduct>(Func<IMatch<Nil>, TProduct> matchAction);

		IParsingExpression<Nil> Wildcard();
		IParsingExpression<TProduct> Wildcard<TProduct>(Func<IMatch<char>, TProduct> matchAction);

		// # Non-terminals

		IParsingExpression<T> Dynamic<T>(Func<IParsingExpression<T>> closure);
		IParsingExpression<Nil> Assert(Func<bool> predicate);

		IParsingExpression<T> Ahead<T>(IParsingExpression<T> expression);
		IParsingExpression<TProduct> Ahead<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction);

		IParsingExpression<T[]> AtLeast<T>(uint minOccurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> AtLeast<T, TProduct>(uint minOccurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction);
		IParsingExpression<T[]> AtMost<T>(uint maxOccurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> AtMost<T, TProduct>(uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction);
		IParsingExpression<T[]> Between<T>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> Between<T, TProduct>(uint minOccurs, uint maxOccurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction);
		IParsingExpression<T[]> Exactly<T>(uint occurs, IParsingExpression<T> expression);
		IParsingExpression<TProduct> Exactly<T, TProduct>(uint occurs, IParsingExpression<T> expression, Func<IMatch<T[]>, TProduct> matchAction);

		IParsingExpression<Nil> NotAhead<T>(IParsingExpression<T> expression);
		IParsingExpression<TProduct> NotAhead<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<Nil>, TProduct> matchAction);

		IParsingExpression<T> Optional<T>(IParsingExpression<T> expression);
		IParsingExpression<TProduct> Optional<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction);
		IParsingExpression<TProduct> Optional<T, TProduct>(IParsingExpression<T> expression, Func<IMatch<T>, TProduct> matchAction, Func<IMatch<Nil>, TProduct> noMatchAction);

		IParsingExpression<T> Reference<T>(Func<IParsingExpression<T>> reference);
		IParsingExpression<TProduct> Reference<T, TProduct>(Func<IParsingExpression<T>> reference, Func<IMatch<T>, TProduct> matchAction);

		IParsingExpression<object> ChoiceOrdered(params IParsingExpression[] choices);
		IParsingExpression<object> ChoiceOrdered(IEnumerable<IParsingExpression> choices);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IEnumerable<IParsingExpression> choices, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TChoice> ChoiceOrdered<TChoice>(params IParsingExpression<TChoice>[] choices);
		IParsingExpression<TChoice> ChoiceOrdered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices);
		IParsingExpression<TProduct> ChoiceOrdered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceOrdered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, IParsingExpression e16, Func<IMatch<object>, TProduct> matchAction);

		IParsingExpression<object> ChoiceUnordered(params IParsingExpression[] choices);
		IParsingExpression<object> ChoiceUnordered(IEnumerable<IParsingExpression> choices);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IEnumerable<IParsingExpression> choices, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TChoice> ChoiceUnordered<TChoice>(params IParsingExpression<TChoice>[] choices);
		IParsingExpression<TChoice> ChoiceUnordered<TChoice>(IEnumerable<IParsingExpression<TChoice>> choices);
		IParsingExpression<TProduct> ChoiceUnordered<TChoice, TProduct>(IEnumerable<IParsingExpression<TChoice>> choices, Func<IMatch<TChoice>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, Func<IMatch<object>, TProduct> matchAction);
		IParsingExpression<TProduct> ChoiceUnordered<TProduct>(IParsingExpression e1, IParsingExpression e2, IParsingExpression e3, IParsingExpression e4, IParsingExpression e5, IParsingExpression e6, IParsingExpression e7, IParsingExpression e8, IParsingExpression e9, IParsingExpression e10, IParsingExpression e11, IParsingExpression e12, IParsingExpression e13, IParsingExpression e14, IParsingExpression e15, IParsingExpression e16, Func<IMatch<object>, TProduct> matchAction);

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
