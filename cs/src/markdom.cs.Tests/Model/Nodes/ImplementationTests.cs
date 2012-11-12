using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;

namespace markdom.cs.Nodes {
	public class ImplementationTests {
		[Fact] public void All_fields_on_INode_implementations_are_readonly() {
			var nodeTypeFieldsWhichAreNotReadOnly =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(field => !field.IsInitOnly);

			nodeTypeFieldsWhichAreNotReadOnly.Should().BeEmpty("because all nodes should contain only readonly fields");
		}

		[Fact] public void No_fields_on_INode_implementors_are_public() {
			var nodeTypeFieldsWhichArePublic =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(field => field.IsPublic);

			nodeTypeFieldsWhichArePublic.Should().BeEmpty("because no node types should have public fields");
		}

		[Fact] public void No_public_properties_on_INode_implementors_declare_setters() {
			var publicNodePropertiesDeclaringSetters =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetProperties())
				.Where(property => null != property.GetSetMethod());
			
			publicNodePropertiesDeclaringSetters.Should().BeEmpty("because no node types should declare settable properties");
		}

		[Fact] public void No_INode_implementors_expose_arrays() {
			var publicNodePropertiesWithArrayType =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetProperties())
				.Where(property => property.PropertyType.IsArray);

			publicNodePropertiesWithArrayType.Should().BeEmpty("because node types should not expose array properties");
		}
	}
}
