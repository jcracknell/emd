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

			#region Helpers

			private void WriteIndent() {
				for(var i = 0; i < _indentLevel; i++)
					_writer.Write(INDENT);
			}

			private void WriteComposite(object o, object[] attrs, Action content) {
				if(null == o) throw ExceptionBecause.ArgumentNull(() => o);
				if(null == attrs) throw ExceptionBecause.ArgumentNull(() => attrs);
				if(null == content) throw ExceptionBecause.ArgumentNull(() => content);

				if(o is INode) WriteSourceRange((o as INode).SourceRange);
				if(o is IExpression) WriteSourceRange((o as IExpression).SourceRange);

				WriteIndent();
				_writer.Write("(");
				WriteType(o);
				WriteAttrs(attrs);
				_writer.WriteLine();

				_indentLevel++;
				content();
				_indentLevel--;

				WriteIndent();
				_writer.WriteLine(")");
			}

			private void WriteComposite(object o, Action content) {
				WriteComposite(o, new object[0], content);
			}

			private void WriteComposite(object o, object attr1, Action content) {
				WriteComposite(o, new object[] { attr1 }, content);
			}

			private void WriteComposite(object o, object attr1, object attr2, Action content) {
				WriteComposite(o, new object[] { attr1, attr2 }, content);
			}

			private void WriteComposite(object o, object attr1, object attr2, object attr3, Action content) {
				WriteComposite(o, new object[] { attr1, attr2, attr3 }, content);
			}

			private void WriteComposite(object o, object attr1, object attr2, object attr3, object attr4, Action content) {
				WriteComposite(o, new object[] { attr1, attr2, attr3, attr4 }, content);
			}

			private void WriteAtomic(object o, params object[] attrs) {
				if(null == o) throw ExceptionBecause.ArgumentNull(() => o);
				if(null == attrs) throw ExceptionBecause.ArgumentNull(() => attrs);

				WriteIndent();
				_writer.Write("(");
				WriteType(o);
				WriteAttrs(attrs);
				_writer.WriteLine(")");
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

			private void WriteAttrs(object[] values) {
				foreach(var value in values) {
					_writer.Write(" ");
					_writer.Write(
						null == value ? "null" :
						typeof(string).Equals(value.GetType()) ? StringUtils.LiteralEncode(value.ToString()) :
						value.ToString()
					);
				}
			}

			private void WriteSourceRange(pegleg.cs.Parsing.SourceRange sourceRange) {
				WriteIndent();
				_writer.Write("# Line:");
				_writer.Write(sourceRange.Line);
				_writer.Write(" Character:");
				_writer.WriteLine(sourceRange.LineIndex);
			}

			private void WriteBinaryExpression(BinaryExpression expression) {
				WriteComposite(expression, () => {
					expression.Left.HandleWith(this);
					expression.Right.HandleWith(this);
				});
			}

			private void WriteUnaryExpression(UnaryExpression expression) {
				WriteComposite(expression, () => {
					expression.Body.HandleWith(this);
				});
			}

			#endregion

			#region INodeHandler members

			public void Handle(AutoLinkNode node) {
				WriteComposite(node, node.Uri, () => {
					node.Arguments.Each(argument => argument.HandleWith(this));
				});
			}

			public void Handle(BlockquoteNode node) {
				WriteComposite(node, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(CodeNode node) {
				WriteAtomic(node, node.Text);
			}

			public void Handle(DefinitionListDefinitionNode node) {
				WriteComposite(node, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(DefinitionListNode node) {
				WriteComposite(node, () => {
					node.Items.Each(item => {
						WriteComposite(item, () => {
							WriteComposite(item.Term, () => {
								item.Term.Children.Each(c => c.HandleWith(this));
							});
							item.Definitions.Each(def => {
								WriteComposite(def, () => {
									def.Children.Each(c => c.HandleWith(this));
								});
							});
						});
					});
				});
			}

			public void Handle(EmphasisNode node) {
				WriteComposite(node, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(EntityNode node) {
				WriteAtomic(node, node.Value);
			}

			public void Handle(ExpressionBlockNode node) {
				WriteComposite(node, () => {
					node.Expression.HandleWith(this);
				});
			}

			public void Handle(HeadingNode node) {
				WriteAtomic(node, node.Level, node.Text);
			}

			public void Handle(InlineExpressionNode node) {
				WriteComposite(node, () => {
					node.Expression.HandleWith(this);
				});
			}

			public void Handle(LineBreakNode node) {
				WriteAtomic(node);
			}

			public void Handle(LinkNode node) {
				WriteComposite(node, node.ReferenceId, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(DocumentNode node) {
				WriteComposite(node, () => {
					node.Content.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(OrderedListNode node) {
				WriteComposite(node, node.CounterStyle, node.Start, node.SeparatorStyle, () => {
					node.Items.Each(item => {
						WriteComposite(item, () => {
							item.Children.Each(c => c.HandleWith(this));
						});
					});
				});
			}

			public void Handle(ParagraphNode node) {
				WriteComposite(node, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(QuotedNode node) {
				WriteComposite(node, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(ReferenceNode node) {
				WriteComposite(node, node.ReferenceId.Value, () => {
					node.Arguments.Each(argument => argument.HandleWith(this));
				});
			}

			public void Handle(SpaceNode node) {
				WriteAtomic(node);
			}

			public void Handle(StrongNode node) {
				WriteComposite(node, () => {
					node.Children.Each(child => child.HandleWith(this));
				});
			}

			public void Handle(SymbolNode node) {
				WriteAtomic(node, node.Symbol);
			}

			public void Handle(TableNode node) {
				WriteComposite(node, () => {
					node.Rows.Each(row => {
						WriteComposite(row, () => {
							row.Cells.Each(cell => {
								WriteComposite(cell, cell.Kind, cell.ColumnSpan, cell.RowSpan, () => {
									cell.Children.Each(c => c.HandleWith(this));
								});
							});
						});
					});
				});
			}

			public void Handle(TextNode node) {
				WriteAtomic(node, node.Text);
			}

			public void Handle(UnorderedListNode node) {
				WriteComposite(node, () => {
					node.Items.Each(item => {
						WriteComposite(item, () => {
							item.Children.Each(c => c.HandleWith(this));
						});
					});
				});
			}

			#endregion

			#region IExpressionHandler members

			public void Handle(AdditionExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(ArrayLiteralExpression expression) {
				WriteComposite(expression, () => {
					expression.Elements.Each(element => element.HandleWith(this));
				});
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
				WriteAtomic(expression, expression.Value);
			}

			public void Handle(CallExpression expression) {
				WriteComposite(expression, () => {
					expression.Body.HandleWith(this);
					expression.Arguments.Each(argument => argument.HandleWith(this));
				});
			}

			public void Handle(ConditionalExpression expression) {
				WriteComposite(expression, () => {
					expression.Condition.HandleWith(this);
					expression.TrueExpression.HandleWith(this);
					expression.FalseExpression.HandleWith(this);
				});
			}

			public void Handle(DeleteExpression expression) {
				WriteUnaryExpression(expression);
			}

			public void Handle(DivisionExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(DocumentLiteralExpression expression) {
				WriteComposite(expression, () => {
					expression.Content.Each(node => node.HandleWith(this));
				});
			}

			public void Handle(DynamicPropertyExpression expression) {
				WriteComposite(expression, () => {
					expression.Body.HandleWith(this);
					expression.MemberName.HandleWith(this);
				});
			}

			public void Handle(ElidedExpression expression) {
				WriteAtomic(expression);
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
				WriteAtomic(expression, expression.Name);
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
				WriteAtomic(expression);
			}

			public void Handle(NumericLiteralExpression expression) {
				WriteAtomic(expression, expression.Value);
			}

			public void Handle(ObjectLiteralExpression expression) {
				WriteComposite(expression, () => {
					expression.PropertyAssignments.Each(propertyAssignment => {
						WriteComposite(propertyAssignment, propertyAssignment.PropertyName, () => {
							propertyAssignment.PropertyValue.HandleWith(this);
						});
					});
				});
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
				WriteComposite(expression, expression.MemberName, () => {
					expression.Body.HandleWith(this);
				});
			}

			public void Handle(StrictEqualsExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(StrictNotEqualsExpression expression) {
				WriteBinaryExpression(expression);
			}

			public void Handle(StringLiteralExpression expression) {
				WriteAtomic(expression, expression.Value);
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
				WriteAtomic(expression, expression.Value);
			}

			public void Handle(VoidExpression expression) {
				WriteUnaryExpression(expression);
			}

			#endregion
		}
	}
}
