using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Utils {
	public static class JavaScriptUtils {
		private const char UNDEFINED_ESCAPE = '*';
		private const char MAX_ESCAPE_ENCODE = '\\';
		private const char MAX_ESCAPE_DECODE = 'v';
		private const char MAX_HEX_DECODE = 'f';
		private static readonly int[] HEX_DECODE;
		private static readonly char[] HEX_ENCODE;
		private static readonly char[] ESCAPE_DECODE;
		private static readonly char[] ESCAPE_ENCODE;

		static JavaScriptUtils() {
			// Per ECMA-262 7.8.4 p23

			ESCAPE_ENCODE = new char[MAX_ESCAPE_ENCODE+1];
			for(var i = 0; i < ESCAPE_ENCODE.Length; i++) ESCAPE_ENCODE[i] = UNDEFINED_ESCAPE;

			ESCAPE_DECODE = new char[MAX_ESCAPE_DECODE+1];
			for(var i = 0; i < ESCAPE_DECODE.Length; i++) ESCAPE_DECODE[i] = UNDEFINED_ESCAPE;

			ESCAPE_ENCODE['\''] = '\'';		ESCAPE_DECODE['\''] = '\'';
			ESCAPE_ENCODE['"'] = '"';			ESCAPE_DECODE['"'] = '"';
			ESCAPE_ENCODE['\\'] = '\\';		ESCAPE_DECODE['\\'] = '\\';
			ESCAPE_ENCODE['\b'] = 'b';		ESCAPE_DECODE['b'] = '\b';
			ESCAPE_ENCODE['\f'] = 'f';		ESCAPE_DECODE['f'] = '\f';
			ESCAPE_ENCODE['\n'] = 'n';		ESCAPE_DECODE['n'] = '\n';
			ESCAPE_ENCODE['\r'] = 'r';		ESCAPE_DECODE['r'] = '\r';
			ESCAPE_ENCODE['\t'] = 't';		ESCAPE_DECODE['t'] = '\t';
			ESCAPE_ENCODE['\v'] = 'v';		ESCAPE_DECODE['v'] = '\v';
			ESCAPE_ENCODE['\x00'] = '0';	ESCAPE_DECODE['0'] = '\x00';

			HEX_ENCODE = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

			HEX_DECODE = new int[MAX_HEX_DECODE+1];
			for(var i = 0; i < HEX_DECODE.Length; i++) HEX_DECODE[i] = -1;

			HEX_DECODE['0'] = 0;
			HEX_DECODE['1'] = 1;
			HEX_DECODE['2'] = 2;
			HEX_DECODE['3'] = 3;
			HEX_DECODE['4'] = 4;
			HEX_DECODE['5'] = 5;
			HEX_DECODE['6'] = 6;
			HEX_DECODE['7'] = 7;
			HEX_DECODE['8'] = 8;
			HEX_DECODE['9'] = 9;
			HEX_DECODE['a'] = HEX_DECODE['A'] = 10;
			HEX_DECODE['b'] = HEX_DECODE['B'] = 11;
			HEX_DECODE['c'] = HEX_DECODE['C'] = 12;
			HEX_DECODE['d'] = HEX_DECODE['D'] = 13;
			HEX_DECODE['e'] = HEX_DECODE['E'] = 14;
			HEX_DECODE['f'] = HEX_DECODE['F'] = 15;
		}

		/// <summary>
		/// Decode an identifier name from its JavaScript source representation.
		/// </summary>
		public static string IdentifierDecode(string encoded) {
			var len = encoded.Length;
			var buffer = new char[len];
			var rp = 0;
			var wp = 0;
			char c, dc;

			while(rp != len) {
				c = encoded[rp++];

				if('\\' == c && TryDecodeUnicodeEscape(encoded, rp, out dc)) {
					rp += 5;
					buffer[wp++] = dc;
				} else {
					buffer[wp++] = c;
				}
			}

			return new string(buffer, 0, wp);
		}

		/// <summary>
		/// Encode an identifier name into its JavaScript source representation.
		/// This method targets ASCII for maximum compatibility.
		/// </summary>
		public static string IdentifierEncode(string unencoded) {
			var len = unencoded.Length;
			var sb = new StringBuilder(unencoded.Length * 2);
			var rp = 0;
			char c;

			while(rp != len) {
				c = unencoded[rp++];

				if(CharUtils.IsAsciiPrintableCharacter(c)) {
					sb.Append(c);
				} else {
					AppendUnicodeEscape(sb, c);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Decode a string from its JavaScript source representation.
		/// </summary>
		public static string StringDecode(string encoded) {
			var len = encoded.Length;
			var buffer = new char[len];
			var rp = 0;
			var wp = 0;
			char c, dc;

			while(rp != len) {
				c = encoded[rp++];

				if('\\' == c) {
					if(TryDecodeHexadecimalEscape(encoded, rp, out dc)) {
						rp += 3;
						buffer[wp++] = dc;
					} else if(TryDecodeUnicodeEscape(encoded, rp, out dc)) {
						rp += 5;
						buffer[wp++] = dc;
					} else {
						c = encoded[rp++];

						if(MAX_ESCAPE_DECODE >= c && UNDEFINED_ESCAPE != ESCAPE_DECODE[c]) {
							buffer[wp++] = ESCAPE_DECODE[c];
						} else {
							buffer[wp++] = c;
						}
					}
				} else {
					buffer[wp++] = c;
				}
			}

			return new string(buffer, 0, wp);
		}

		/// <summary>
		/// Encode a string into its unquoted javascript source representation.
		/// This method targets ASCII for maximum compatibility.
		/// </summary>
		public static string StringEncode(string unencoded) {
			var sb = new StringBuilder(unencoded.Length * 2);
			var len = unencoded.Length;
			var rp = 0;
			char c;

			while(rp != len) {
				c = unencoded[rp++];
				
				if(MAX_ESCAPE_ENCODE >= c && UNDEFINED_ESCAPE != ESCAPE_ENCODE[c]) {
					sb.Append('\\');
					sb.Append(ESCAPE_ENCODE[c]);
				} else if(CharUtils.IsAsciiPrintableCharacter(c)) {
					sb.Append(c);
				} else {
					AppendUnicodeEscape(sb, c);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Attempt to decode a three-character hexadecimal escape sequence of the form <code>xHH</code> at the specified <paramref name="index"/> in the provided <paramref name="string"/>
		/// </summary>
		/// <param name="string">The string from which a hexadecimal escape sequence should be decoded.</param>
		/// <param name="index">The index at which a hexidecimal escape sequence should be decoded.</param>
		/// <param name="decoded">The decoded hexadecimal escape sequence.</param>
		/// <returns><code>true</code> if a hexadecimal escape sequence was successfully decoded, <code>false</code> otherwise.</returns>
		public static bool TryDecodeHexadecimalEscape(string @string, int index, out char decoded) {
			do {
				if(3 > @string.Length - index || 'x' != @string[index++])
					break;

				var hc1 = @string[index++];
				var hc2 = @string[index];

				if(hc1 >= MAX_HEX_DECODE || hc2 >= MAX_HEX_DECODE)
					break;

				var hv1 = HEX_DECODE[hc1];
				var hv2 = HEX_DECODE[hc2];
				if(-1 == hv1 || -1 == hv2)
					break;

				decoded = (char)(hv1 * 0x10 + hv2);
				return true;
			} while(false);

			decoded = '\x00';
			return false;
		}

		/// <summary>
		/// Attempt to decode a five-character unicode escape sequence of the form <code>uHHHH</code> at the specified <paramref name="index"/> in the provided <paramref name="string"/>.
		/// </summary>
		/// <param name="string">The string from which a unicode escape sequence should be decoded.</param>
		/// <param name="index">The index at which a unicode escape sequence should be decoded.</param>
		/// <param name="decoded">The decoded unicode escape sequence.</param>
		/// <returns><code>true</code> if a unicode escape sequence was successfully decoded, <code>false</code> otherwise.</returns>
		public static bool TryDecodeUnicodeEscape(string @string, int index, out char decoded) {
			do {
				if(5 > @string.Length - index || 'u' != @string[index++])
					break;

				var uc1 = @string[index++];
				var uc2 = @string[index++];
				var uc3 = @string[index++];
				var uc4 = @string[index];

				if(uc1 > MAX_HEX_DECODE || uc2 > MAX_HEX_DECODE || uc3 > MAX_HEX_DECODE || uc4 > MAX_HEX_DECODE)
					break;

				var uv1 = HEX_DECODE[uc1];
				var uv2 = HEX_DECODE[uc2];
				var uv3 = HEX_DECODE[uc3];
				var uv4 = HEX_DECODE[uc4];

				if(-1 == uv1 || -1 == uv2 || -1 == uv3 || -1 == uv4)
					break;

				decoded = (char)(uv1 * 0x1000 + uv2 * 0x100 + uv3 * 0x10 + uv4);
				return true;
			} while(false);

			decoded = '\x00';
			return false;
		}

		private static void AppendUnicodeEscape(StringBuilder sb, char c) {
			sb
				.Append(@"\u")
				.Append(HEX_ENCODE[(c >> 12) & 0x0F])
				.Append(HEX_ENCODE[(c >>  8) & 0x0F])
				.Append(HEX_ENCODE[(c >>  4) & 0x0F])
				.Append(HEX_ENCODE[c & 0x0F]);
		}

		public static string NumberEncode(double number) {
			// TODO: this is probably insufficient.
			return number.ToString();
		}
	}
}
