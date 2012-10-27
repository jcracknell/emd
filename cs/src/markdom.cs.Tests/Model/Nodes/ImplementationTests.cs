using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace markdom.cs.Model.Nodes {
	[TestClass]
	public class ImplementationTests {
		private bool IsNodeType(Type candidate) {
			if(typeof(Node).Equals(candidate))
				return true;

			if(null == candidate.BaseType) return false;

			return IsNodeType(candidate.BaseType);
		}

		private bool IsInstantiableNodeType(Type candidate) {
			return candidate.IsClass && !candidate.IsAbstract && IsNodeType(candidate);
		}

		private bool MethodIsEqualsOverride(MethodInfo method) {
			return method.Name.Equals("Equals")
				&& method.ReturnType.Equals(typeof(bool))
				&& method.GetParameters().Length.Equals(1)
				&& method.GetParameters()[0].ParameterType.Equals(typeof(object));
		}

		private bool TypeDoesNotDeclareEqualsOverride(Type candidate) {
			return !candidate
				.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
				.Any(MethodIsEqualsOverride);
		}

		[TestMethod]
		public void All_Node_implementations_override_Equals() {
			var nodeImplementationsWithoutEqualsOverride =
				typeof(Node).Assembly.GetTypes()
				.Where(IsInstantiableNodeType)
				.Where(TypeDoesNotDeclareEqualsOverride);

			if(nodeImplementationsWithoutEqualsOverride.Any())
				Assert.Fail(
					string.Concat(
						"The following instantiable node types do not declare an override for Equals: ",
						string.Join(", ", nodeImplementationsWithoutEqualsOverride.Select(t => t.FullName).ToArray())));
		}
	}
}
