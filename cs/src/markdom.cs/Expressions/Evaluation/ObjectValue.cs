using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public class ObjectValue : IValue {
		private readonly Dictionary<string, IValue> _properties = new Dictionary<string,IValue>();

		public ObjectValue() { }

		public T HandleWith<T>(IValueHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
