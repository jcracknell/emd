using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
  public class MatchingResultCache {
    private readonly Dictionary<int,IMatchingResult>[] _cache;
    private static int _queries = 0;
    private static int _hits = 0;

    public MatchingResultCache(int size) {
      _cache = new Dictionary<int,IMatchingResult>[size];
    }

    public bool HasResult<TProduct>(int index, IParsingExpression<TProduct> expression, out IMatchingResult<TProduct> cachedResult) {
      _queries++;
      var indexCache = _cache[index];
      if(null == indexCache) {
        cachedResult = null;
        return false;
      }

      IMatchingResult stored;
      if(indexCache.TryGetValue(expression.Id, out stored)) {
        _hits++;
        cachedResult = (IMatchingResult<TProduct>)stored;
        return true;
      }

      cachedResult = null;
      return false;
    }

    public void Store(int index, IParsingExpression expression, IMatchingResult result) {
      var indexCache = _cache[index];
      if(null == indexCache)
        _cache[index] = indexCache = new Dictionary<int,IMatchingResult>();

      indexCache[expression.Id] = result;
    }
  }
}
