using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Utils {
	public static class NumeralUtils {

		#region Alpha

		public static string AlphaNumeral(int value) {
			const int BUFFER_SIZE = 8;
			const int A = (int)'a' - 1;

			if(0 == value) return "";

			long v = value;
			int sign;
			if(v < 0) {
				sign = -1;
				v = (v ^ (-1)) + 1;
			} else {
				sign = 1;
			}

			var buffer = new char[BUFFER_SIZE];
			var i = BUFFER_SIZE;
			long digitValue;

			while(0 != v) {
				digitValue = v % 26;
				if(0 == digitValue) digitValue = 26;

				buffer[--i] = (char)(A + digitValue);
				v = (v - digitValue) / 26;
			}

			if(-1 == sign) buffer[--i] = '-';

			return new string(buffer, i, BUFFER_SIZE - i);
		}

		public static int? ParseAlphaNumeral(string s) {
			const int MAX_DIGITS = 7;
			int len = s.Length;
			int i = -1;

			while(i != len && char.IsWhiteSpace(s[++i]));

			long sign = 1;
			if(i != len) {
				if('-' == s[i]) {
					sign = -1;
					i++;
				} else if('+' == s[i]) {
					i++;
				}
			}

			long accumulator = 0;
			char c;
			int digits = 0;
			while(i != len && digits != MAX_DIGITS) {
				c = s[i++];
				if('A' <= c && c <= 'z') {
					if('a' <= c) {
						accumulator = accumulator * 26 + c - ('a' - 1);
						digits++;
						continue;
					}
					if(c <= 'Z') {
						accumulator = accumulator * 26 + c - ('A' - 1);
						digits++;
						continue;
					}
				}	
				break;
			}

			accumulator = accumulator * sign;
			if(0 == accumulator || accumulator > int.MaxValue || accumulator < int.MinValue)
				return new int?();

			return (int)accumulator;
		}

		#endregion

		#region Roman

		public static string RomanNumeral(int value) {
			if(0 == value || value > 3999 || value < -3999) return "";

			string sign;
			if(value < 0) {
				sign = "-";
				value = value * -1;
			} else {
				sign = "";
			}

			return string.Concat(
				sign,
				new string('m', value / 1000),
				RomanNumeralDecade(value / 100 % 10, 'm', 'd', 'c'), 
				RomanNumeralDecade(value / 10 % 10, 'c', 'l', 'x'),
				RomanNumeralDecade(value % 10, 'x', 'v', 'i')
			);
		}

		private static string RomanNumeralDecade(int value, char decem, char quintum, char unit) {
			switch(value) {
				case 0: return string.Empty;
				case 1: return new string(new char[] { unit });
				case 2: return new string(new char[] { unit, unit });
				case 3: return new string(new char[] { unit, unit, unit });
				case 4: return new string(new char[] { unit, quintum });
				case 5: return new string(new char[] { quintum });
				case 6: return new string(new char[] { quintum, unit });
				case 7: return new string(new char[] { quintum, unit, unit });
				case 8: return new string(new char[] { quintum, unit, unit, unit });
				case 9: return new string(new char[] { unit, decem });
			}
			throw new Exception("Invalid value.");
		}

		private static readonly char[] DECADES = new char[] { 'm', 'd', 'c', 'l', 'x', 'v', 'i' };
		public static int? ParseRomanNumeral(string s) {
			s = s.ToLower();
			int len = s.Length;
			int i = -1;
			
			while(i != len && char.IsWhiteSpace(s[++i]));

			int sign = 1;
			if(i != len) {
				if('-' == s[i]) {
					sign = -1;
					i++;
				} else if('+' == s[i]) {
					i++;
				}
			}

			int value = 0;
			while(0 != len - i && 'm' == char.ToLower(s[i]) && 3000 != value) {
				value += 1000;
				i++;
			}


			int decade = 0;
			int remaining = len - i;
			int factor = 100;
			while(decade <= 4 && remaining > 0) {
				char decem = DECADES[decade++];
				char quintum = DECADES[decade++];
				char unit = DECADES[decade];

				if(unit == s[i]) {
					if(remaining >= 2) {
						if(decem == s[i+1]) {
							value += 9 * factor;
							i += 2;
						} else if(quintum == s[i+1]) { // iv
							value += 4 * factor;
							i += 2;
						} else if(unit == s[i+1]) { 
							if(remaining >= 3 && unit == s[i+2]) { // iii
								value += 3 * factor;
								i += 3;
							} else {
								value += 2 * factor;
								i += 2;
							}
						} else {
							value += factor;
							i += 1;
						}
					} else {
						value += factor;
						i += 1;
					}
				} else if(quintum == s[i]) {
					if(remaining >= 2 && unit == s[i+1]) {
						if(remaining >= 3 && unit == s[i+2]) {
							if(remaining >= 4 && unit == s[i+3]) {
								value += 8 * factor;
								i += 4;
							} else {
								value += 7 * factor;
								i += 3;
							}
						} else {
							value += 6 * factor;
							i += 2;
						}
					} else {
						value += 5 * factor;
						i += 1;
					}
				}

				factor = factor / 10;
				remaining = len - i;
			}

			while(i != len)
				if(!char.IsWhiteSpace(s[i++]))
					return new int?();

			return 0 == value ? new int?() : sign * value;
		}


		

		#endregion
	}
}
