using markdom.cs.Model.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class AutoLinkNode : Node {
		private readonly UriExpression _uri;
		private readonly Expression[] _arguments;

		public AutoLinkNode(UriExpression uri, Expression[] arguments, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => uri, uri);
			CodeContract.ArgumentIsNotNull(() => arguments, arguments);
			
			_uri = uri;
			_arguments = arguments;	
		}

		public UriExpression Uri { get { return _uri; } }

		public IEnumerable<Expression> Arguments { get { return _arguments; } }

		public override NodeType NodeType { get { return NodeType.AutoLink; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as AutoLinkNode;
			return null != other
				&& this.Uri.Equals(other.Uri)
				&& Enumerable.SequenceEqual(this.Arguments, other.Arguments)
				&& base.Equals(other);
		}
	}
}
