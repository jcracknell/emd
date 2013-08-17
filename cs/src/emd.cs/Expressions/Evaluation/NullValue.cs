using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions.Evaluation {
  public class NullValue : IValue {
    private static readonly NullValue _instance = new NullValue();

    public static NullValue Instance { get { return _instance; } }

    private NullValue() { }

    public T HandleWith<T>(IValueHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
