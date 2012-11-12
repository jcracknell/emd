using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes {
	public interface IInlineNode : INode { }
	public interface IRichInlineNode : IInlineNode { }
	public interface IFormattedInlineNode : IRichInlineNode { }
	public interface IPlainInlineNode : IFormattedInlineNode { }
}
