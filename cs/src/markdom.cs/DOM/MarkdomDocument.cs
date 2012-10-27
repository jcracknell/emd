using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class MarkdomDocument {
		private readonly Node[] _content;

		public MarkdomDocument(Node[] content) {
			CodeContract.ArgumentIsNotNull(() => content, content);

			_content = content;
		}

		public IEnumerable<Node> Content { get { return _content; } }
	}
}
