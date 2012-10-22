using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM {
	public class ReferenceId {
		public string Value { get; private set; }
		
		public ReferenceId(string value) {
			Value = value;
		}
	}
}
