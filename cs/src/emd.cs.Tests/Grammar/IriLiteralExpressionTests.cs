using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar {
	public class IriLiteralExpressionTests : GrammarTestFixture {
		[Fact] public void IriExpression_matches_remote_uri() {
			EmdGrammar.IriLiteralExpression.ShouldMatchAllOf("http://www.google.com");
		}

		[Fact] public void IriExpression_matches_uri_with_balanced_parentheses() {
			var match = EmdGrammar.IriLiteralExpression.ShouldMatchAllOf("http://msdn.microsoft.com/en-us/library/a6td98xe(v=vs.71).aspx");

			match.Product.Value.Should().Be("http://msdn.microsoft.com/en-us/library/a6td98xe(v=vs.71).aspx");
		}

		[Fact] public void IriExpression_discards_characters_following_unbalanced_parentheses() {
			var match = EmdGrammar.IriLiteralExpression.ShouldMatchSomeOf("http://msdn.microsoft.com/en-us/library/a6td98xev=vs.71).aspx");

			match.Product.Value.Should().Be("http://msdn.microsoft.com/en-us/library/a6td98xev=vs.71");
		}

		[Fact] public void IriLiteral_should_match_the_canonical_example_idn() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://bücher.de/");
		}

		[Fact] public void IriLiteral_should_match_custom_irn() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("my:irn:scheme:24");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_ending_in_slash() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_with_path() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/some/path");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_with_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca?q=bla");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_with_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca#fragment");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_with_path_and_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/some/path?q=bla");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_with_path_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/some/path#fragment");
		}

		[Fact] public void IriLiteral_should_match_scheme_absolute_iri_with_path_query_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/some/path?q=bla#fragment");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_ending_in_slash() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca/");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_with_path() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca/some/path");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_with_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca?q=bla");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_with_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca#fragment");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_with_path_and_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca/some/path?q=bla");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_with_path_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca/some/path#fragment");
		}

		[Fact] public void IriLiteral_should_match_scheme_relative_iri_with_path_query_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("//www.cracknell.ca/some/path?q=bla#fragment");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_ending_in_slash() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca/");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_with_path() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca/some/path");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_with_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca?q=bla");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_with_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca#fragment");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_with_path_and_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca/some/path?q=bla");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_with_path_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca/some/path#fragment");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_with_path_query_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca/some/path?q=bla#fragment");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_regname() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_regname_user() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("james@www.cracknell.ca");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_regname_port() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("www.cracknell.ca:80");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_regname_user_and_port() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("james@www.cracknell.ca:80");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv4_address() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("123.234.210.10");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv4_address_user() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("james@123.234.210.10");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv4_address_port() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("123.234.210.10:80");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv4_address_user_and_port() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("james@123.234.210.10:80");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv6_address() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[FEDC:BA98:7654:3210:0123:4567:89AB:CDEF]");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv6_address_user() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("james@[FEDC:BA98:7654:3210:0123:4567:89AB:CDEF]");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv6_address_port() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[FEDC:BA98:7654:3210:0123:4567:89AB:CDEF]:80");
		}

		[Fact] public void IriLiteral_should_match_schemeless_iri_authority_IPv6_address_user_and_port() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("james@[FEDC:BA98:7654:3210:0123:4567:89AB:CDEF]:80");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_root() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_with_path() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/some/path");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_ending_in_slash() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/some/path/");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_with_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/?q=bla");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_with_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/#fragment");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_with_path_and_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/some/path?q=bla");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_with_path_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/some/path#fragment");
		}

		[Fact] public void IriLiteral_should_match_absolute_path_with_path_query_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("/some/path?q=bla#fragment");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_no_separator() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("path");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_path() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("some/path");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("?q=bla");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("#fragment");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_path_and_query() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("some/path?q=bla");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_path_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("some/path#fragment");
		}

		[Fact] public void IriLiteral_should_match_relative_path_with_path_query_and_fragment() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("some/path?q=bla#fragment");
		}

		[Fact] public void IriLiteral_should_match_when_used_as_an_object_literal_property_name() {
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("prop:")
			.Length.Should().Be(4);
		}

		[Fact] public void IriLiteral_should_not_end_with_a_comma() {
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("http://www.cracknell.ca/a,");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/a,a");
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("http://www.cracknell.ca?q=a,");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca?q=a,a");
		}

		[Fact] public void IriLiteral_should_not_end_with_a_colon() {
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("http://www.cracknell.ca/a:");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/a:a");
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("http://www.cracknell.ca?q=a:");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca?q=a:a");
		}

		[Fact] public void IriLiteral_should_not_end_with_a_semicolon() {
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("http://www.cracknell.ca/a;");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca/a;a");
			EmdGrammar.IriLiteral.ShouldMatchSomeOf("http://www.cracknell.ca?q=a;");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://www.cracknell.ca?q=a;a");
		}

		[Fact] public void IriLiteral_should_match_example_IPv6_urls_from_RFC_2732() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:80/index.html");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[1080:0:0:0:8:800:200C:417A]/index.html");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[3ffe:2a00:100:7031::1]");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[1080::8:800:200C:417A]/foo");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[::192.9.5.5]/ipng");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[::FFFF:129.144.52.38]:80/index.html");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("http://[2010:836B:4179::836B:4179]");
		}

		[Fact] public void IriLiteral_should_match_example_IPv6_urls_from_RFC_2732_with_no_scheme() {
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:80/index.html");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[1080:0:0:0:8:800:200C:417A]/index.html");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[3ffe:2a00:100:7031::1]");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[1080::8:800:200C:417A]/foo");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[::192.9.5.5]/ipng");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[::FFFF:129.144.52.38]:80/index.html");
			EmdGrammar.IriLiteral.ShouldMatchAllOf("[2010:836B:4179::836B:4179]");
		}

		[Fact] public void IPv6Address_should_match_abcd_abcd_abcd_abcd_abcd_abcd_abcd_abcd() {
			EmdGrammar.IPv6Address.ShouldMatchAllOf("abcd:abcd:abcd:abcd:abcd:abcd:abcd:abcd");
		}

		[Fact] public void IPv4Address_should_match_123_123_123_123() {
			EmdGrammar.IPv4Address.ShouldMatchAllOf("123.123.123.123");
		}

		[Fact] public void IPv4Address_should_match_1_1_1_1() {
			EmdGrammar.IPv4Address.ShouldMatchAllOf("1.1.1.1");
		}

		[Fact] public void IPv4Address_should_not_match_0_0_0_0() {
			EmdGrammar.IPv4Address.ShouldMatchAllOf("0.0.0.0");
		}
	}
}
