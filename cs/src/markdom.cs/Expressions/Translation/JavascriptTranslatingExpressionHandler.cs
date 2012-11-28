using markdom.cs.Utils;
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
			_writer.Write(JavascriptUtils.NumberEncode(expression.Value));
		}

		public void Handle(ObjectLiteralExpression expression) {
			_writer.Write("{");

			var propertyAssignmentEnumerator = expression.PropertyAssignments.GetEnumerator();
			if(propertyAssignmentEnumerator.MoveNext()) do {
				_writer.Write("'");
				_writer.Write(JavascriptUtils.StringEncode(propertyAssignmentEnumerator.Current.PropertyName));
				_writer.Write("'");
				_writer.Write(":");

				propertyAssignmentEnumerator.Current.PropertyValue.HandleWith(this);

				if(!propertyAssignmentEnumerator.MoveNext())
					break;

				_writer.Write(",");
			} while(true);

			_writer.Write("}");
		}

		public void Handle(StringLiteralExpression expression) {
			_writer.Write("'");
			_writer.Write(JavascriptUtils.StringEncode(expression.Value));
			_writer.Write("'");
		}

		public void Handle(UriLiteralExpression expression) {
			_writer.Write("'");
			_writer.Write(JavascriptUtils.StringEncode(expression.Value));
			_writer.Write("'");
		}

		public void Handle(DeleteExpression expression) {
			_writer.Write("delete ");
			expression.Body.HandleWith(this);
		}

		public void Handle(BitwiseNotExpression expression) {
			_writer.Write("~");
			expression.Body.HandleWith(this);
		}

		public void Handle(LogicalNotExpression expression) {
			_writer.Write("!");
			expression.Body.HandleWith(this);
		}

		public void Handle(NegativeExpression expression) {
			_writer.Write("-");
			expression.Body.HandleWith(this);
		}

		public void Handle(PositiveExpression expression) {
			_writer.Write("+");
			expression.Body.HandleWith(this);
		}

		public void Handle(PrefixDecrementExpression expression) {
			_writer.Write("--");
			expression.Body.HandleWith(this);
		}

		public void Handle(PrefixIncrementExpression expression) {
			_writer.Write("++");
			expression.Body.HandleWith(this);
		}

		public void Handle(TypeofExpression expression) {
			_writer.Write("typeof ");
			expression.Body.HandleWith(this);
		}

		public void Handle(VoidExpression expression) {
			_writer.Write("void ");
			expression.Body.HandleWith(this);
		}

		public void Handle(PostfixDecrementExpression expression) {
			expression.Body.HandleWith(this);
			_writer.Write("--");
		}

		public void Handle(PostfixIncrementExpression expression) {
			expression.Body.HandleWith(this);
			_writer.Write("++");
		}

		public void Handle(DivisionExpression expression) {
			expression.Left.HandleWith(this);
			_writer.Write("/");
			expression.Right.HandleWith(this);
		}

		public void Handle(ModuloExpression expression) {
			expression.Left.HandleWith(this);
			_writer.Write("%");
			expression.Right.HandleWith(this);
		}

		public void Handle(MultiplicationExpression expression) {
			expression.Left.HandleWith(this);
			_writer.Write("*");
			expression.Right.HandleWith(this);
		}

		public void Handle(AdditionExpression expression) {
			expression.Left.HandleWith(this);
			_writer.Write("+");
			expression.Right.HandleWith(this);
		}

		public void Handle(SubtractionExpression expression) {
			expression.Left.HandleWith(this);
			_writer.Write("-");
			expression.Right.HandleWith(this);
		}

		public void Handle(LeftShiftExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("<<");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(RightShiftExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write(">>");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(UnsignedRightShiftExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write(">>>");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(GreaterThanExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write(">");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(GreaterThanOrEqualToExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write(">=");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(InExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write(" in ");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(InstanceOfExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write(" instanceof ");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(LessThanExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("<");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(LessThanOrEqualToExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("<=");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(EqualsExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("==");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(NotEqualsExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("!=");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(StrictEqualsExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("===");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(StrictNotEqualsExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("!==");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(BitwiseAndExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("&");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(BitwiseOrExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("|");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(BitwiseXOrExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("^");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}


		public void Handle(LogicalAndExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("&&");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}


		public void Handle(LogicalOrExpression expression) {
			_writer.Write("(");
			expression.Left.HandleWith(this);
			_writer.Write("||");
			expression.Right.HandleWith(this);
			_writer.Write(")");
		}

		public void Handle(ConditionalExpression expression) {
			_writer.Write("(");
			expression.Condition.HandleWith(this);
			_writer.Write("?");
			expression.TrueExpression.HandleWith(this);
			_writer.Write(":");
			expression.FalseExpression.HandleWith(this);
			_writer.Write(")");
		}
	}
}
