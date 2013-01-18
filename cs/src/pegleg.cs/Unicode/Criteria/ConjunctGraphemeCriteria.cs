using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	public class ConjunctGraphemeCriteria : IGraphemeCriteria {
		private readonly IGraphemeCriteria[] _criteria;

		public ConjunctGraphemeCriteria(IEnumerable<IGraphemeCriteria> criteria) {
			if(null == criteria) throw Xception.Because.ArgumentNull(() => criteria);
			if(!criteria.Any()) throw Xception.Because.Argument(() => criteria, "cannot be empty");

			_criteria = criteria.ToArray();
		}

		public bool SatisfiedBy(string str, int index, int length, System.Globalization.UnicodeCategory category) {
			for(var i = 0; i < _criteria.Length; i++)
				if(!_criteria[i].SatisfiedBy(str, index, length, category))
					return false;

			return true;
		}
	}
}
