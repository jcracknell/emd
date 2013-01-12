using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs.Unicode {
	public static class UnicodeUtils {
		/// <summary>
		/// The minimum value for a UTF-32 code point, 0.
		/// </summary>
		public const int MinCodePoint = 0x000000;

		/// <summary>
		/// The maximum value for a UTF-32 code point, 0x10FFFF.
		/// </summary>
		public const int MaxCodePoint =  0x10FFFF;

		private delegate UnicodeCategory MCharUnicodeInfo_InternalGetUnicodeCategory(int ch);
		private static readonly MCharUnicodeInfo_InternalGetUnicodeCategory
		CharUnicodeInfo_InternalGetUnicodeCategory = (MCharUnicodeInfo_InternalGetUnicodeCategory)
			Delegate.CreateDelegate(
				typeof(MCharUnicodeInfo_InternalGetUnicodeCategory),
				ReflectionUtils.GetAllMethods(typeof(CharUnicodeInfo))
				.Single(m =>
					"InternalGetUnicodeCategory".Equals(m.Name)
					&& m.GetParameters().Select(p => p.ParameterType)
						.SequenceEqual(
							typeof(MCharUnicodeInfo_InternalGetUnicodeCategory)
							.GetMethod("Invoke").GetParameters().Select(p => p.ParameterType)
						)
				)
			);

		/// <summary>
		/// <para>
		/// Retrieve the <see cref="UnicodeCategory"/> and <paramref name="length"/> of the UTF-16 grapheme at the specified
		/// <paramref name="index"/> in <paramref name="str"/>.
		/// </para>
		/// <para>
		/// The Unicode Consortium defines a grapheme as a "minimally distinctive unit of writing unit of
		/// writing in context of a particular writing system", or more succinctly as "what end users think
		/// of as characters".
		/// This method returns the <see cref="UnicodeCategory"/> of the character (or surrogate pair) at <paramref name="index"/>,
		/// and also provides the length of the character and any following combining marks making a multi-character grapheme
		/// provided that they can be combined with the initial character.
		/// </para> 
		/// </summary>
		/// <param name="length">The length of the grapheme starting at <paramref name="index"/> in <paramref name="str"/>.</param>
		public static UnicodeCategory GetGraphemeInfo(string str, int index, out int length) {
			if(null == str) throw Xception.Because.ArgumentNull(() => str);

			// The logic of this method is based heavily on that of System.Globalization.StringInfo::GetNextTextElement(string,int):string,
			// modified to create a much more flexible method allowing retrieval of grapheme length and category in a single operation
			// and without creating a substring.

			// Category is determined entirely by the first element
			int codePoint;
			var category = GetCodePointInfo(str, index, out codePoint, out length);
		
			// Can we add combining marks to the initial character?
			if(
				!IsCombiningCategory(category)
				&& UnicodeCategory.Format != category
				&& UnicodeCategory.Control != category
				&& UnicodeCategory.OtherNotAssigned != category
				&& UnicodeCategory.Surrogate != category
			) {
				// Read any combining marks following the initial character.
				int currentCharLength;
				int currentIndex = index + length;
				int strLength = str.Length;
				if (strLength != currentIndex && IsCombiningCategory(GetCodePointInfo(str, currentIndex, out codePoint, out currentCharLength))) {
					currentIndex += currentCharLength;

					while (strLength != currentIndex && IsCombiningCategory(GetCodePointInfo(str, currentIndex, out codePoint, out currentCharLength)))
						currentIndex += currentCharLength;

					// Update length now that we have consumed combining marks
					length = currentIndex - index;
				}
			}

			return category;
		}

		private static bool IsCombiningCategory(UnicodeCategory uc) {
			return UnicodeCategory.NonSpacingMark == uc
				|| UnicodeCategory.SpacingCombiningMark == uc
				|| UnicodeCategory.EnclosingMark == uc;
		}

		/// <summary>
		/// Retrieve the <see cref="UnicodeCategory"/> and <paramref name="length"/> of the UTF-16 code point at the specified
		/// <paramref name="index"/> in <paramref name="str"/>.
		/// </summary>
		/// <param name="codePoint">The UTF-32 code point starting at <paramref name="index"/> in <paramref name="str"/>.</param>
		/// <param name="length">The length of the code point starting at <paramref name="index"/> in <paramref name="str"/>.</param>
		public static UnicodeCategory GetCodePointInfo(string str, int index, out int codePoint, out int length) {
			if(null == str) throw Xception.Because.ArgumentNull(() => str);

			codePoint = GetCodePoint(str, index, out length);
			return CharUnicodeInfo_InternalGetUnicodeCategory(codePoint);
		}

		/// <summary>
		/// Retrieve the UTF-32 code point at the specified <paramref name="index"/> in <paramref name="str"/>.
		/// </summary>
		/// <param name="length">The length of the codepoint at <paramref name="index"/> in <paramref name="str"/>.</param>
		public static int GetCodePoint(string str, int index, out int length) {
			int value = (int)str[index];

			if(0xD800 <= value && value <= 0xDBFF && str.Length - 1 > index) {
				int lowSurrogate = (int)str[index + 1];

				if(0xDC00 <= lowSurrogate && lowSurrogate <= 0xDFFF) {
					length = 2;
					return 0x10000 + (((value - 0xD800) << 10) | (lowSurrogate - 0xDC00));
				}
			}

			length = 1;
			return value;
		}

		/// <summary>
		/// Returns true if the provided <paramref name="value"/> is a valid UTF-32 code point.
		/// </summary>
		public static bool IsValidCodePoint(int value) {
			return value <= MaxCodePoint && MinCodePoint <= value;
		}

		/// <summary>
		/// Returns the <see cref="UnicodeCategory"/> for the provided <paramref name="codePoint"/>.
		/// </summary>
		public static UnicodeCategory GetCategory(int codePoint) {
			// This method is useful because it is possible to determine the code point without knowing its category,
			// but not the opposite.
			return CharUnicodeInfo_InternalGetUnicodeCategory(codePoint);
		}
	}
}
