using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace markdom.cs {
	public static class TextUtils {
		public static string RemoveDiacritics(string s) {
			var stFormD = s.Normalize(NormalizationForm.FormD);
			var sb = new StringBuilder();

			for(int i = 0; i < stFormD.Length; i++)
				if(UnicodeCategory.NonSpacingMark != CharUnicodeInfo.GetUnicodeCategory(stFormD[i]))
					sb.Append(stFormD[i]);

			return(sb.ToString().Normalize(NormalizationForm.FormC));
		}
	}
}
