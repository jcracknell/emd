using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  public class NegatedGraphemeCriteria : IGraphemeCriteria {
    private IGraphemeCriteria _negated;

    public NegatedGraphemeCriteria(IGraphemeCriteria negated) {
      if(null == negated) throw Xception.Because.ArgumentNull(() => negated);

      _negated = negated;
    }

    public bool SatisfiedBy(string str, int index, int length, UnicodeCategory category) {
      return !_negated.SatisfiedBy(str, index, length, category);
    }
  }
}
