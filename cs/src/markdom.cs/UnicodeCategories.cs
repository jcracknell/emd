using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs {
	public static partial class UnicodeCategories {
		
		public class CodePoint {
			private readonly char[] _chars;

			public CodePoint(char[] chars) {
				_chars = chars;
			}

			public char this[int i] { get { return _chars[i]; } }
			public int Length { get { return _chars.Length; } }
		}

		private static CodePoint C(params char[] chars) { return new CodePoint(chars); }

		public static IEnumerable<IEnumerable<CodePoint>> All {
			get {
				#region yields
				yield return Cc;
				yield return Cf;
				yield return Co;
				yield return Cs;
				yield return Ll;
				yield return Lm;
				yield return Lo;
				yield return Lt;
				yield return Lu;
				yield return Mc;
				yield return Me;
				yield return Mn;
				yield return Nd;
				yield return Nl;
				yield return No;
				yield return Pc;
				yield return Pd;
				yield return Pe;
				yield return Pf;
				yield return Pi;
				yield return Po;
				yield return Ps;
				yield return Sc;
				yield return Sk;
				yield return Sm;
				yield return So;
				yield return Zl;
				yield return Zp;
				yield return Zs;
				#endregion
			}
		}

		/// <summary>Other, Control</summary>
		public static IEnumerable<CodePoint> Cc { get { return UCc; } }
		/// <summary>Other, Format</summary>
		public static IEnumerable<CodePoint> Cf { get { return UCf; } }
		/// <summary>Other, Private Use</summary>
		public static IEnumerable<CodePoint> Co { get { return UCo; } }
		/// <summary>Other, Surrogate</summary>
		public static IEnumerable<CodePoint> Cs { get { return UCs; } }
		/// <summary>Letter, Lowercase</summary>
		public static IEnumerable<CodePoint> Ll { get { return ULl; } }
		/// <summary>Letter, Modifier</summary>
		public static IEnumerable<CodePoint> Lm { get { return ULm; } }
		/// <summary>Letter, Other</summary>
		public static IEnumerable<CodePoint> Lo { get { return ULo; } }
		/// <summary>Letter, Titlecase</summary>
		public static IEnumerable<CodePoint> Lt { get { return ULt; } }
		/// <summary>Letter, Uppercase</summary>
		public static IEnumerable<CodePoint> Lu { get { return ULu; } }
		/// <summary>Mark, Spacing Combining</summary>
		public static IEnumerable<CodePoint> Mc { get { return UMc; } }
		/// <summary>Mark, Enclosing</summary>
		public static IEnumerable<CodePoint> Me { get { return UMe; } }
		/// <summary>Mark, Nonspacing</summary>
		public static IEnumerable<CodePoint> Mn { get { return UMn; } }
		/// <summary>Number, Decimal Digit</summary>
		public static IEnumerable<CodePoint> Nd { get { return UNd; } }
		/// <summary>Number, Letter</summary>
		public static IEnumerable<CodePoint> Nl { get { return UNl; } }
		/// <summary>Number, Other</summary>
		public static IEnumerable<CodePoint> No { get { return UNo; } }
		/// <summary>Punctuation, Connector</summary>
		public static IEnumerable<CodePoint> Pc { get { return UPc; } }
		/// <summary>Punctuation, Dash</summary>
		public static IEnumerable<CodePoint> Pd { get { return UPd; } }
		/// <summary>Punctuation, Close</summary>
		public static IEnumerable<CodePoint> Pe { get { return UPe; } }
		/// <summary>Punctuation, Final quote</summary>
		public static IEnumerable<CodePoint> Pf { get { return UPf; } }
		/// <summary>Punctuation, Initial quote</summary>
		public static IEnumerable<CodePoint> Pi { get { return UPi; } }
		/// <summary>Punctuation, Other</summary>
		public static IEnumerable<CodePoint> Po { get { return UPo; } }
		/// <summary>Punctuation, Open</summary>
		public static IEnumerable<CodePoint> Ps { get { return UPs; } }
		/// <summary>Symbol, Currency</summary>
		public static IEnumerable<CodePoint> Sc { get { return USc; } }
		/// <summary>Symbol, Modifier</summary>
		public static IEnumerable<CodePoint> Sk { get { return USk; } }
		/// <summary>Symbol, Math</summary>
		public static IEnumerable<CodePoint> Sm { get { return USm; } }
		/// <summary>Symbol, Other</summary>
		public static IEnumerable<CodePoint> So { get { return USo; } }
		/// <summary>Separator, Line</summary>
		public static IEnumerable<CodePoint> Zl { get { return UZl; } }
		/// <summary>Separator, Paragraph</summary>
		public static IEnumerable<CodePoint> Zp { get { return UZp; } }
		/// <summary>Separator, Space</summary>
		public static IEnumerable<CodePoint> Zs { get { return UZs; } }
	}
}
