using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM {
	public abstract class Node {
		private readonly SourceRange _sourceRange;

		public Node(SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public abstract NodeType NodeType { get; }
		public abstract void HandleWith(INodeHandler handler);
		public abstract T HandleWith<T>(INodeHandler<T> handler);

		public override int GetHashCode() {
			return new HashCodeBuilder()
				.Merge(GetType())
				.Merge(SourceRange)
				.GetHashCode();
		}

		public override bool Equals(object obj) {
			var other = obj as Node;
			return null != other
				&& this.SourceRange.Equals(other.SourceRange);
		}
	}
}
