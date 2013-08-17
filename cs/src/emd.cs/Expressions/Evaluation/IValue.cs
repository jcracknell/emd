using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions.Evaluation {
  public interface IValue {
    T HandleWith<T>(IValueHandler<T> handler);
  }
}
