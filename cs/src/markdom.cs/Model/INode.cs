using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public interface INode {
		NodeType NodeType { get; }
		SourceRange SourceRange { get; }
		void HandleWith(INodeHandler handler);
		T HandleWith<T>(INodeHandler<T> handler);
	}
}
