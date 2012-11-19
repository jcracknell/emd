using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode {
	public static class UnicodeParsingExpressions {
		public static IParsingExpression<Nil> UnicodeCharacterIn(params IEnumerable<UnicodeCodePoint>[] categories) {
			var normalCodePointDefinitions = categories.Flatten().Where(cp => !cp.IsSurrogatePair);
			var surrogatePairCodePointDefinitions = categories.Flatten().Where(cp => cp.IsSurrogatePair);

			var normalCodePoint =
				ParsingExpression.CharacterIn(normalCodePointDefinitions.Select(cp => cp.FirstCodeUnit));

			if(!surrogatePairCodePointDefinitions.Any())
				return normalCodePoint;

			// Define a rule which can be used to quickly match a valid first code unit for
			// a surrogate pair amongst the provided code points
			var surrogatePairLeadUnit =
				ParsingExpression.CharacterIn(
					categories.Flatten()
					.Where(cp => cp.IsSurrogatePair)
					.Select(cp => cp.FirstCodeUnit));

			var surrogatePair =
				ParsingExpression.ChoiceUnordered(
					categories.Flatten()
					.Where(cp => cp.IsSurrogatePair)
					.GroupBy(cp => cp.FirstCodeUnit)
					.Select(g =>
						ParsingExpression.Sequence(
							ParsingExpression.CharacterIn(g.Key),
							ParsingExpression.CharacterIn(g.Select(cp => cp.SecondCodeUnit))
						)
					)
				);

			return ParsingExpression.ChoiceOrdered(
				normalCodePoint,
				ParsingExpression.Sequence(
					ParsingExpression.Ahead(surrogatePairLeadUnit),
					surrogatePair));
		}

		public static IParsingExpression<Nil> UnicodeCharacterNotIn(params IEnumerable<char>[] chars) {
			bool[] forbiddenChars = new bool[char.MaxValue + 1];
			foreach(var forbiddenChar in chars.Flatten())
				forbiddenChars[forbiddenChar] = true;

			return UnicodeCharacterIn(UnicodeCategories.All.Where(cp => cp.IsSurrogatePair || !forbiddenChars[cp.FirstCodeUnit]));
		}

		public static IParsingExpression<Nil> UnicodeCharacterNotIn(params char[] chars) {
			return UnicodeCharacterNotIn(chars.AsEnumerable());
		}

		public static IParsingExpression<Nil> UnicodeCharacterNotIn(ParsingExpression e, params IEnumerable<UnicodeCodePoint>[] categories) {
			var forbiddenCodepoints = new HashSet<UnicodeCodePoint>(categories.Flatten());
			return UnicodeCharacterIn(UnicodeCategories.All.Where(cp => !forbiddenCodepoints.Contains(cp)));
		}

		private static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) {
			return enumerable.SelectMany(i => i);
		}
	}
}
