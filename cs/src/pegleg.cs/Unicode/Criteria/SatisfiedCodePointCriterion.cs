using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  /// <summary>
  /// <see cref="ICodePointCriteria"/> implementation which is always satisfied.
  /// </summary>
  public class SatisfiedCodePointCriterion : ICodePointCriteria {
    private static readonly SatisfiedCodePointCriterion _instance = new SatisfiedCodePointCriterion();

    public static SatisfiedCodePointCriterion Instance { get { return _instance; } }

    private SatisfiedCodePointCriterion() { }

    public bool SatisfiedBy(int codePoint) {
      return true;
    }
  }
}
