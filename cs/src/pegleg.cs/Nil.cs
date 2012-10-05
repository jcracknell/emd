using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs {
	/// <summary>
	/// Exactly what it sounds like. Functions as a void type.
	/// </summary>
	public class Nil {
		private static readonly Nil _instance;

		private Nil() { }

		static Nil() {
			_instance = new Nil();
		}

		public static Nil Value { get { return _instance; }	}

		public override string ToString() {
			return typeof(Nil).Name;
		}
	}
}
