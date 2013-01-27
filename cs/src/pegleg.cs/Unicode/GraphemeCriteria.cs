using pegleg.cs.Unicode.Criteria;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode {
	/// <summary>
	/// Factory class for the creation of <see cref="IGraphemeCriteria"/> instances.
	/// </summary>
	public static class GraphemeCriteria {

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any of the provided <paramref name="graphemes"/>.
		/// </summary>
		public static IGraphemeCriteria InValues(params char[] graphemes) {
			return InValues(graphemes.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any of the provided <paramref name="graphemes"/>.
		/// </summary>
		public static IGraphemeCriteria InValues(params int[] graphemes) {
			return InValues(graphemes.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any of the provided <paramref name="graphemes"/>.
		/// </summary>
		public static IGraphemeCriteria InValues(params string[] graphemes) {
			return InValues(graphemes.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any of the provided <paramref name="graphemes"/>.
		/// </summary>
		public static IGraphemeCriteria InValues(params IEnumerable<char>[] graphemes) {
			// As all chars are single code points we can optimize this case
			return SingleCodePoint(CodePointCriteria.InValues(graphemes));
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any of the provided <paramref name="graphemes"/>.
		/// </summary>
		public static IGraphemeCriteria InValues(params IEnumerable<int>[] graphemes) {
			return SingleCodePoint(CodePointCriteria.InValues(graphemes));
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any of the provided <paramref name="graphemes"/>.
		/// </summary>
		public static IGraphemeCriteria InValues(params IEnumerable<string>[] graphemes) {
			if(graphemes.Flatten().All(UnicodeUtils.IsSingleCodePoint)) {
				int length;
				return SingleCodePoint(CodePointCriteria.InValues(graphemes.Flatten().Select(g => UnicodeUtils.GetCodePoint(g, 0, out length))));
			} else {
				return new InValuesGraphemeCriterion(graphemes.Flatten());
			}
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by a grapheme in any of the provided <paramref name="categories"/>.
		/// </summary>
		public static IGraphemeCriteria InCategories(params UnicodeCategory[] categories) {
			return InCategories(categories.AsEnumerable());
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by a grapheme in any of the provided <paramref name="categories"/>.
		/// </summary>
		public static IGraphemeCriteria InCategories(params IEnumerable<UnicodeCategory>[] categories) {
			return new InCategoriesGraphemeCriterion(categories.Flatten());
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any grapheme composed of a single code point.
		/// </summary>
		public static IGraphemeCriteria SingleCodePoint() {
			return new SingleCodePointGraphemeCriterion();
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any grapheme composed of a single code point satisfying the provided <paramref name="criteria"/>.
		/// </summary>
		public static IGraphemeCriteria SingleCodePoint(ICodePointCriteria criteria) {
			return new SingleCodePointGraphemeCriterion(criteria);
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> satisfied by any grapheme which does not satisfy the provided <paramref name="criteria"/>.
		/// </summary>
		public static IGraphemeCriteria Not(IGraphemeCriteria criteria) {
			return new NegatedGraphemeCriteria(criteria);
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> which is satisfied if any of the provided <paramref name="criteria"/> are satisfied (logical OR).
		/// </summary>
		public static IGraphemeCriteria Or(params IGraphemeCriteria[] criteria) {
			return new DisjunctGraphemeCriteria(criteria);
		}

		/// <summary>
		/// Create an <see cref="IGraphemeCriteria"/> which is satisfied if all of the provided <paramref name="criteria"/> are satisfied (logical AND).
		/// </summary>
		public static IGraphemeCriteria And(params IGraphemeCriteria[] criteria) {
			return new ConjunctGraphemeCriteria(criteria);
		}
	}
}
