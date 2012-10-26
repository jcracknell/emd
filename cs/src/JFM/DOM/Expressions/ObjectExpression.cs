﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM.Expressions {
	public class ObjectExpression : Expression {
		private readonly PropertyAssignment[] _propertyAssignments;

		public ObjectExpression(PropertyAssignment[] propertyAssignments, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => propertyAssignments, propertyAssignments);

			_propertyAssignments = propertyAssignments;
		}

		public IEnumerable<PropertyAssignment> PropertyAssignments { get { return _propertyAssignments; } }

		public override ExpressionType Type { get { return ExpressionType.Object; } }

		public override bool Equals(object obj) {
			var other = obj as ObjectExpression;
			return null != other
				&& this.SourceRange.Equals(other.SourceRange)
				&& Enumerable.SequenceEqual(this.PropertyAssignments, other.PropertyAssignments);
		}
	}
}
