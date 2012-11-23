using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Utils {
	public static class JavascriptUtils {
		private const char UNDEFINED_ESCAPE = '*';
		private const char MAX_DECODE_HEX_CHAR = 'f';
		private static readonly int[] DECODE_HEX;
		private static readonly char[] ENCODE_HEX;
		private static readonly char[] DECODE_ESCAPE;
		private static readonly char[] ENCODE_ESCAPE;

		static JavascriptUtils() {
			// Per ECMA-262 7.8.4 p23

			ENCODE_ESCAPE = new char['\\'+1];
			for(var i = 0; i < ENCODE_ESCAPE.Length; i++) ENCODE_ESCAPE[i] = UNDEFINED_ESCAPE;

			DECODE_ESCAPE = new char['v'+1];
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

			DECODE_HEX = new int[MAX_DECODE_HEX_CHAR+1];
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
		public static string DecodeString(string content) {
			var len = content.Length;
			var buffer = new char[len];
			var rp = 0;
			var wp = 0;
			char c, dc;

			while(rp != len) {
				c = content[rp++];

				if('\\' == c) {
					c = content[rp++];

					if('x' == c) { // Hexadecimal escape sequence
						if(DecodeHex(content, rp, out dc)) {
							rp += 2;
							buffer[wp++] = dc;
						} else {
							buffer[wp++] = 'x';
						}
					} else if('u' == c) { // Unicode escape sequence
						if(DecodeUnicode(content, rp, out dc)) {
							rp += 4;
							buffer[wp++] = dc;
						} else {
							buffer[wp++] = 'u';
						}
					} else if(UNDEFINED_ESCAPE != DECODE_ESCAPE[c]) {
						// Escape sequence
						buffer[wp++] = DECODE_ESCAPE[c];
					} else {
						buffer[wp++] = c;
					}
				} else {
					buffer[wp++] = c;
				}
			}

			return new string(buffer, 0, wp);
		}

		private static bool DecodeHex(string s, int i, out char c) {
			do {
				if(2 > s.Length - i)
					break;

				var hc1 = s[i++];
				var hc2 = s[i];

				if(hc1 >= MAX_DECODE_HEX_CHAR || hc2 >= MAX_DECODE_HEX_CHAR)
					break;

				var hv1 = DECODE_HEX[hc1];
				var hv2 = DECODE_HEX[hc2];
				if(-1 == hv1 || -1 == hv2)
					break;

				c = (char)(hv1 * 0x10 + hv2);
				return true;
			} while(false);

			c = '\x00';
			return false;
		}

		private static bool DecodeUnicode(string s, int i, out char c) {
			do {
				if(4 > s.Length - i)
					break;

				var uc1 = s[i++];
				var uc2 = s[i++];
				var uc3 = s[i++];
				var uc4 = s[i];

				if(uc1 > MAX_DECODE_HEX_CHAR || uc2 > MAX_DECODE_HEX_CHAR || uc3 > MAX_DECODE_HEX_CHAR || uc4 > MAX_DECODE_HEX_CHAR)
					break;

				var uv1 = DECODE_HEX[uc1];
				var uv2 = DECODE_HEX[uc2];
				var uv3 = DECODE_HEX[uc3];
				var uv4 = DECODE_HEX[uc4];

				if(-1 == uv1 || -1 == uv2 || -1 == uv3 || -1 == uv4)
					break;

				c = (char)(uv1 * 0x1000 + uv2 * 0x100 + uv3 * 0x10 + uv4);
				return true;
			} while(false);

			c = '\x00';
			return false;
		}
	}
}
