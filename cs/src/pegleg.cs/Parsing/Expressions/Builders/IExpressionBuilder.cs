﻿using System;
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

		IExpression<T> Named<T>(string name, IExpression<T> expression);

		// # Terminals

		IExpression<string> Literal(string literal);
		IExpression<string> Literal(char literal);
		IExpression<TProduct> Literal<TProduct>(string literal, Func<IExpressionMatch<string>, TProduct> matchAction);
		IExpression<TProduct> Literal<TProduct>(char literal, Func<IExpressionMatch<string>, TProduct> matchAction);

		IExpression<string> CharacterInRange(char rangeStart, char rangeEnd);
		IExpression<TProduct> CharacterInRange<TProduct>(char rangeStart, char rangeEnd, Func<IExpressionMatch<string>, TProduct> matchAction);

		IExpression<Match> Regex(Regex regex);
		IExpression<Match> Regex(string regex);
		IExpression<TProduct> Regex<TProduct>(Regex regex, Func<IExpressionMatch<Match>, TProduct> matchAction);
		IExpression<TProduct> Regex<TProduct>(string regex, Func<IExpressionMatch<Match>, TProduct> matchAction);

		IExpression<Nil> EndOfInput();
		IExpression<TProduct> EndOfInput<TProduct>(Func<IExpressionMatch<Nil>, TProduct> matchAction);

		IExpression<string> Wildcard();
		IExpression<TProduct> Wildcard<TProduct>(Func<IExpressionMatch<string>, TProduct> matchAction);

		IExpression<Nil> Nothing();
		IExpression<TProduct> Nothing<TProduct>(Func<IExpressionMatch<Nil>, TProduct> matchAction);

		// # Non-terminals

		IExpression<T> Dynamic<T>(Func<IExpression<T>> closure);
		IExpression<Nil> Assert(Func<bool> predicate);

		IExpression<T> Ahead<T>(IExpression<T> expression);
		IExpression<TProduct> Ahead<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<T>, TProduct> matchAction);

		IExpression<T[]> AtLeast<T>(uint minOccurs, IExpression<T> expression);
		IExpression<TProduct> AtLeast<T, TProduct>(uint minOccurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction);
		IExpression<T[]> AtMost<T>(uint maxOccurs, IExpression<T> expression);
		IExpression<TProduct> AtMost<T, TProduct>(uint maxOccurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction);
		IExpression<T[]> Between<T>(uint minOccurs, uint maxOccurs, IExpression<T> expression);
		IExpression<TProduct> Between<T, TProduct>(uint minOccurs, uint maxOccurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction);
		IExpression<T[]> Exactly<T>(uint occurs, IExpression<T> expression);
		IExpression<TProduct> Exactly<T, TProduct>(uint occurs, IExpression<T> expression, Func<IExpressionMatch<T[]>, TProduct> matchAction);

		IExpression<object> ChoiceOrdered(params IExpression[] choices);
		IExpression<TProduct> ChoiceOrdered<TProduct>(IExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction);
		IExpression<TChoice> ChoiceOrdered<TChoice>(params IExpression<TChoice>[] choices);
		IExpression<TProduct> ChoiceOrdered<TChoice, TProduct>(IExpression<TChoice>[] choices, Func<IExpressionMatch<TChoice>, TProduct> matchAction);

		IExpression<object> ChoiceUnordered(params IExpression[] choices);
		IExpression<TProduct> ChoiceUnordered<TProduct>(IExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction);
		IExpression<TChoice> ChoiceUnordered<TChoice>(params IExpression<TChoice>[] choices);
		IExpression<TProduct> ChoiceUnordered<TChoice, TProduct>(IExpression<TChoice>[] choices, Func<IExpressionMatch<TChoice>, TProduct> matchAction);

		IExpression<Nil> NotAhead<T>(IExpression<T> expression);
		IExpression<TProduct> NotAhead<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<Nil>, TProduct> matchAction);

		IExpression<T> Optional<T>(IExpression<T> expression);
		IExpression<TProduct> Optional<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<T>, TProduct> matchAction);
		IExpression<TProduct> Optional<T, TProduct>(IExpression<T> expression, Func<IExpressionMatch<T>, TProduct> matchAction, Func<IExpressionMatch<Nil>, TProduct> noMatchAction);

		IExpression<T> Reference<T>(Func<IExpression<T>> reference);
		IExpression<TProduct> Reference<T, TProduct>(Func<IExpression<T>> reference, Func<IExpressionMatch<T>, TProduct> matchAction);


		IExpression<object[]> Sequence(params IExpression[] sequence);
		IExpression<TProduct> Sequence<TProduct>(IExpression[] sequence, Func<IExpressionMatch<SequenceProducts>, TProduct> matchAction);

		// These are convenient because you do not have to name all of the expressions
		IExpression<TProduct> Sequence<T1, T2, TProduct>(IExpression<T1> e1, IExpression<T2> e2, Func<IExpressionMatch<SequenceProducts<T1, T2>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, Func<IExpressionMatch<SequenceProducts<T1, T2, T3>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, IExpression<T14> e14, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, IExpression<T14> e14, IExpression<T15> e15, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>, TProduct> matchAction);
		IExpression<TProduct> Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TProduct>(IExpression<T1> e1, IExpression<T2> e2, IExpression<T3> e3, IExpression<T4> e4, IExpression<T5> e5, IExpression<T6> e6, IExpression<T7> e7, IExpression<T8> e8, IExpression<T9> e9, IExpression<T10> e10, IExpression<T11> e11, IExpression<T12> e12, IExpression<T13> e13, IExpression<T14> e14, IExpression<T15> e15, IExpression<T16> e16, Func<IExpressionMatch<SequenceProducts<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>>, TProduct> matchAction);
	}
}
