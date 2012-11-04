using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs {
	[TestClass]
	public class NumeralUtilTests {
		[TestMethod]
		public void AlphaNumeral_renders_int_MaxValue() {
			NumeralUtils.AlphaNumeral(int.MaxValue);
		}

		[TestMethod]
		public void AlphaNumeral_renders_int_MinValue() {
			NumeralUtils.AlphaNumeral(int.MinValue);
		}

		public void AlphaNumeral_renders_0() {
			Assert.AreEqual("", NumeralUtils.AlphaNumeral(0));
		}

		[TestMethod]
		public void AlphaNumeral_renders_1() {
			Assert.AreEqual("a", NumeralUtils.AlphaNumeral(1));
		}

		[TestMethod]
		public void AlphaNumeral_renders_26() {
			Assert.AreEqual("z", NumeralUtils.AlphaNumeral(26));
		}

		[TestMethod]
		public void AlphaNumeral_renders_27() {
			Assert.AreEqual("aa", NumeralUtils.AlphaNumeral(27));
		}

		[TestMethod]
		public void AlphaNumeral_renders_17603() {
			Assert.AreEqual("zaa", NumeralUtils.AlphaNumeral(17603));
		}

		[TestMethod]
		public void AlphaNumeral_renders_negative_17603() {
			Assert.AreEqual("-zaa", NumeralUtils.AlphaNumeral(-17603));
		}

		[TestMethod]
		public void ParseAlphaNumeral_parses_a() {
			Assert.AreEqual(1, NumeralUtils.ParseAlphaNumeral("a"));
		}

		[TestMethod]
		public void ParseAlphaNumeral_parses_aa() {
			Assert.AreEqual(27, NumeralUtils.ParseAlphaNumeral("aa"));
		}

		[TestMethod]
		public void ParseAlphaNumeral_parses_zaa() {
			Assert.AreEqual(17603, NumeralUtils.ParseAlphaNumeral("zaa"));
		}

		[TestMethod]
		public void RomanNumeral_renders_1() {
			Assert.AreEqual("i", NumeralUtils.RomanNumeral(1));
		}

		[TestMethod]
		public void RomanNumeral_renders_2() {
			Assert.AreEqual("ii", NumeralUtils.RomanNumeral(2));
		}

		[TestMethod]
		public void RomanNumeral_renders_3() {
			Assert.AreEqual("iii", NumeralUtils.RomanNumeral(3));
		}

		[TestMethod]
		public void RomanNumeral_renders_4() {
			Assert.AreEqual("iv", NumeralUtils.RomanNumeral(4));
		}

		[TestMethod]
		public void RomanNumeral_renders_5() {
			Assert.AreEqual("v", NumeralUtils.RomanNumeral(5));
		}

		[TestMethod]
		public void RomanNumeral_renders_6() {
			Assert.AreEqual("vi", NumeralUtils.RomanNumeral(6));
		}

		[TestMethod]
		public void RomanNumeral_renders_7() {
			Assert.AreEqual("vii", NumeralUtils.RomanNumeral(7));
		}

		[TestMethod]
		public void RomanNumeral_renders_8() {
			Assert.AreEqual("viii", NumeralUtils.RomanNumeral(8));
		}

		[TestMethod]
		public void RomanNumeral_renders_9() {
			Assert.AreEqual("ix", NumeralUtils.RomanNumeral(9));
		}

		[TestMethod]
		public void RomanNumeral_renders_1914() {
			Assert.AreEqual("mcmxiv", NumeralUtils.RomanNumeral(1914));
		}

		[TestMethod]
		public void RomanNumeral_renders_2012() {
			Assert.AreEqual("mmxii", NumeralUtils.RomanNumeral(2012));
		}

		[TestMethod]
		public void ParseRomanNumeral_parses_mcmxiv() {
			Assert.AreEqual(1914, NumeralUtils.ParseRomanNumeral("mcmxiv"));
		}

		[TestMethod]
		public void ParseRomanNumeral_parses_3909() {
			Assert.AreEqual(3909, NumeralUtils.ParseRomanNumeral("mmmcmix"));
		}

		[TestMethod]
		public void ParseRomanNumeral_does_not_parse_admiralty_arch() {
			Assert.AreEqual(new int?(), NumeralUtils.ParseRomanNumeral("mdccccx"));
		}

		[TestMethod]
		public void RomanNumeral_round_trip() {
			for(int i = -3999; i <= 3999; i++) {
				if(0 != i)
					Assert.AreEqual(i, NumeralUtils.ParseRomanNumeral(NumeralUtils.RomanNumeral(i)));
			}
		}
	}
}
