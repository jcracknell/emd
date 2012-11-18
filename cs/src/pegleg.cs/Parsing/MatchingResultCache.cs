﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class MatchingResultCache {
		private readonly Dictionary<Guid,IMatchingResult>[] _cache;
		private int _minIndex = int.MaxValue;

		public MatchingResultCache(int size) {
			_cache = new Dictionary<Guid,IMatchingResult>[size];
		}

		public bool HasResult<TProduct>(int index, IParsingExpression<TProduct> expression, out IMatchingResult<TProduct> cachedResult) {
			var indexCache = _cache[index];
			if(null == indexCache) {
				cachedResult = null;
				return false;
			}

			IMatchingResult stored;
			if(indexCache.TryGetValue(expression.Id, out stored)) {
				cachedResult = (IMatchingResult<TProduct>)stored;
				return true;
			}

			cachedResult = null;
			return false;
		}

		public void Store(int index, IParsingExpression expression, IMatchingResult result) {
			var indexCache = _cache[index];
			if(null == indexCache)
				_cache[index] = indexCache = new Dictionary<Guid,IMatchingResult>();

			indexCache[expression.Id] = result;
			
			if(index < _minIndex)
				_minIndex = index;
		}

		public void ClearPriorTo(int index) {
			while(_minIndex < index)
				_cache[_minIndex++] = null;
		}
	}
}
