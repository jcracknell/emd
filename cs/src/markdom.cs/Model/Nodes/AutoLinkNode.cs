using markdom.cs.Model.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class AutoLinkNode : IRichInlineNode {
		private readonly UriExpression _uri;
		private readonly Expression[] _arguments;
		private readonly SourceRange _sourceRange;

		public AutoLinkNode(UriExpression uri, Expression[] arguments, SourceRange sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => uri, uri);
			CodeContract.ArgumentIsNotNull(() => arguments, arguments);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);
			
			_uri = uri;
			_arguments = arguments;	
			_sourceRange = sourceRange;
		}

		public UriExpression Uri { get { return _uri; } }

		public IEnumerable<Expression> Arguments { get { return _arguments; } }

		public NodeType NodeType { get { return NodeType.AutoLink; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
