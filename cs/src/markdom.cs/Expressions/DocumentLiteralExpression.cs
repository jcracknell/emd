﻿using markdom.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class DocumentLiteralExpression : IExpression {
		private readonly INode[] _content;
		private readonly SourceRange _sourceRange;

		public DocumentLiteralExpression(INode[] content, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => content, content);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_content = content;
			_sourceRange = sourceRange;
		}

		public IEnumerable<INode> Content { get { return _content; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
