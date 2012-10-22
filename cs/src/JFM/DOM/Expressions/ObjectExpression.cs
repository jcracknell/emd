using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM.Expressions {
	public class ObjectExpression : Expression {
		private readonly PropertyAssignmentExpression[] _propertyAssignments;

		public ObjectExpression(PropertyAssignmentExpression[] propertyAssignments, SourceRange sourceRange)
			 : base(sourceRange)
		{ 
			CodeContract.ArgumentIsNotNull(() => propertyAssignments, propertyAssignments);

			_propertyAssignments = propertyAssignments;
		}


		public IEnumerable<PropertyAssignmentExpression> PropertyAssignments { get { return _propertyAssignments; } }

		public override ExpressionType Type { get { return ExpressionType.Object; } }
	}
}
