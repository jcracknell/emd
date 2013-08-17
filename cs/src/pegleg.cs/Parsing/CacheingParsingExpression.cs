using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
  public abstract class CacheingParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
    public override IMatchingResult<TProduct> Matches(MatchingContext context) {
      IMatchingResult<TProduct> cachedResult;
      if(context.ConsumesCachedResultFor(this, out cachedResult))
        return cachedResult;

      cachedResult = MatchesUncached(context);
      context.CacheResultFor(this, cachedResult);

      return cachedResult;
    }

    protected abstract IMatchingResult<TProduct> MatchesUncached(MatchingContext context);
  }
}
