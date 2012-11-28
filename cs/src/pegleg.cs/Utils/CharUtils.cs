using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Utils {
	public static class CharUtils {
		private static readonly char[] HEX_DIGIT = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
		private const uint LOW_FOUR = 0x0000000Fu;

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
			if(IsAsciiPrintableCharacter(c))
				return string.Concat("'", c, "'");
			return string.Concat("'\\x", AsHex(c), "'");
		}

		/// <summary>
		/// Returns true if the provided character is an ASCII printable character - a character in the range
		/// 0x20 (' ') to 0x7E ('~').
		/// </summary>
		public static bool IsAsciiPrintableCharacter(char c) {
			return ' ' <= c && c <= '~';
		}

		/// <summary>
		/// Returns true if the provided character is a 'normal' ASCII character - a printable character,
		/// newline ('\n'), tab ('\t'), or carriage return ('\r').
		/// </summary>
		public static bool IsAsciiNormalCharacter(char c) {
			return IsAsciiPrintableCharacter(c) || '\n' == c || '\t' == c || '\r' == c;
		}

		public static string AsHex(char c) {
			uint cv = c;

			char[] buffer = new char[4];
			buffer[3] = HEX_DIGIT[cv & LOW_FOUR];
			buffer[2] = HEX_DIGIT[(cv >>= 4) & LOW_FOUR];
			buffer[1] = HEX_DIGIT[(cv >>= 4) & LOW_FOUR];
			buffer[0] = HEX_DIGIT[(cv >>= 4) & LOW_FOUR];

			return new string(buffer);
		}
	}
}
