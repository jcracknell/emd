using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM.Expressions {
	public class PropertyAssignmentExpression : Expression {
		private static readonly ExpressionType[] VALID_PROPERTYNAME_TYPES =
			new ExpressionType[] { ExpressionType.String };
		private static readonly string INVALID_PROPERTYNAME_TYPE_MSG = 
			string.Concat("invalid expression type, must be one of ", string.Join(", ", VALID_PROPERTYNAME_TYPES.Select(t => t.ToString()).ToArray()));

		private readonly Expression _propertyName;
		private readonly Expression _propertyValue;

		public PropertyAssignmentExpression(Expression propertyName, Expression propertyValue, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => propertyName, propertyName);
			CodeContract.ArgumentIsNotNull(() => propertyValue, propertyValue);
			CodeContract.ArgumentIsValid(() => propertyName, VALID_PROPERTYNAME_TYPES.Contains(propertyName.Type), INVALID_PROPERTYNAME_TYPE_MSG);

			_propertyName = propertyName;
			_propertyValue = propertyValue;
		}

		public Expression PropertyName { get { return _propertyName; } }

		public Expression PropertyValue { get { return _propertyValue; } }

		public override ExpressionType Type { get { return ExpressionType.PropertyAssignment; } }
	}
}
