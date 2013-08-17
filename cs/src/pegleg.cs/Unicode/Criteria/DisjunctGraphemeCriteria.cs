using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
  public class DisjunctGraphemeCriteria : IGraphemeCriteria {
    private readonly IGraphemeCriteria[] _criteria;

    public DisjunctGraphemeCriteria(IGraphemeCriteria[] criteria) {
      if(null == criteria) throw Xception.Because.ArgumentNull(() => criteria);
      if(0 == criteria.Length) throw Xception.Because.Argument(() => criteria, "cannot be empty");

      _criteria = criteria;
    }

    public bool SatisfiedBy(string str, int index, int length, System.Globalization.UnicodeCategory category) {
      for(var i = 0; i < _criteria.Length; i++)
        if(_criteria[i].SatisfiedBy(str, index, length, category))
          return true;

      return false;
    }
  }
}
