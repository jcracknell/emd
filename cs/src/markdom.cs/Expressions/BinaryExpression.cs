﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public abstract class BinaryExpression : Expression {
		private readonly IExpression _left;
		private readonly IExpression _right;

		public BinaryExpression(IExpression left, IExpression right, SourceRange sourceRange)
			: base(sourceRange)
		{ 
			if(null == left) throw ExceptionBecause.ArgumentNull(() => left);
			if(null == right) throw ExceptionBecause.ArgumentNull(() => right);

			_left = left;
			_right = right;
		}

		public IExpression Left { get { return _left; } }

		public IExpression Right { get { return _right; } }
	}
}
