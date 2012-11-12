using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public class ArrayValue : IValue {
		private readonly List<IValue> _elements = new List<IValue>();
	}
}
