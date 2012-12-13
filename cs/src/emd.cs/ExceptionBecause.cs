using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace emd.cs {
	internal static class ExceptionBecause {
		public static ArgumentNullException ArgumentNull<T>(Expression<Func<T>> arg) {
			if(null == arg) throw ExceptionBecause.ArgumentNull(() => arg);

			var argumentName = GetArgumentName(arg);
			return new ArgumentNullException(argumentName, string.Concat("Argument ", argumentName, " cannot be null."));
		}

		public static ArgumentException Argument<T>(Expression<Func<T>> arg, string reason) {
			if(null == arg) throw ExceptionBecause.ArgumentNull(() => arg);
			if(null == reason) throw ExceptionBecause.ArgumentNull(() => reason);

			var argumentName = GetArgumentName(arg);
			return new ArgumentException(string.Concat("Argument ", argumentName, " is invalid: ", reason), argumentName);
		}

		private static string GetArgumentName(LambdaExpression arg) {
			var member = ReflectionUtils.GetMemberFrom(arg);
			if(MemberTypes.Field != member.MemberType)
				throw ExceptionBecause.Argument(() => arg, "provided argument expression does not reference a field.");

			return member.Name;
		}
	}
}
