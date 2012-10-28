using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Model.Nodes {
	public static class ReflectionHelpers {
		public static bool IsNodeClassOrInterface(Type candidate) {
			return typeof(INode).Equals(candidate)
				|| candidate.GetInterfaces().Any(implementedInterface => typeof(INode).Equals(implementedInterface));
		}

		public static bool IsNodeClass(Type candidate) {
			return candidate.IsClass && IsNodeClassOrInterface(candidate);
		}

		public static bool IsConcreteNodeClass(Type candidate) {
			return !candidate.IsAbstract && IsNodeClass(candidate);
		}

		public static bool IsIEnumerableNode(Type iface) {
			return iface.IsInterface
				&& iface.IsGenericType
				&& !iface.IsGenericTypeDefinition
				&& typeof(IEnumerable<>).Equals(iface.GetGenericTypeDefinition())
				&& IsNodeClassOrInterface(iface.GetGenericArguments()[0]);
		}

		public static bool IsEnumerableNodeClassOrInterface(Type candidate) {
			return IsIEnumerableNode(candidate) || candidate.GetInterfaces().Any(IsIEnumerableNode);
		}

		public static bool MethodIsEqualsOverride(MethodInfo method) {
			return method.Name.Equals("Equals")
				&& method.ReturnType.Equals(typeof(bool))
				&& method.GetParameters().Length.Equals(1)
				&& method.GetParameters()[0].ParameterType.Equals(typeof(object));
		}

		public static bool TypeDeclaresEqualsOverride(Type candidate) {
			return candidate
				.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
				.Any(MethodIsEqualsOverride);
		}
	}
}
