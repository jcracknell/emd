using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
	public interface IInlineNode : INode { }
	public interface IRichInlineNode : IInlineNode { }
	public interface IFormattedInlineNode : IRichInlineNode { }
	public interface IPlainInlineNode : IFormattedInlineNode { }
}
