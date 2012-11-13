using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions.Evaluation {
	public interface IValueHandler<T> {
		T Handle(BooleanValue value);
		T Handle(NullValue value);
		T Handle(NumberValue value);
		T Handle(ObjectValue value);
		T Handle(StringValue value);
		T Handle(UndefinedValue value);
	}
}
