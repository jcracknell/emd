using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public class StringValue : IValue {
		private readonly string _value;

		public StringValue(string value) {
			if(null == value) throw ExceptionBecause.ArgumentNull(() => value);

			_value = value;
		}

		public string Value { get { return _value; } }

		public T HandleWith<T>(IValueHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
