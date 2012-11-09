using FluentAssertions;
using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace markdom.cs.Grammar
{
	public class EntityTests : GrammarTestFixture {
		[Fact] public void Entity_matches_decimal_html_entity() {
			var expected = new EntityNode(233, new MarkdomSourceRange(0, 6, 1, 0));

			var match = Grammar.Entity.ShouldMatch("&#233;");

			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Entity_matches_hexadecimal_html_entity() {
			var expected = new EntityNode(233, new MarkdomSourceRange(0, 6, 1, 0));

			var match = Grammar.Entity.ShouldMatch("&#xE9;");

			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}

		[Fact] public void Entity_matches_named_html_entity() {
			var expected = new EntityNode(233, new MarkdomSourceRange(0, 8, 1, 0));

			var match = Grammar.Entity.ShouldMatch("&eacute;");

			match.Succeeded.Should().BeTrue();
			match.Product.ShouldBeEquivalentTo(expected);
		}
	}
}
