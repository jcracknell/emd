using emd.cs.Expressions;
using emd.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace emd.cs.Conversion.Html {
	public class XmlWritingNodeHandler : INodeHandler {
		private readonly XmlWriter _writer;
		private readonly ReferenceCollection _references;

		public XmlWritingNodeHandler(XmlWriter writer, ReferenceCollection references) {
			if(null == writer) throw ExceptionBecause.ArgumentNull(() => writer);
			if(null == references) throw ExceptionBecause.ArgumentNull(() => references);

			_writer = writer;
			_references = references;
		}

		#region Xml Writing Helpers

		protected void Write(string elementName, object attributes, Action body) {
			_writer.WriteStartElement(elementName);

			if(null != attributes)
			foreach(var property in attributes.GetType().GetProperties()) {
				var propertyName = property.Name;
				if('@' == propertyName[0])
					propertyName = propertyName.Substring(1);

				var propertyValue = property.GetValue(attributes, null);

				if(null == propertyValue) continue;
				if(typeof(bool).Equals(propertyValue.GetType())) {
					if(true == (bool)propertyValue)
						_writer.WriteAttributeString(propertyName, null);
				} else {
					_writer.WriteAttributeString(propertyName, propertyValue.ToString());
				}
			}

			if(null != body)
				body();

			_writer.WriteEndElement();
		}

		protected void Write(string elementName, Action body) {
			Write(elementName, null, body);
		}

		protected void Write(string elementName) {
			Write(elementName, null, null);
		}

		protected void WriteContent(string content) {
			_writer.WriteString(content);
		}

		#endregion

		public void Handle(AutoLinkNode node) {
			Write("a", new { href = node.Uri }, () => {
				WriteContent(node.Uri);
			});
		}

		public void Handle(BlockquoteNode node) {
			Write("blockquote", () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(CodeNode node) {
			Write("code", () => {
				WriteContent(node.Text);
			});
		}

		public void Handle(EmphasisNode node) {
			Write("em", () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(EntityNode node) {
			_writer.WriteString(node.Value);
		}

		public void Handle(ExpressionBlockNode node) {
			// TODO: Expression evaluation
			Write("div", () => {
				WriteContent("{EXPRESSION BLOCK}");
			});
		}

		public void Handle(HeadingNode node) {
			if(node.Level <= 6) {
				Write("h" + node.Level, () => {
					WriteContent(node.Text);
				});
			} else {
				Write("div", new { @class = "heading-" + node.Level }, () => {
					WriteContent(node.Text);
				});
			}
		}

		public void Handle(InlineExpressionNode node) {
			// TODO: Expression evaluation
			Write("span", () => {
				WriteContent("{INLINE EXPRESSION}");
			});
		}

		public void Handle(LineBreakNode node) {
			Write("br");
		}

		public void Handle(LinkNode node) {
			// TODO: Expression evaluation
			string href = null;
			Write("a", new { href = href }, () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(MarkdomDocumentNode node) {
			node.Content.Each(c => c.HandleWith(this));
		}

		public void Handle(OrderedListNode node) {
			Write("ol", new { start = node.Start }, () => {
				node.Items.Each(i => i.HandleWith(this));
			});
		}

		public void Handle(OrderedListItemNode node) {
			Write("li", () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(ParagraphNode node) {
			Write("p", () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(QuotedNode node) {
			WriteContent(QuoteType.Double == node.QuoteType ? "\u201c" : "\u2018");
			node.Children.Each(c => c.HandleWith(this));
			WriteContent(QuoteType.Double == node.QuoteType ? "\u201d" : "\u2019");
		}

		public void Handle(ReferenceNode node) { }

		public void Handle(SpaceNode node) {
			WriteContent(" ");
		}

		public void Handle(StrongNode node) {
			Write("strong", () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(SymbolNode node) {
			WriteContent(node.Symbol);
		}

		public void Handle(TableNode node) {
			Write("table", () => {
				node.Rows.Each(r => r.HandleWith(this));
			});
		}

		public void Handle(TableDataCellNode node) {
			Write("td", new { colspan = node.ColumnSpan, rowspan = node.RowSpan }, () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(TableHeaderCellNode node) {
			Write("th", new { colspan = node.ColumnSpan, rowspan = node.RowSpan }, () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(TableRowNode node) {
			Write("tr", () => {
				node.Cells.Each(c => c.HandleWith(this));
			});
		}

		public void Handle(TextNode node) {
			WriteContent(node.Text);
		}

		public void Handle(UnorderedListNode node) {
			Write("ul", () => {
				node.Items.Each(i => i.HandleWith(this));
			});
		}

		public void Handle(UnorderedListItemNode node) {
			Write("li", () => {
				node.Children.Each(c => c.HandleWith(this));
			});
		}
	}
}
