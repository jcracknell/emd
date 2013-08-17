using emd.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Conversion {
  public class ReferenceExtractingNodeHandler : INodeHandler<IEnumerable<ReferenceNode>> {
    public IEnumerable<ReferenceNode> Handle(AutoLinkNode node) {
      return Enumerable.Empty<ReferenceNode>();
    }

    public IEnumerable<ReferenceNode> Handle(CodeNode node) {
      return Enumerable.Empty<ReferenceNode>();
    }

    public IEnumerable<ReferenceNode> Handle(DefinitionListNode node) {
      return node.Items.SelectMany(item =>
        item.Term.Children.SelectMany(c => c.HandleWith(this))
        .Concat(item.Definitions.SelectMany(d => d.Children.SelectMany(c => c.HandleWith(this))))
      );
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

    public IEnumerable<ReferenceNode> Handle(DocumentNode node) {
      return node.Content.SelectMany(content => content.HandleWith(this));
    }

    public IEnumerable<ReferenceNode> Handle(OrderedListNode node) {
      return node.Items.SelectMany(item => item.Children.SelectMany(c => c.HandleWith(this)));
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
    }

    public IEnumerable<ReferenceNode> Handle(SymbolNode node) {
      return Enumerable.Empty<ReferenceNode>();
    }

    public IEnumerable<ReferenceNode> Handle(TableNode node) {
      return node.Rows.SelectMany(r => r.Cells.SelectMany(c => c.Children.SelectMany(x => x.HandleWith(this))));
    }

    public IEnumerable<ReferenceNode> Handle(TextNode node) {
      return Enumerable.Empty<ReferenceNode>();
    }

    public IEnumerable<ReferenceNode> Handle(UnorderedListNode node) {
      return node.Items.SelectMany(i => i.Children.SelectMany(c => c.HandleWith(this)));
    }

    public IEnumerable<ReferenceNode> Handle(BlockquoteNode node) {
      return node.Children.SelectMany(item => item.HandleWith(this));
    }

    public IEnumerable<ReferenceNode> Handle(ExpressionBlockNode node) {
      return Enumerable.Empty<ReferenceNode>();
    }
  }
}
