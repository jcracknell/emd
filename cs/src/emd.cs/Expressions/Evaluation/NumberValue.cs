using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions.Evaluation {
  public class NumberValue : IValue {
    private readonly double _value;

    public NumberValue(double value) {
      _value = value;
    }

    public double Value { get { return _value; } }

    public T HandleWith<T>(IValueHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
