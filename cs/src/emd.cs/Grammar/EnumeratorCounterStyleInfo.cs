﻿using emd.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
  public class EnumeratorCounterStyleInfo {
    public readonly OrderedListCounterStyle Style;
    public readonly int? InterpretedValue;

    public EnumeratorCounterStyleInfo(OrderedListCounterStyle style, int? interpretedValue) {
      Style = style;
      InterpretedValue = interpretedValue;
    }
  }
}
