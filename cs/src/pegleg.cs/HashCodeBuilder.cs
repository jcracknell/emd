using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	internal class HashCodeBuilder {
		private int _shift;
		private int _hashCode;

		public HashCodeBuilder Merge(object o) {
			if(null == o) return this;

			int oh = o.GetHashCode();

			_hashCode ^= (oh << _shift) | (oh >> _shift);
			_shift = ((_shift ^ oh) - 3) & 31;

			return this;
		}

		public HashCodeBuilder MergeAll(IEnumerable os) {
			foreach(var o in os) {
				if(null == o) continue;

				var oh = o.GetHashCode();

				_hashCode ^= (oh << _shift) | (oh >> _shift);
				_shift = ((_shift ^ oh) - 3) & 31;
			}

			return this;
		}

		public override int GetHashCode() {
			return _hashCode;
		}

		public override string ToString() {
			return string.Concat(typeof(HashCodeBuilder).Name, "(", _hashCode, ")");
		}
	}
}
