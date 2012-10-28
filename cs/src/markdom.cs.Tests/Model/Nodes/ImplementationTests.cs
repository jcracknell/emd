using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using pegleg.cs.ExtensionMethods;

namespace markdom.cs.Model.Nodes {
	[TestClass]
	public class ImplementationTests {
		[TestMethod]
		public void All_fields_on_INode_implementations_are_readonly() {
			var nodeTypeFieldsWhichAreNotReadOnly =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(field => !field.IsInitOnly);

			if(nodeTypeFieldsWhichAreNotReadOnly.Any())
				Assert.Fail(
					nodeTypeFieldsWhichAreNotReadOnly
					.Select(f => "Field " + f.DeclaringType.FullName + "::" + f.Name + " is not readonly.")
					.Join("\n"));
		}

		[TestMethod]
		public void No_fields_on_INode_implementors_are_public() {
			var nodeTypeFieldsWhichArePublic =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic))
				.Where(field => field.IsPublic);

			if(nodeTypeFieldsWhichArePublic.Any())
				Assert.Fail(
					nodeTypeFieldsWhichArePublic
					.Select(f => "Field " + f.DeclaringType.FullName + "::" + f.Name + " is public.")
					.Join("\n"));
		}

		[TestMethod]
		public void No_public_properties_on_INode_implementors_declare_setters() {
			var publicNodePropertiesDeclaringSetters =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetProperties())
				.Where(property => null != property.GetSetMethod());
			
			if(publicNodePropertiesDeclaringSetters.Any())
				Assert.Fail(
					publicNodePropertiesDeclaringSetters
					.Select(p => "Property " + p.DeclaringType.FullName + "::" + p.Name + " declares a public setter.")
					.Join("\n"));
		}

		[TestMethod]
		public void No_INode_implementors_expose_arrays() {
			var publicNodePropertiesWithArrayType =
				typeof(INode).Assembly.GetTypes()
				.Where(ReflectionHelpers.IsNodeClass)
				.SelectMany(nodeType => nodeType.GetProperties())
				.Where(property => property.PropertyType.IsArray);

			if(publicNodePropertiesWithArrayType.Any())
				Assert.Fail(
					publicNodePropertiesWithArrayType
					.Select(p => "Property " + p.DeclaringType.FullName + "::" + p.Name + " has array type.")
					.Join("\n"));
		}
	}
}
