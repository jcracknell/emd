using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs {
	internal static class CodeContract {
		public static void ArgumentIsNotNull<T>(Expression<Func<T>> arg, T actual) where T : class {
			#if DEBUG
			GetArgumentName(arg);
			#endif

			if(null == actual)
				throw new ArgumentNullException(GetArgumentName(arg));
		}

		public static void ArgumentIsValid(Expression<Func<object>> arg, bool valid, string assertion) {
			#if DEBUG
			GetArgumentName(arg);				
			CodeContract.ArgumentIsNotNull(() => assertion, assertion);
			#endif

			if(valid) return;

			var argumentName = GetArgumentName(arg);

			throw new ArgumentException(
				string.Concat(argumentName, " is invalid: ", assertion),
				assertion);
		}

		public static void ThrowArgumentException(Expression<Func<object>> arg, string message) {
			#if DEBUG
			GetArgumentName(arg);
			CodeContract.ArgumentIsNotNull(() => message, message);
			#endif

			var argumentName = GetArgumentName(arg);
			throw new ArgumentException(
				string.Concat(argumentName, " ", message),
				argumentName);
		}

		private static string GetArgumentName(LambdaExpression arg) {
			var member = ReflectionUtils.GetMemberFrom(arg);

			if(MemberTypes.Field != member.MemberType)
				throw new ArgumentException("Invalid " + typeof(CodeContract).Name + " usage: provided argument expression does not reference a field.");

			return member.Name;
		}
	}
}
