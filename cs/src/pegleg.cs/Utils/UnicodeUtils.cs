using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs.Utils {
	public static class UnicodeUtils {
		// System.Globalization.CharUnicodeInfo::InternalGetUnicodeCategory(string, int, out int):UnicodeCategory
		private delegate UnicodeCategory MCharUnicodeInfo_InternalGetUnicodeCategory(string str, int index, out int charLength);
		private static readonly MCharUnicodeInfo_InternalGetUnicodeCategory
		CharUnicodeInfo_InternalGetUnicodeCategory = (MCharUnicodeInfo_InternalGetUnicodeCategory)
			Delegate.CreateDelegate(
				typeof(MCharUnicodeInfo_InternalGetUnicodeCategory),
				typeof(CharUnicodeInfo)
					.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod)
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
		/// <param name="str">The <see cref="string"/> in from which grapheme information should be retrieved.</param>
		/// <param name="index">The start position in <paramref name="str"/> from which grapheme information should be retrieved.</param>
		/// <param name="length">The length of the grapheme starting at <paramref name="index"/> in <paramref name="str"/>.</param>
		public static UnicodeCategory GetGraphemeInfo(string str, int index, out int length) {
			if(null == str) throw Xception.Because.ArgumentNull(() => str);

			// The logic of this method is based heavily on that of System.Globalization.StringInfo::GetNextTextElement(string,int):string,
			// modified to create a much more flexible method.
			// It makes use of System.Globalization.CharUnicodeInfo::InternalGetUnicodeCategory(string, int, out int):UnicodeCategory, an
			// internal method, in order to be able to retrieve the length and category of a character at a specified index in a single
			// operation.

			// Category is determined entirely by the first element
			var category = CharUnicodeInfo_InternalGetUnicodeCategory(str, index, out length);
		
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
				if(strLength != currentIndex && IsCombiningCategory(CharUnicodeInfo_InternalGetUnicodeCategory(str, currentIndex, out currentCharLength))) {
					currentIndex += currentCharLength;

					while(strLength != currentIndex && IsCombiningCategory(CharUnicodeInfo_InternalGetUnicodeCategory(str, currentIndex, out currentCharLength)))
						currentIndex += currentCharLength;

					// Update length now that we have consumed combining marks
					length = currentIndex - index;
				}
			}

			return category;
		}

		/// <summary>
		/// Retrieve the <see cref="UnicodeCategory"/> and <paramref name="length"/> of the UTF-16 code point at the specified
		/// <paramref name="index"/> in <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The <see cref="string"/> from which code point information should be retrieved.</param>
		/// <param name="index">The start position in <paramref name="str"/> from which code point information should be retrieved.</param>
		/// <param name="length">The length of the code point starting at <paramref name="index"/> in <paramref name="str"/>.</param>
		public static UnicodeCategory GetCodePointInfo(string str, int index, out int length) {
			if(null == str) throw Xception.Because.ArgumentNull(() => str);

			return CharUnicodeInfo_InternalGetUnicodeCategory(str, index, out length);
		}

		private static bool IsCombiningCategory(UnicodeCategory uc) {
			return UnicodeCategory.NonSpacingMark == uc
				|| UnicodeCategory.SpacingCombiningMark == uc
				|| UnicodeCategory.EnclosingMark == uc;
		}

		/// <summary>
		/// Returns true if the provided <paramref name="str"/> contains a single unicode grapheme starting at index 0.
		/// </summary>
		/// <param name="str">A string to be verified as a single unicode grapheme.</param>
		/// <returns>True if the provided <paramref name="str"/> contains a single unicode grapheme starting at index 0.</returns>
		public static bool IsSingleGrapheme(string str) {
			int length;
			GetGraphemeInfo(str, 0, out length);

			return str.Length == length;
		}
	}
}
