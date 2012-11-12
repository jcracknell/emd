using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public class BooleanValue {
		private readonly bool _value;

		public BooleanValue(bool value) {
			_value = value;
		}

		public bool Value { get { return _value; } }
	}
}
