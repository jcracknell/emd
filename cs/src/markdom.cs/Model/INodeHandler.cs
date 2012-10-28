using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model{
	public interface INodeHandler {
		void Handle(AutoLinkNode node);
		void Handle(EmphasisNode node);
		void Handle(EntityNode node);
		void Handle(HeadingNode node);
		void Handle(InlineExpressionNode node);
		void Handle(LineBreakNode node);
		void Handle(LinkNode node);
		void Handle(OrderedListNode node);
		void Handle(OrderedListItemNode node);
		void Handle(ParagraphNode node);
		void Handle(QuotedNode node);
		void Handle(SpaceNode node);
		void Handle(StrongNode node);
		void Handle(SymbolNode node);
		void Handle(TableNode node);
		void Handle(TableDataCellNode node);
		void Handle(TableHeaderCellNode node);
		void Handle(TableRowNode node);
		void Handle(TextNode node);
		void Handle(UnorderedListNode node);
		void Handle(UnorderedListItemNode node);
	}

	public interface INodeHandler<T> {
		T Handle(AutoLinkNode node);
		T Handle(EmphasisNode node);
		T Handle(EntityNode node);
		T Handle(HeadingNode node);
		T Handle(InlineExpressionNode node);
		T Handle(LineBreakNode node);
		T Handle(LinkNode node);
		T Handle(OrderedListNode node);
		T Handle(OrderedListItemNode node);
		T Handle(ParagraphNode node);
		T Handle(QuotedNode node);
		T Handle(SpaceNode node);
		T Handle(StrongNode node);
		T Handle(SymbolNode node);
		T Handle(TableNode node);
		T Handle(TableDataCellNode node);
		T Handle(TableHeaderCellNode node);
		T Handle(TableRowNode node);
		T Handle(TextNode node);
		T Handle(UnorderedListNode node);
		T Handle(UnorderedListItemNode node);
	}
}
