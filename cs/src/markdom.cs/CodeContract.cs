using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace markdom.cs {
	internal static class CodeContract {
		public static void ArgumentIsNotNull<T>(Expression<Func<T>> arg, T actual) where T : class {
			#if DEBUG
			GetArgumentName(arg);
			#endif

			if(null != actual) return;

			var argumentName = GetArgumentName(arg);
			throw new ArgumentNullException(argumentName, string.Concat("Argument ", argumentName, " cannot be null."));
		}

		public static void ArgumentIsValid(Expression<Func<object>> arg, bool valid, string assertion) {
			#if DEBUG
			GetArgumentName(arg);				
			CodeContract.ArgumentIsNotNull(() => assertion, assertion);
			#endif

			if(valid) return;

			var argumentName = GetArgumentName(arg);

			throw new ArgumentException(string.Concat("Argument ", argumentName, " is invalid: ", assertion), argumentName);
		}

		public static void ThrowArgumentException(Expression<Func<object>> arg, string message) {
			#if DEBUG
			GetArgumentName(arg);
			CodeContract.ArgumentIsNotNull(() => message, message);
			#endif

			var argumentName = GetArgumentName(arg);
			throw new ArgumentException(string.Concat(argumentName, " ", message), argumentName);
		}

		private static string GetArgumentName(LambdaExpression arg) {
			var member = ReflectionUtils.GetMemberFrom(arg);

			if(MemberTypes.Field != member.MemberType)
				throw new ArgumentException("Invalid " + typeof(CodeContract).Name + " usage: provided argument expression does not reference a field.");

			return member.Name;
		}
	}
}
