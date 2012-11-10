﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Utils {
	public static class CharUtils {
		/// <summary>
		/// Creates an enumerable containing all char values between and including the provided values.
		/// </summary>
		public static IEnumerable<char> Range(char start, char end) {
			char c = start;
			if(start <= end) {
				while(c <= end && c != char.MaxValue)
					yield return c++;
				if(char.MaxValue == end)
					yield return char.MaxValue;
			} else {
				while(c >= end && c != char.MinValue)
					yield return c--;
				if(char.MinValue == end)
					yield return char.MinValue;
			}
		}

		/// <summary>
		/// Compute a minimal set of <see cref="CharacterRange"/> instances covering the provided characters.
		/// </summary>
		public static IEnumerable<CharacterRange> RangesCovering(IEnumerable<char> chars) {
			var ranges = new LinkedList<CharacterRange>();
			var enumerator = chars.OrderBy(c => c).Distinct().GetEnumerator();
			if(enumerator.MoveNext()) {
				var rangeStart = enumerator.Current;
				var rangeEnd = rangeStart;

				while(enumerator.MoveNext()) {
					if(enumerator.Current == rangeEnd + 1) {
						rangeEnd = enumerator.Current;
					} else {
						ranges.AddLast(new CharacterRange(rangeStart, rangeEnd));
						rangeStart = rangeEnd = enumerator.Current;
					}
				}

				ranges.AddLast(new CharacterRange(rangeStart, rangeEnd));
			}

			return ranges;
		}

		/// <summary>
		/// Compute a minimal set of <see cref="CharacterRange"/> instances excluding the provided characters.
		/// </summary>
		public static IEnumerable<CharacterRange> RangesExcluding(IEnumerable<char> chars) {
			var enumerator = chars.OrderBy(c => c).Distinct().GetEnumerator();
			if(!enumerator.MoveNext()) 
				return new CharacterRange[] { new CharacterRange(char.MinValue, char.MaxValue) };

			var ranges = new LinkedList<CharacterRange>();
			if(char.MinValue != enumerator.Current)
				ranges.AddLast(new CharacterRange(char.MinValue, (char)(enumerator.Current - 1)));

			var rangeStart = enumerator.Current + 1;
			while(enumerator.MoveNext()) {
				if(enumerator.Current != rangeStart)
					ranges.AddLast(new CharacterRange((char)rangeStart, (char)(enumerator.Current - 1)));

				if(char.MaxValue != enumerator.Current)
					rangeStart = enumerator.Current + 1;
			}

			if(char.MaxValue != rangeStart)
				ranges.AddLast(new CharacterRange((char)rangeStart, char.MaxValue));

			return ranges;
		}

		public static string LiteralEncode(char c) {
			if(' ' <= c && c <= '~')
				return string.Concat("'", c, "'");
			return string.Concat("'\\x", ((int)c).ToString("X").PadLeft(4, '0'), "'");
		}
	}
}
