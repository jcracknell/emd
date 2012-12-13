using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
	public class AutoLinkNode : IRichInlineNode {
		private readonly string _uri;
		private readonly IExpression[] _arguments;
		private readonly SourceRange _sourceRange;

		public AutoLinkNode(string uri, IExpression[] arguments, SourceRange sourceRange)
		{
			if(null == uri) throw ExceptionBecause.ArgumentNull(() => uri);
			if(null == arguments) throw ExceptionBecause.ArgumentNull(() => arguments);
			
			_uri = uri;
			_arguments = arguments;	
			_sourceRange = sourceRange;
		}

		public string Uri { get { return _uri; } }

		public IEnumerable<IExpression> Arguments { get { return _arguments; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
