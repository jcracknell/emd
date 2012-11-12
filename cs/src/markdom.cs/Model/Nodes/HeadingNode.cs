﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class HeadingNode : IBlockNode {
		private readonly int _level;
		private readonly string _text;
		private readonly SourceRange _sourceRange;

		public HeadingNode(string text, int level, SourceRange sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => text, text);
			CodeContract.ArgumentIsValid(() => level, level >= 0, "must be a non-negative integer");

			_text = text;
			_level = level;
			_sourceRange = sourceRange;
		}

		public string Text { get { return _text; } }

		public int Level { get { return _level; } }

		public NodeKind Kind { get { return NodeKind.Heading; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
