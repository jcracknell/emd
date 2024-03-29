﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
  public interface IMatch {
    SourceRange SourceRange { get; }
    int Length { get; }

    /// <summary>
    /// The section of raw input matched by the expression.
    /// </summary>
    string String { get; }
  }

  /// <summary>
  /// Interface describing a successful match for an expression.
  /// </summary>
  /// <remarks>
  /// This interface uses methods instead of properties in order to improve syntactical
  /// compatibility with languages which do not support properties.
  /// </remarks>
  public interface IMatch<out TProduct> : IMatch {
    TProduct Product { get; }
  }
}
