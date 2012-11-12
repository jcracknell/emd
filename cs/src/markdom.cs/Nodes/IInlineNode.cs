using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes {
	public interface IInlineNode : INode { }
	public interface IPlainInlineNode : IInlineNode { }
	public interface IFormattedInlineNode : IPlainInlineNode { }
	public interface IRichInlineNode : IFormattedInlineNode { }
}
