using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs.Utils {
	internal static class ReflectionUtils {
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
	}
}
