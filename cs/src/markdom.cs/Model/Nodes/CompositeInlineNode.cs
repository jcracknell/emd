using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public abstract class CompositeNode : Node {
		private readonly Node[] _children;

		public CompositeNode(Node[] children, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => children, children);

			_children = children;
		}

		public IEnumerable<Node> Children { get { return _children; } }

		public override int GetHashCode() {
			return new HashCodeBuilder()
				.Merge(this.SourceRange)
				.MergeAll(this.Children)
				.GetHashCode();
		}

		protected bool Equals(CompositeNode other) {
			return null != other 
				&& this.SourceRange.Equals(other.SourceRange)
				&& this.Children.SequenceEqual(other.Children);
		}
	}
}
