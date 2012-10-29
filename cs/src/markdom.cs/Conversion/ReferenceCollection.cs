using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Conversion {
	public class ReferenceCollection {
		IDictionary<ReferenceId, IEnumerable<IExpression>> _references;

		public ReferenceCollection(MarkdomDocumentNode document) {
			_references = new Dictionary<ReferenceId, IEnumerable<IExpression>>();

			var handler = new ReferenceExtractingNodeHandler();
			foreach(var referenceNode in document.HandleWith(handler)) {
				if(null != referenceNode.ReferenceId)
					_references[referenceNode.ReferenceId] = referenceNode.Arguments;
			}
		}

		public IEnumerable<IExpression> GetArgumentsFor(ReferenceId referenceId) {
			IEnumerable<IExpression> arguments;
			if(_references.TryGetValue(referenceId, out arguments))
				return arguments;

			return Enumerable.Empty<IExpression>();
		}
	}
}
