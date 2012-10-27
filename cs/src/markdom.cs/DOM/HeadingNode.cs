using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class HeadingNode : Node {
		private readonly int _level;
		private readonly string _text;

		public HeadingNode(string text, int level, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => text, text);
			CodeContract.ArgumentIsValid(() => level, level >= 0, "must be a non-negative integer");

			_text = text;
			_level = level;
		}

		public string Text { get { return _text; } }

		public int Level { get { return _level; } }

		public override NodeType NodeType { get { return NodeType.Heading; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as HeadingNode;
			return null != other
				&& this.Level.Equals(other.Level)
				&& base.Equals(other);
		}
	}
}
