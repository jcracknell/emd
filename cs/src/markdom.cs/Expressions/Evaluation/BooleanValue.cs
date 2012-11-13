using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public class BooleanValue : IValue {
		private static readonly BooleanValue _trueInstance = new BooleanValue(true);
		private static readonly BooleanValue _falseInstance = new BooleanValue(false);

		public BooleanValue TrueInstance { get { return _trueInstance; } }
		public BooleanValue FalseInstance { get { return _falseInstance; } }

		private readonly bool _value;

		private BooleanValue(bool value) {
			_value = value;
		}

		public bool Value { get { return _value; } }

		public T HandleWith<T>(IValueHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
