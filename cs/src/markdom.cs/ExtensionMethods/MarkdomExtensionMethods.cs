using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.ExtensionMethods {
	public static class MarkdomExtensionMethods {
		public static string Join(this IEnumerable<string> parts) {
			return Join(parts, "");
		}

		public static string Join(this IEnumerable<string> parts, string separator) {
			return string.Join(separator, parts.ToArray());
		}
	}
}
