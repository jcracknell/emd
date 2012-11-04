using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Conversion {
	public class ReferenceExtractingNodeHandler : INodeHandler<IEnumerable<ReferenceNode>> {
		public IEnumerable<ReferenceNode> Handle(AutoLinkNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(EmphasisNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(EntityNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(HeadingNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(InlineExpressionNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(LineBreakNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(LinkNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(MarkdomDocumentNode node) {
			return node.Content.SelectMany(content => content.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(OrderedListNode node) {
			return node.Items.SelectMany(item => item.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(OrderedListItemNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(ParagraphNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(QuotedNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(ReferenceNode node) {
			yield return node;
		}

		public IEnumerable<ReferenceNode> Handle(SpaceNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(StrongNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
			throw new NotImplementedException();
		}

		public IEnumerable<ReferenceNode> Handle(SymbolNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(TableNode node) {
			return node.Rows.SelectMany(row => row.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(TableDataCellNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(TableHeaderCellNode node) {
			return node.Children.SelectMany(child => child.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(TableRowNode node) {
			return node.Cells.SelectMany(cell => cell.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(TextNode node) {
			return Enumerable.Empty<ReferenceNode>();
		}

		public IEnumerable<ReferenceNode> Handle(UnorderedListNode node) {
			return node.Items.SelectMany(item => item.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(UnorderedListItemNode node) {
			return node.Children.SelectMany(item => item.HandleWith(this));
		}

		public IEnumerable<ReferenceNode> Handle(BlockquoteNode node) {
			return node.Children.SelectMany(item => item.HandleWith(this));
		}
	}
}
