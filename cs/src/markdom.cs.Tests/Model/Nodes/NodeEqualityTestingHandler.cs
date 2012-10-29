using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Model.Nodes {
	public class NodeEqualityTestingHandler : INodeHandler {
		private readonly INode _expected;

		public NodeEqualityTestingHandler(INode expected) {
			if(null == expected) throw new ArgumentNullException("expected");

			_expected = expected;
		}

		private void AssertNodeIsAsExpected(INode actual) {
			if(_expected == actual)
				Assert.Fail("This IS the expected node you ass.");

			Assert.AreEqual(_expected.GetType(), actual.GetType(),
				"Node type does not match the expected node type.");

			var publicProperties = actual.GetType().GetProperties();
			foreach(var publicProperty in publicProperties)
				AssertPropertyIsAsExpected(actual, publicProperty);
		}

		private void AssertPropertyIsAsExpected(INode actual, PropertyInfo property) {
			if(ReflectionHelpers.IsNodeClassOrInterface(property.PropertyType)) {
				AssertNodePropertyIsAsExpected(actual, property);
			} else if(ReflectionHelpers.IsEnumerableNodeClassOrInterface(property.PropertyType)) {
				AssertEnumerableNodePropertyIsAsExpected(actual, property);
			} else {
				AssertNonNodePropertyIsAsExpected(actual, property);
			}
		}

		private void AssertNodePropertyIsAsExpected(INode actual, PropertyInfo property) {
			var expectedNode = (INode)property.GetValue(_expected);
			var actualNode = (INode)property.GetValue(actual);

			actualNode.HandleWith(new NodeEqualityTestingHandler(expectedNode));
		}

		private void AssertEnumerableNodePropertyIsAsExpected(INode actual, PropertyInfo property) {
			var expectedNodeEnumerable = (IEnumerable<INode>)property.GetValue(_expected);
			var actualNodeEnumerable = (IEnumerable<INode>)property.GetValue(actual);
			
			var expectedEnumerator = expectedNodeEnumerable.GetEnumerator();
			var actualEnumerator = actualNodeEnumerable.GetEnumerator();
			while(expectedEnumerator.MoveNext()) {
				Assert.IsTrue(actualEnumerator.MoveNext(), "Expected additional nodes.");
				actualEnumerator.Current.HandleWith(new NodeEqualityTestingHandler(expectedEnumerator.Current));
			}

			Assert.IsFalse(actualEnumerator.MoveNext(), "Expected end of node sequence.");
		}

		private void AssertNonNodePropertyIsAsExpected(INode actual, PropertyInfo property) {
			var expectedPropertyValue = property.GetValue(_expected);
			var actualPropertyValue = property.GetValue(actual);

			if(TypeEqualityIsVerifiable(property.PropertyType)) {
				Assert.AreEqual(expectedPropertyValue, actualPropertyValue);			
				return;
			} 

			var propertyTypeEnumerableImplementation =
				new Type[] { property.PropertyType }.Concat(property.PropertyType.GetInterfaces())
				.SingleOrDefault(i => i.IsInterface && i.IsGenericType && typeof(IEnumerable<>).Equals(i.GetGenericTypeDefinition()));

			if(null != propertyTypeEnumerableImplementation) {
				var expectedEnumerable = (IEnumerable<object>)expectedPropertyValue;
				var actualEnumerable = (IEnumerable<object>)actualPropertyValue;

				if(TypeEqualityIsVerifiable(propertyTypeEnumerableImplementation.GetGenericArguments()[0])) {
					Assert.IsTrue(Enumerable.SequenceEqual(expectedEnumerable, actualEnumerable));
					return;
				} else {
					Assert.AreEqual(expectedEnumerable.Count(), actualEnumerable.Count());
				}
			}
		}

		private bool TypeEqualityIsVerifiable(Type type) {
			return type.IsPrimitive
				|| typeof(string).Equals(type)
				|| ReflectionHelpers.TypeDeclaresEqualsOverride(type);
		}

		public void Handle(AutoLinkNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(EmphasisNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(EntityNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(HeadingNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(LineBreakNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(LinkNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(OrderedListNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(OrderedListItemNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(ParagraphNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(QuotedNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(SpaceNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(StrongNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(SymbolNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(TableNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(TableDataCellNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(TableHeaderCellNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(TableRowNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(TextNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(UnorderedListNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(UnorderedListItemNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(InlineExpressionNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(ReferenceNode node) {
			AssertNodeIsAsExpected(node);
		}

		public void Handle(MarkdomDocumentNode node) {
			AssertNodeIsAsExpected(node);
		}
	}
}
