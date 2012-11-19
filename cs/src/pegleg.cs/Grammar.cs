using pegleg.cs.Parsing;
using pegleg.cs.Parsing.Expressions;
using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace pegleg.cs {
	public abstract class Grammar<TProduct> : ParsingExpression {
		private IParsingExpression<TProduct> _start = null;

		protected IParsingExpression<TProduct> Start {
			get { return _start; }
			set { _start = value; }
		}

		protected void Define<T>(Expression<Func<IParsingExpression<T>>> reference, IParsingExpression<T> expression) {
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

		private void DefineField(FieldInfo field, IParsingExpression expression) {
			field.SetValue(this, expression);
		}

		private void DefineProperty(PropertyInfo property, IParsingExpression expression) {
			property.SetValue(this, expression, null);
		}

	}
}
