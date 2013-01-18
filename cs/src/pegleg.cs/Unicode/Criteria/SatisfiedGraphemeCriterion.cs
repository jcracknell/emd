using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// <see cref="IGraphemeCriteria"/> implementation which is always satisfied.
	/// </summary>
	public class SatisfiedGraphemeCriterion : IGraphemeCriteria {
		private static readonly SatisfiedGraphemeCriterion _instance = new SatisfiedGraphemeCriterion();

		public static SatisfiedGraphemeCriterion Instance { get { return _instance; } }

		private SatisfiedGraphemeCriterion() { }

		public bool SatisfiedBy(string str, int index, int length, UnicodeCategory category) {
			return true;
		}
	}
}
