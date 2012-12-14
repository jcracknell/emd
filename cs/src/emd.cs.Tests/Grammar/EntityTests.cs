using FluentAssertions;
using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace emd.cs.Grammar
{
	public class EntityTests : GrammarTestFixture {
		[Fact] public void Entity_matches_decimal_entity() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\#233;");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0, 6, 1, 0))
			);
		}

		[Fact] public void Entity_matches_u_hexadecimal_entity() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\uE9");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,4,1,0))
			);
		}

		[Fact] public void Entity_matches_u_hexadecimal_entity_with_optional_leading_hash() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\#uE9");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,5,1,0))
			);
		}

		[Fact] public void Entity_matches_u_hexadecimal_entity_with_optional_trailing_semicolon() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\uE9;");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,5,1,0))
			);
		}

		[Fact] public void Entity_matches_x_hexadecimal_entity() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\xE9");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,4,1,0))
			);
		}

		[Fact] public void Entity_matches_x_hexadecimal_entity_with_optional_leading_hash() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\#xE9");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,5,1,0))
			);
		}

		[Fact] public void Entity_matches_x_hexadecimal_entity_with_optional_trailing_semicolon() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\xE9;");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,5,1,0))
			);
		}

		[Fact] public void Entity_matches_named_entity() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\eacute");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,7,1,0))
			);
		}

		[Fact] public void Entity_matches_named_entity_with_optional_trailing_semicolon() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\eacute;");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u00e9", new SourceRange(0,8,1,0))
			);
		}

		[Fact] public void Entity_matches_named_entity_beginning_with_u() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\uring");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u016f", new SourceRange(0,6,1,0))
			);
		}

		[Fact] public void Entity_matches_named_entity_beginning_with_x() {
			var match = EmdGrammar.Entity.ShouldMatch(@"\xi");

			match.Product.ShouldBeEquivalentTo(
				new EntityNode("\u03be", new SourceRange(0,3,1,0))
			);
		}
	}
}
