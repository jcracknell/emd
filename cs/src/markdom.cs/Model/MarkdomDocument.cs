using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public class MarkdomDocument {
		private readonly IBlockNode[] _content;

		public MarkdomDocument(IBlockNode[] content) {
			CodeContract.ArgumentIsNotNull(() => content, content);

			_content = content;
		}

		public IEnumerable<IBlockNode> Content { get { return _content; } }
	}
}
