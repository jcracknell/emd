using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Factory class for the creation of <see cref="IUnicodeCriteria"/> instances.
	/// </summary>
	public class UnicodeCriteria {
		private UnicodeCriteria() { }

		/// <summary>
		/// Create a base <see cref="IUnicodeCriteria"/> which will match all of a selectable class of text elements.
		/// </summary>
		public static TextElementSelector All { get { return new TextElementSelector(true); } }

		/// <summary>
		/// Create a base <see cref="IUnicodeCriteria"/> which will match none of a selectable class of text elements.
		/// </summary>
		public static TextElementSelector No { get { return new TextElementSelector(false); } }

		public class TextElementSelector {
			private readonly bool _sense;

			public TextElementSelector(bool sense) {
				_sense = sense;
			}

			/// <summary>
			/// Create a base <see cref="IUnicodeCriteria"/> for unicode graphemes.
			/// </summary>
			public GraphemeCriteria Graphemes {
				get { return _sense ? GraphemeCriteria.Any : GraphemeCriteria.None; }
			}

			/// <summary>
			/// Create a base <see cref="IUnicodeCriteria"/> for unicode code points.
			/// </summary>
			public CodePointCriteria CodePoints {
				get { return _sense ? CodePointCriteria.Any : CodePointCriteria.None; }
			}
		}
	}
}
