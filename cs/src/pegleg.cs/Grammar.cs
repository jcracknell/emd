using pegleg.cs.Parsing;
using pegleg.cs.Parsing.Expressions;
using pegleg.cs.Parsing.Expressions.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs {
	public abstract class Grammar<TProduct> : ExpressionBuilder {
		private IExpression<TProduct> _start = null;

		protected IExpression<TProduct> Start {
			get { return _start; }
			set { _start = value; }
		}

		protected void Define<T>(Expression<Func<IExpression<T>>> reference, IExpression<T> expression) {
			var member = ReflectionUtils.GetMemberFrom(reference);
			
			var namedExpression = Named(member.Name, expression);

			switch(member.MemberType) {
				case MemberTypes.Field:
					DefineField((FieldInfo)member, namedExpression);
					return;
				case MemberTypes.Property:
					DefineProperty((PropertyInfo)member, namedExpression);
					return;
				default:
					CodeContract.ThrowArgumentException(() => reference, "does not reference a property or field");
					return;
			}
		}

		private void DefineField(FieldInfo field, IExpression expression) {
			field.SetValue(this, expression);
		}

		private void DefineProperty(PropertyInfo property, IExpression expression) {
			property.SetValue(this, expression, null);
		}

	}
}
