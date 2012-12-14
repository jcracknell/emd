using emd.cs.Expressions;
using emd.cs.Nodes;
using pegleg.cs.Utils;
using System;
using System.IO;
using System.Text;

namespace emd.cs.Conversion {
	public class StructuralRenderer : IRenderer {
		private const string INDENT = "\t";

		public void Render(INode node, Stream ostream) {
			if(null == node) throw ExceptionBecause.ArgumentNull(() => node);

			using(var writer = new StreamWriter(ostream, new System.Text.UTF8Encoding(false)))
				node.HandleWith(new Handler(writer));
		}

		public class Handler : INodeHandler, IExpressionHandler {
			private readonly TextWriter _writer;
			private int _indentLevel = 0;

			public Handler(TextWriter writer) {
				if(null == writer) throw new ArgumentNullException("writer");

				_writer = writer;
			}

			private void WriteIndent() {
				for(var i = 0; i < _indentLevel; i++)
					_writer.Write(INDENT);
			}

			private void WriteType(object o) {
				var typeName = o.GetType().Name;
				if(typeName.EndsWith("Node")) {
					_writer.Write(typeName.Substring(0, typeName.Length - "Node".Length));
				} else if(typeName.EndsWith("Expression")) {
					_writer.Write(typeName.Substring(0, typeName.Length - "Expression".Length));
				} else {
					_writer.Write(typeName);
				}
			}

			private void WriteStart(object node, params object[] values) {
				WriteIndent();
				_writer.Write("\\");
				WriteType(node);
				WriteNodeValues(values);
				_writer.WriteLine();
				_indentLevel++;
			}

			private void WriteEnd(object node) {
				_indentLevel--;
				WriteIndent();
				_writer.Write("/");
				WriteType(node);
				_writer.WriteLine();
			}

			private void WriteNodeValues(object[] values) {
				foreach(var value in values) {
					_writer.Write(" ");
					_writer.Write(
						null == value ? "null" :
						typeof(string).Equals(value.GetType()) ? StringUtils.LiteralEncode(value.ToString()) :
						value.ToString()
					);
				}
			}

			private void WriteValue(object node, params object[] values) {
				WriteIndent();
				WriteType(node);
				WriteNodeValues(values);
				_writer.WriteLine();
			}

			private void WriteBinaryExpression(BinaryExpression expression) {
				WriteStart(expression);
				expression.Left.HandleWith(this);
				expression.Right.HandleWith(this);
				WriteEnd(expression);
			}

			private void WriteUnaryExpression(UnaryExpression expression) {
				WriteStart(expression);
				expression.Body.HandleWith(this);
				WriteEnd(expression);
			}

			#region INodeHandler members

