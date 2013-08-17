using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  /// <summary>
  /// <see cref="IGraphemeCriterion"/> implementation which is satisfied by any grapheme composed of
  /// a single code point satisfying optional <see cref="ICodePointCriteria"/>.
  /// </summary>
  public class SingleCodePointGraphemeCriterion : IGraphemeCriteria {
    private readonly ICodePointCriteria _criteria;

    public SingleCodePointGraphemeCriterion() {
      _criteria = null;
    }

    public SingleCodePointGraphemeCriterion(ICodePointCriteria criteria) {
      if(null == criteria) throw Xception.Because.ArgumentNull(() => criteria);

      _criteria = criteria;
    }

    public bool SatisfiedBy(string str, int index, int length, System.Globalization.UnicodeCategory category) {
      if(2 < length)
        return false;

      int codePointLength;
      int codePoint = UnicodeUtils.GetCodePoint(str, index, out codePointLength);

      return length == codePointLength
        && (null == _criteria || _criteria.SatisfiedBy(codePoint));
    }
  }
}
