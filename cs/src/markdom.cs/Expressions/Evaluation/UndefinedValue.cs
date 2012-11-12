using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public class UndefinedValue : IValue {
		private static readonly UndefinedValue _instance = new UndefinedValue();

		public static UndefinedValue Instance { get { return _instance; } }

		static UndefinedValue() { }
	}
}
