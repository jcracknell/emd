using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  public class NegatedCodePointCriteria : ICodePointCriteria {
    private readonly ICodePointCriteria _negated;

    public NegatedCodePointCriteria(ICodePointCriteria negated) {
      if(null == negated) throw Xception.Because.ArgumentNull(() => negated);

      _negated = negated;
    }

    public bool SatisfiedBy(int codePoint) {
      return !_negated.SatisfiedBy(codePoint);
    }
  }
}
