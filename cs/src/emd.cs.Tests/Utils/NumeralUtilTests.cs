using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Utils {
	public class NumeralUtilTests {
		[Fact]
		public void AlphaNumeral_renders_int_MaxValue() {
			NumeralUtils.AlphaNumeral(int.MaxValue);
		}

		[Fact]
		public void AlphaNumeral_renders_int_MinValue() {
			NumeralUtils.AlphaNumeral(int.MinValue);
		}

		public void AlphaNumeral_renders_0() {
			NumeralUtils.AlphaNumeral(0).Should().Be("");
		}

		[Fact]
		public void AlphaNumeral_renders_1() {
			NumeralUtils.AlphaNumeral(1).Should().Be("a");
		}

		[Fact]
		public void AlphaNumeral_renders_26() {
			NumeralUtils.AlphaNumeral(26).Should().Be("z");
		}

		[Fact]
		public void AlphaNumeral_renders_27() {
			NumeralUtils.AlphaNumeral(27).Should().Be("aa");
		}

		[Fact]
		public void AlphaNumeral_renders_17603() {
			NumeralUtils.AlphaNumeral(17603).Should().Be("zaa");
		}

		[Fact]
		public void AlphaNumeral_renders_negative_17603() {
			NumeralUtils.AlphaNumeral(-17603).Should().Be("-zaa");
		}

		[Fact]
		public void ParseAlphaNumeral_parses_a() {
			NumeralUtils.ParseAlphaNumeral("a").Should().Be(1);
		}

		[Fact]
		public void ParseAlphaNumeral_parses_aa() {
			NumeralUtils.ParseAlphaNumeral("aa").Should().Be(27);
		}

		[Fact]
		public void ParseAlphaNumeral_parses_zaa() {
			NumeralUtils.ParseAlphaNumeral("zaa").Should().Be(17603);
		}

		[Fact]
		public void RomanNumeral_renders_1() {
			NumeralUtils.RomanNumeral(1).Should().Be("i");
		}

		[Fact]
		public void RomanNumeral_renders_2() {
			NumeralUtils.RomanNumeral(2).Should().Be("ii");
		}

		[Fact]
		public void RomanNumeral_renders_3() {
			NumeralUtils.RomanNumeral(3).Should().Be("iii");
		}

		[Fact]
		public void RomanNumeral_renders_4() {
			NumeralUtils.RomanNumeral(4).Should().Be("iv");
		}

		[Fact]
		public void RomanNumeral_renders_5() {
			NumeralUtils.RomanNumeral(5).Should().Be("v");
		}

		[Fact]
		public void RomanNumeral_renders_6() {
			NumeralUtils.RomanNumeral(6).Should().Be("vi");
		}

		[Fact]
		public void RomanNumeral_renders_7() {
			NumeralUtils.RomanNumeral(7).Should().Be("vii");
		}

		[Fact]
		public void RomanNumeral_renders_8() {
			NumeralUtils.RomanNumeral(8).Should().Be("viii");
		}

		[Fact]
		public void RomanNumeral_renders_9() {
			NumeralUtils.RomanNumeral(9).Should().Be("ix");
		}

		[Fact]
		public void RomanNumeral_renders_1914() {
			NumeralUtils.RomanNumeral(1914).Should().Be("mcmxiv");
		}

		[Fact]
		public void RomanNumeral_renders_2012() {
			NumeralUtils.RomanNumeral(2012).Should().Be("mmxii");
		}

		[Fact]
		public void ParseRomanNumeral_parses_mcmxiv() {
			NumeralUtils.ParseRomanNumeral("mcmxiv").Should().Be(1914);
		}

		[Fact]
		public void ParseRomanNumeral_parses_3909() {
			NumeralUtils.ParseRomanNumeral("mmmcmix").Should().Be(3909);
		}

		[Fact]
		public void ParseRomanNumeral_does_not_parse_admiralty_arch() {
			NumeralUtils.ParseRomanNumeral("mdccccx").Should().Be(new int?());
		}

		[Fact]
		public void RomanNumeral_round_trip() {
			for(int i = -3999; i <= 3999; i++) {
				if(0 != i)
					NumeralUtils.ParseRomanNumeral(NumeralUtils.RomanNumeral(i)).Should().Be(i);
			}
		}
	}
}
