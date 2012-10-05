using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM {
	public class JfmDocument {
		private readonly Node[] _content;

		public JfmDocument(Node[] content) {
			CodeContract.ArgumentIsNotNull(() => content, content);

			_content = content;
		}

		public IEnumerable<Node> Content { get { return _content; } }
	}
}
