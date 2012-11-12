using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class PropertyAssignment {
		private readonly IExpression _propertyName;
		private readonly IExpression _propertyValue;
		private readonly SourceRange _sourceRange;

		public PropertyAssignment(IExpression propertyName, IExpression propertyValue, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => propertyName, propertyName);
			CodeContract.ArgumentIsNotNull(() => propertyValue, propertyValue);

			_propertyName = propertyName;
			_propertyValue = propertyValue;
			_sourceRange = sourceRange;
		}

		public IExpression PropertyName { get { return _propertyName; } }

		public IExpression PropertyValue { get { return _propertyValue; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public override bool Equals(object obj) {
			var other = obj as PropertyAssignment;
			return null != other
				&& this.PropertyName.Equals(other.PropertyName)
				&& this.PropertyValue.Equals(other.PropertyValue)
				&& this.SourceRange.Equals(other.SourceRange);
		}
	}
}
