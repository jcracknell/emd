﻿using pegleg.cs.Unicode.Criteria;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode {
	/// <summary>
	/// Factory class for the creation of <see cref="ICodePointCriteria"/> instances.
	/// </summary>
	public static class CodePointCriteria {
		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by any of the provided <paramref name="codePoints"/>.
		/// </summary>
		public static ICodePointCriteria In(params char[] codePoints) {
			return In(codePoints.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by any of the provided <paramref name="codePoints"/>.
		/// </summary>
		public static ICodePointCriteria In(params int[] codePoints) {
			return In(codePoints.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by any of the provided <paramref name="codePoints"/>.
		/// </summary>
		public static ICodePointCriteria In(params IEnumerable<int>[] codePoints) {
			return new InValuesCodePointCriterion(codePoints.Flatten());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by any of the provided <paramref name="codePoints"/>.
		/// </summary>
		public static ICodePointCriteria In(params IEnumerable<char>[] codePoints) {
			return new InValuesCodePointCriterion(codePoints.Flatten().Select(c => (int)c));
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by any code point in the provided <paramref name="categories"/>.
		/// </summary>
		public static ICodePointCriteria In(params UnicodeCategory[] categories) {
			return In(categories.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by any code point in the provided <paramref name="categories"/>.
		/// </summary>
		public static ICodePointCriteria In(params IEnumerable<UnicodeCategory>[] categories) {
			return new InCategoriesCodePointCriterion(categories.Flatten());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> satisfied by all code points in the range from
		/// <paramref name="startCodePoint"/> through <paramref name="endCodePoint"/>, inclusive.
		/// </summary>
		public static ICodePointCriteria InRange(int startCodePoint, int endCodePoint) {
			if(startCodePoint < endCodePoint) {
				return new InRangeCodePointCriterion(startCodePoint, endCodePoint);
			} else {
				return new InRangeCodePointCriterion(endCodePoint, startCodePoint);
			}
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> which is satisfied whenever the provided <paramref name="criteria"/> are unsatisfied.
		/// </summary>
		public static ICodePointCriteria Not(ICodePointCriteria criteria) {
			return new NegatedCodePointCriteria(criteria);
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> which is satisfied if any of the provided <paramref name="criteria"/> are satisfied (logical OR).
		/// </summary>
		public static ICodePointCriteria Or(params ICodePointCriteria[] criteria) {
			return Or(criteria.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> which is satisfied if any of the provided <paramref name="criteria"/> are satisfied (logical OR).
		/// </summary>
		public static ICodePointCriteria Or(params IEnumerable<ICodePointCriteria>[] criteria) {
			return new DisjunctCodePointCriteria(criteria.Flatten().ToArray());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> which is satisfied if all of the provided <paramref name="criteria"/> are satisfied (logical AND).
		/// </summary>
		public static ICodePointCriteria And(params ICodePointCriteria[] criteria) {
			return And(criteria.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="ICodePointCriteria"/> which is satisfied if all of the provided <paramref name="criteria"/> are satisfied (logical AND).
		/// </summary>
		public static ICodePointCriteria And(params IEnumerable<ICodePointCriteria>[] criteria) {
			return new ConjunctCodePointCriteria(criteria.Flatten().ToArray());
		}
	}
}
