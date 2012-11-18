using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Translation {
	public class JavascriptUtils {
		public static string Encode(string s) {
			throw new NotImplementedException();
		}

		public static string Encode(double d) {
			// TODO: there are probably some edge cases which need covering here
			return d.ToString();
		}
	}
}
