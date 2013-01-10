using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs.Utils {
	public static class ReflectionUtils {
		public static MemberExpression GetMemberExpressionFrom(LambdaExpression expression) {
			var body = expression.Body;

			while(ExpressionType.Convert == body.NodeType)
				body = ((UnaryExpression)body).Operand;

			return body as MemberExpression;
		}
		
		public static MemberInfo GetMemberFrom(LambdaExpression expression) {
			var memberExpression = GetMemberExpressionFrom(expression);
			if(null == memberExpression)
				throw new ArgumentException("Expression does not reference a member.", "expression");

			return memberExpression.Member;
		}

		/// <summary>
		/// Retrieves an array containing all reflectable members for the provided <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> for which all members should be retrieved.</param>
		public static MemberInfo[] GetAllMembers(Type type) {
			if(null == type) throw Xception.Because.ArgumentNull(() => type);

			return type.GetMembers(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		}
	}
}
