using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Utils {
	public static class JavascriptUtils {
		private const char UNDEFINED_ESCAPE = '*';
		private const char MAX_ENCODE_ESCAPE = '\\';
		private const char MAX_DECODE_ESCAPE = 'v';
		private const char MAX_DECODE_HEX = 'f';
		private static readonly int[] DECODE_HEX;
		private static readonly char[] ENCODE_HEX;
		private static readonly char[] DECODE_ESCAPE;
		private static readonly char[] ENCODE_ESCAPE;

		static JavascriptUtils() {
			// Per ECMA-262 7.8.4 p23

			ENCODE_ESCAPE = new char[MAX_ENCODE_ESCAPE+1];
			for(var i = 0; i < ENCODE_ESCAPE.Length; i++) ENCODE_ESCAPE[i] = UNDEFINED_ESCAPE;

			DECODE_ESCAPE = new char[MAX_DECODE_ESCAPE+1];
			for(var i = 0; i < DECODE_ESCAPE.Length; i++) DECODE_ESCAPE[i] = UNDEFINED_ESCAPE;

			ENCODE_ESCAPE['\''] = '\'';		DECODE_ESCAPE['\''] = '\'';
			ENCODE_ESCAPE['"'] = '"';			DECODE_ESCAPE['"'] = '"';
			ENCODE_ESCAPE['\\'] = '\\';		DECODE_ESCAPE['\\'] = '\\';
			ENCODE_ESCAPE['\b'] = 'b';		DECODE_ESCAPE['b'] = '\b';
			ENCODE_ESCAPE['\f'] = 'f';		DECODE_ESCAPE['f'] = '\f';
			ENCODE_ESCAPE['\n'] = 'n';		DECODE_ESCAPE['n'] = '\n';
			ENCODE_ESCAPE['\r'] = 'r';		DECODE_ESCAPE['r'] = '\r';
			ENCODE_ESCAPE['\t'] = 't';		DECODE_ESCAPE['t'] = '\t';
			ENCODE_ESCAPE['\v'] = 'v';		DECODE_ESCAPE['v'] = '\v';
			ENCODE_ESCAPE['\x00'] = '0';	DECODE_ESCAPE['0'] = '\x00';

			ENCODE_HEX = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

			DECODE_HEX = new int[MAX_DECODE_HEX+1];
			for(var i = 0; i < DECODE_HEX.Length; i++) DECODE_HEX[i] = -1;

			DECODE_HEX['0'] = 0;
			DECODE_HEX['1'] = 1;
			DECODE_HEX['2'] = 2;
			DECODE_HEX['3'] = 3;
			DECODE_HEX['4'] = 4;
			DECODE_HEX['5'] = 5;
			DECODE_HEX['6'] = 6;
			DECODE_HEX['7'] = 7;
			DECODE_HEX['8'] = 8;
			DECODE_HEX['9'] = 9;
			DECODE_HEX['a'] = DECODE_HEX['A'] = 10;
			DECODE_HEX['b'] = DECODE_HEX['B'] = 11;
			DECODE_HEX['c'] = DECODE_HEX['C'] = 12;
			DECODE_HEX['d'] = DECODE_HEX['D'] = 13;
			DECODE_HEX['e'] = DECODE_HEX['E'] = 14;
			DECODE_HEX['f'] = DECODE_HEX['F'] = 15;
		}

		/// <summary>
		/// Decode a string from its javascript source representation.
		/// </summary>
		public static string DecodeString(string encoded) {
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

						if(MAX_DECODE_ESCAPE >= c && UNDEFINED_ESCAPE != DECODE_ESCAPE[c]) {
							buffer[wp++] = DECODE_ESCAPE[c];
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

				if(hc1 >= MAX_DECODE_HEX || hc2 >= MAX_DECODE_HEX)
					break;

				var hv1 = DECODE_HEX[hc1];
				var hv2 = DECODE_HEX[hc2];
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

				if(uc1 > MAX_DECODE_HEX || uc2 > MAX_DECODE_HEX || uc3 > MAX_DECODE_HEX || uc4 > MAX_DECODE_HEX)
					break;

				var uv1 = DECODE_HEX[uc1];
				var uv2 = DECODE_HEX[uc2];
				var uv3 = DECODE_HEX[uc3];
				var uv4 = DECODE_HEX[uc4];

				if(-1 == uv1 || -1 == uv2 || -1 == uv3 || -1 == uv4)
					break;

				decoded = (char)(uv1 * 0x1000 + uv2 * 0x100 + uv3 * 0x10 + uv4);
				return true;
			} while(false);

			decoded = '\x00';
			return false;
		}

		/// <summary>
		/// Encode a string into its unquoted javascript source representation.
		/// </summary>
		public static string EncodeString(string unencoded) {
			var sb = new StringBuilder(unencoded.Length * 2);
			var len = unencoded.Length;
			var rp = 0;
			char c;

			while(rp != len) {
				c = unencoded[rp++];
				
				if(MAX_ENCODE_ESCAPE >= c && UNDEFINED_ESCAPE != ENCODE_ESCAPE[c]) {
					sb.Append('\\');
					sb.Append(ENCODE_ESCAPE[c]);
				} else if(!(' ' <= c && c <= '~')) {
					sb.Append(@"\u");
					sb.Append(ENCODE_HEX[(c >> 12) & 0x0F]);
					sb.Append(ENCODE_HEX[(c >>  8) & 0x0F]);
					sb.Append(ENCODE_HEX[(c >>  4) & 0x0F]);
					sb.Append(ENCODE_HEX[c & 0x0F]);
				} else {
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		public static string EncodeNumber(double number) {
			// TODO: this is probably insufficient.
			return number.ToString();
		}
	}
}
