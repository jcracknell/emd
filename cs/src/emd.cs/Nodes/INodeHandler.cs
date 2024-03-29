﻿using emd.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
  public interface INodeHandler {
    void Handle(AutoLinkNode node);
    void Handle(BlockquoteNode node);
    void Handle(CodeNode node);
    void Handle(DefinitionListNode node);
    void Handle(DocumentNode node);
    void Handle(EmphasisNode node);
    void Handle(EntityNode node);
    void Handle(ExpressionBlockNode node);
    void Handle(HeadingNode node);
    void Handle(InlineExpressionNode node);
    void Handle(LineBreakNode node);
    void Handle(LinkNode node);
    void Handle(OrderedListNode node);
    void Handle(ParagraphNode node);
    void Handle(QuotedNode node);
    void Handle(ReferenceNode node);
    void Handle(SpaceNode node);
    void Handle(StrongNode node);
    void Handle(SymbolNode node);
    void Handle(TableNode node);
    void Handle(TextNode node);
    void Handle(UnorderedListNode node);
  }

  public interface INodeHandler<T> {
    T Handle(AutoLinkNode node);
    T Handle(BlockquoteNode node);
    T Handle(CodeNode node);
    T Handle(DefinitionListNode node);
    T Handle(DocumentNode node);
    T Handle(EmphasisNode node);
    T Handle(EntityNode node);
    T Handle(ExpressionBlockNode node);
    T Handle(HeadingNode node);
    T Handle(InlineExpressionNode node);
    T Handle(LineBreakNode node);
    T Handle(LinkNode node);
    T Handle(OrderedListNode node);
    T Handle(ParagraphNode node);
    T Handle(QuotedNode node);
    T Handle(ReferenceNode node);
    T Handle(SpaceNode node);
    T Handle(StrongNode node);
    T Handle(SymbolNode node);
    T Handle(TableNode node);
    T Handle(TextNode node);
    T Handle(UnorderedListNode node);
  }
}
