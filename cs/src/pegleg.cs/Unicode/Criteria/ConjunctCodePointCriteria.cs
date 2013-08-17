using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  public class ConjunctCodePointCriteria : ICodePointCriteria {
    private readonly ICodePointCriteria[] _criteria;

    public ConjunctCodePointCriteria(IEnumerable<ICodePointCriteria> criteria) {
      if(null == criteria) throw Xception.Because.ArgumentNull(() => criteria);
      if(!criteria.Any()) throw Xception.Because.Argument(() => criteria, "cannot be empty");

      _criteria = criteria.ToArray();
    }

    public bool SatisfiedBy(int codePoint) {
      for(var i = 0; i < _criteria.Length; i++)
        if(!_criteria[i].SatisfiedBy(codePoint))
          return false;

      return true;
    }
  }
}
