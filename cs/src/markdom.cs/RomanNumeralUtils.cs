using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs {
	public static class RomanNumeralUtils {
		public static int DigitValue(char digit) {
			switch(digit) {
				case 'i': return 1;
				case 'I': return 1;
				case 'v': return 5;
				case 'V': return 5;
				case 'x': return 10;
				case 'X': return 10;
				case 'l': return 50;
				case 'L': return 50;
				case 'c': return 100;
				case 'C': return 100;
				case 'd': return 500;
				case 'D': return 500;
				case 'm': return 1000;
				case 'M': return 1000;
				default: return -1;
			}
		}
	}
}
