using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.ExtensionMethods {
	public static class PeglegExtensionMethods {
		public static string Join(this IEnumerable<string> parts) {
			return Join(parts, "");
		}

		public static string Join(this IEnumerable<string> parts, string separator) {
			var sb = new StringBuilder();
			var enumerator = parts.GetEnumerator();

			if(enumerator.MoveNext())
				sb.Append(enumerator.Current);

			while(enumerator.MoveNext())
				sb.Append(separator).Append(enumerator.Current);

			return sb.ToString();
		}

		public static int ParseDefault(this string s, int otherwise) {
			int v; return int.TryParse(s, out v) ? v : otherwise;
		}

		public static long ParseDefault(this string s, long otherwise) {
			long v; return long.TryParse(s, out v) ? v : otherwise;
		}

		public static uint ParseDefault(this string s, uint otherwise) {
			uint v; return uint.TryParse(s, out v) ? v : otherwise;
		}

		public static ulong ParseDefault(this string s, ulong otherwise) {
			ulong v; return ulong.TryParse(s, out v) ? v : otherwise;
		}

		public static float ParseDefault(this string s, float otherwise) {
			float v; return float.TryParse(s, out v) ? v : otherwise;
		}

		public static double ParseDefault(this string s, double otherwise) {
			double v; return double.TryParse(s, out v) ? v : otherwise;
		}

		public static decimal ParseDefault(this string s, decimal otherwise) {
			decimal v; return decimal.TryParse(s, out v) ? v : otherwise;
		}

		/// <summary>
		/// Puts the current value in an array with itself as the only entry.
		/// </summary>
		public static T[] InArray<T>(this T o) { return new T[] { o }; }

		/// <summary>
		/// Returns the value of the current <see cref="Nullable`1"/>, or the provided <paramref name="@default"/> value.
		/// Concise way of writing <code>(nullable.HasValue ? nullable.Value : @default)</code>.
		/// </summary>
		public static T ValueOr<T>(this Nullable<T> nullable, T @default) where T : struct {
			return nullable.HasValue ? nullable.Value : @default;
		}
	}
}
