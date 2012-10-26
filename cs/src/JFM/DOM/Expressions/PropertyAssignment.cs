using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM.Expressions {
	public class PropertyAssignment {
		private readonly Expression _propertyName;
		private readonly Expression _propertyValue;
		private readonly SourceRange _sourceRange;

		public PropertyAssignment(Expression propertyName, Expression propertyValue, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => propertyName, propertyName);
			CodeContract.ArgumentIsNotNull(() => propertyValue, propertyValue);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_propertyName = propertyName;
			_propertyValue = propertyValue;
			_sourceRange = sourceRange;
		}

		public Expression PropertyName { get { return _propertyName; } }

		public Expression PropertyValue { get { return _propertyValue; } }

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
