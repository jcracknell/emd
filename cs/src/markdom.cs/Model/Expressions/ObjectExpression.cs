using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class ObjectExpression : IExpression {
		private readonly PropertyAssignment[] _propertyAssignments;
		private readonly MarkdomSourceRange _sourceRange;

		public ObjectExpression(PropertyAssignment[] propertyAssignments, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => propertyAssignments, propertyAssignments);

			_propertyAssignments = propertyAssignments;
			_sourceRange = sourceRange;
		}

		public IEnumerable<PropertyAssignment> PropertyAssignments { get { return _propertyAssignments; } }

		public ExpressionKind Kind { get { return ExpressionKind.Object; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
