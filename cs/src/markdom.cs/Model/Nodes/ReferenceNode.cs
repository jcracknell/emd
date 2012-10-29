using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes {
	public class ReferenceNode : IBlockNode {
		private readonly ReferenceId _referenceId;
		private readonly IExpression[] _arguments;
		private readonly MarkdomSourceRange _sourceRange;

		public ReferenceNode(ReferenceId referenceId, IExpression[] arguments, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => arguments, arguments);

			_referenceId = referenceId;
			_arguments = arguments;
			_sourceRange = sourceRange;
		}

		public ReferenceId ReferenceId { get { return _referenceId; } }

		public IEnumerable<IExpression> Arguments { get { return _arguments; } }

		public NodeKind Kind { get { return NodeKind.Reference; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