			public void Handle(AutoLinkNode node) {
				WriteStart(node, node.Uri);
				node.Arguments.Each(argument => argument.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(BlockquoteNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(CodeNode node) {
				WriteValue(node, node.Text);
			}

			public void Handle(EmphasisNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(EntityNode node) {
				WriteValue(node, node.Value);
			}

			public void Handle(ExpressionBlockNode node) {
				WriteStart(node);
				node.Expression.HandleWith(this);
				WriteEnd(node);
			}

			public void Handle(HeadingNode node) {
				WriteValue(node, node.Level, node.Text);
			}

			public void Handle(InlineExpressionNode node) {
				WriteStart(node);
				node.Expression.HandleWith(this);
				WriteEnd(node);
			}

			public void Handle(LineBreakNode node) {
				WriteValue(node);
			}

			public void Handle(LinkNode node) {
				WriteStart(node, node.ReferenceId);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(DocumentNode node) {
				WriteStart(node);
				node.Content.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(OrderedListNode node) {
				WriteStart(node, node.CounterStyle, node.Start, node.SeparatorStyle);
				node.Items.Each(item => item.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(OrderedListItemNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(ParagraphNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(QuotedNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(ReferenceNode node) {
				WriteStart(node, node.ReferenceId.Value);
				node.Arguments.Each(argument => argument.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(SpaceNode node) {
				WriteValue(node);
			}

			public void Handle(StrongNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(SymbolNode node) {
				WriteValue(node, node.Symbol);
			}

			public void Handle(TableNode node) {
				WriteStart(node);
				node.Rows.Each(row => row.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(TableDataCellNode node) {
				WriteStart(node, node.ColumnSpan, node.RowSpan);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(TableHeaderCellNode node) {
				WriteStart(node, node.ColumnSpan, node.RowSpan);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(TableRowNode node) {
				WriteStart(node);
				node.Cells.Each(cell => cell.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(TextNode node) {
				WriteValue(node, node.Text);
			}

			public void Handle(UnorderedListNode node) {
				WriteStart(node);
				node.Items.Each(item => item.HandleWith(this));
				WriteEnd(node);
			}

			public void Handle(UnorderedListItemNode node) {
				WriteStart(node);
				node.Children.Each(child => child.HandleWith(this));
				WriteEnd(node);
			}

			#endregion

			#region IExpressionHandler members

			public void Handle(AdditionExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(ArrayLiteralExpression expression) {
				WriteStart(expression);

				expression.Elements.Each(element => {
					if(null == element)
						_writer.WriteLine(":ElidedExpression");
					else
						element.HandleWith(this);
				});

				WriteEnd(expression);
			}

			public void Handle(BitwiseAndExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(BitwiseNotExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(BitwiseOrExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(BitwiseXOrExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(BooleanLiteralExpression expression) {
				WriteValue(expression, expression.Value);
			}

			public void Handle(CallExpression expression) {
				WriteStart(expression);
				expression.Body.HandleWith(this);
				expression.Arguments.Each(argument => argument.HandleWith(this));
				WriteEnd(expression);
			}

			public void Handle(ConditionalExpression expression) {
				WriteStart(expression);
				expression.Condition.HandleWith(this);
				expression.TrueExpression.HandleWith(this);
				expression.FalseExpression.HandleWith(this);
				WriteEnd(expression);
			}

			public void Handle(DeleteExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(DivisionExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(DocumentLiteralExpression expression) {
				WriteStart(expression);
				expression.Content.Each(node => node.HandleWith(this));
				WriteEnd(expression);
			}

			public void Handle(DynamicPropertyExpression expression) {
				WriteStart(expression);
				expression.Body.HandleWith(this);
				expression.MemberName.HandleWith(this);
				WriteEnd(expression);
			}

			public void Handle(ElidedExpression expression) {
				WriteValue(expression);
			}

			public void Handle(EqualsExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(GreaterThanExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(GreaterThanOrEqualToExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(IdentifierExpression expression) {
				WriteValue(expression, expression.Name);
			}

			public void Handle(InExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(InstanceOfExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(LeftShiftExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(LessThanExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(LessThanOrEqualToExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(LogicalAndExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(LogicalNotExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(LogicalOrExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(ModuloExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(MultiplicationExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(NegativeExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(NotEqualsExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(NullLiteralExpression expression) {
				WriteValue(expression);
			}

			public void Handle(NumericLiteralExpression expression) {
				WriteValue(expression, expression.Value);
			}

			public void Handle(ObjectLiteralExpression expression) {
				WriteStart(expression);
				expression.PropertyAssignments.Each(propertyAssignment => {
					WriteStart(propertyAssignment, propertyAssignment.PropertyName);
					propertyAssignment.PropertyValue.HandleWith(this);
					WriteEnd(propertyAssignment);
				});
				WriteEnd(expression);
			}

			public void Handle(PositiveExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(PostfixDecrementExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(PostfixIncrementExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(PrefixDecrementExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(PrefixIncrementExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(RightShiftExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(StaticPropertyExpression expression) {
				WriteStart(expression, expression.MemberName);
				expression.Body.HandleWith(this);
				WriteEnd(expression);
			}

			public void Handle(StrictEqualsExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(StrictNotEqualsExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(StringLiteralExpression expression) {
				WriteValue(expression, expression.Value);
			}

			public void Handle(SubtractionExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(TypeofExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(UnsignedRightShiftExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(UriLiteralExpression expression) {
				WriteValue(expression, expression.Value);
			}

			public void Handle(VoidExpression expression) {
				WriteUnaryExpression(expression);
			}

			#endregion
		}
	}
}
