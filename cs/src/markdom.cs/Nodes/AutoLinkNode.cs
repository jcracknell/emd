using markdom.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes{
	public class AutoLinkNode : IRichInlineNode {
		private readonly UriLiteralExpression _uri;
		private readonly IExpression[] _arguments;
		private readonly SourceRange _sourceRange;

		public AutoLinkNode(UriLiteralExpression uri, IExpression[] arguments, SourceRange sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => uri, uri);
			CodeContract.ArgumentIsNotNull(() => arguments, arguments);
			
			_uri = uri;
			_arguments = arguments;	
			_sourceRange = sourceRange;
		}

		public UriLiteralExpression Uri { get { return _uri; } }

		public IEnumerable<IExpression> Arguments { get { return _arguments; } }

		public NodeKind Kind { get { return NodeKind.AutoLink; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
