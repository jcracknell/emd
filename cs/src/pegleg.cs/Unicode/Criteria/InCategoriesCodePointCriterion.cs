﻿using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  /// <summary>
  /// Criterion which is satisfied by code points in a specified set of <see cref="UnicodeCategory"/> values.
  /// </summary>
  public class InCategoriesCodePointCriterion : ICodePointCriteria {
    private static readonly int MAX_UNICODE_CATEGORY = Enum.GetValues(typeof(UnicodeCategory)).Cast<int>().Max();
    private readonly bool[] _acceptanceMap;

    /// <summary>
    /// Create a criterion which is satisfied by code points in the provided <paramref name="categories"/>.
    /// </summary>
    public InCategoriesCodePointCriterion(IEnumerable<UnicodeCategory> categories) {
      if(null == categories) throw Xception.Because.ArgumentNull(() => categories);
      if(!categories.Any()) throw Xception.Because.Argument(() => categories, "cannot be empty");

      _acceptanceMap = new bool[MAX_UNICODE_CATEGORY + 1];
      foreach(var category in categories)
        _acceptanceMap[(int)category] = true;
    }

    public bool SatisfiedBy(int codePoint) {
      var category = (int)UnicodeUtils.GetCategory(codePoint);

      return _acceptanceMap[category];
    }
  }
}
