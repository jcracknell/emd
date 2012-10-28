using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public interface INode {
		NodeType NodeType { get; }
		MarkdomSourceRange SourceRange { get; }
		void HandleWith(INodeHandler handler);
		T HandleWith<T>(INodeHandler<T> handler);
	}
}
