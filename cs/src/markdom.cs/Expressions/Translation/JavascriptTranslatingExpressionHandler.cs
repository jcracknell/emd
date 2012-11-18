using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Translation {
	public class JavascriptTranslatingExpressionHandler : IExpressionHandler {
		private readonly TextWriter _writer;

		public JavascriptTranslatingExpressionHandler(TextWriter writer) {
			_writer = writer;
		}

		public void Handle(ArrayLiteralExpression expression) {
			_writer.Write("[");

			var elementEnumerator = expression.Elements.GetEnumerator();
			if(elementEnumerator.MoveNext()) do {
				// Elided (empty) array elements are represented by null expressions
				if(null != elementEnumerator.Current)
					elementEnumerator.Current.HandleWith(this);

				if(!elementEnumerator.MoveNext())
					break;

				_writer.Write(",");
			} while(true);

			_writer.Write("]");
		}

		public void Handle(BooleanLiteralExpression expression) {
			if(expression.Value) {
				_writer.Write("true");
			} else {
				_writer.Write("false");
			}
		}

		public void Handle(CallExpression expression) {
			expression.Body.HandleWith(this);
			_writer.Write("(");

			var argumentEnumerator = expression.Arguments.GetEnumerator();
			if(argumentEnumerator.MoveNext()) do {
				argumentEnumerator.Current.HandleWith(this);

				if(!argumentEnumerator.MoveNext())
					break;

				_writer.Write(", ");
			} while(true);

			_writer.Write(")");
		}

		public void Handle(DocumentLiteralExpression expression) {
			throw new NotImplementedException();
		}

		public void Handle(IdentifierExpression expression) {
			throw new NotImplementedException();
		}

		public void Handle(DynamicPropertyExpression expression) {
			expression.Body.HandleWith(this);
			_writer.Write("[");
			expression.MemberName.HandleWith(this);
			_writer.Write("]");
		}

		public void Handle(StaticPropertyExpression expression) {
			expression.Body.HandleWith(this);
			_writer.Write(".");
			_writer.Write(expression.MemberName);
		}

		public void Handle(NullLiteralExpression expression) {
			_writer.Write("null");
		}

		public void Handle(NumericLiteralExpression expression) {
			_writer.Write(JavascriptUtils.Encode(expression.Value));
		}

		public void Handle(ObjectLiteralExpression expression) {
			_writer.Write("{");

			var propertyAssignmentEnumerator = expression.PropertyAssignments.GetEnumerator();
			if(propertyAssignmentEnumerator.MoveNext()) do {
				_writer.Write(JavascriptUtils.Encode(propertyAssignmentEnumerator.Current.PropertyName));
				_writer.Write(":");

				propertyAssignmentEnumerator.Current.PropertyValue.HandleWith(this);

				if(!propertyAssignmentEnumerator.MoveNext())
					break;

				_writer.Write(",");
			} while(true);

			_writer.Write("}");
		}

		public void Handle(StringLiteralExpression expression) {
			_writer.Write(JavascriptUtils.Encode(expression.Value));
		}

		public void Handle(UriLiteralExpression expression) {
			_writer.Write(JavascriptUtils.Encode(expression.Value));
		}
	}
}
