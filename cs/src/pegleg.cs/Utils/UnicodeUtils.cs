﻿using System;
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
		/// Retrieve the <see cref="UnicodeCategory"/> and length of the UTF-16 grapheme at the specified
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
			// The logic of this method is based heavily on that of System.Globalization.StringInfo::GetNextTextElement(string,int):string,
			// modified to create a much more flexible method.
			// It makes use of System.Globalization.CharUnicodeInfo::InternalGetUnicodeCategory(string, int, out int):UnicodeCategory, an
			// internal method, in order to be able to retrieve the length and category of a character at a specified index in a single
			// operation.

			var i = index;

			// Category is determined entirely by the first element
			UnicodeCategory category = CharUnicodeInfo_InternalGetUnicodeCategory(str, i, out length);
			i += length;
		
			// Can we add combining marks to the initial character?
			if(
				!IsCombiningCategory(category)
				&& UnicodeCategory.Format != category
				&& UnicodeCategory.Control != category
				&& UnicodeCategory.OtherNotAssigned != category
				&& UnicodeCategory.Surrogate != category
			) {
				// Read any combining marks following the initial character.
				var strLength = str.Length;
				while(strLength != i && IsCombiningCategory(CharUnicodeInfo_InternalGetUnicodeCategory(str, i, out length)))
					i += length;

				length = i - index;
			}

			return category;
		}

		private static bool IsCombiningCategory(UnicodeCategory uc) {
			return UnicodeCategory.NonSpacingMark == uc
				|| UnicodeCategory.SpacingCombiningMark == uc
				|| UnicodeCategory.EnclosingMark == uc;
		}

		public static bool IsSingleTextElement(string str) {
			int length;
			UnicodeCategory unicodeCategory;

			GetGraphemeInfo(str, 0, out length);

			return str.Length == length;
		}
	}
}
