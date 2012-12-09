using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing {
	public class MatchingContext {
		private readonly string _consumable;
		private readonly SourceRange[] _parts;
		private readonly MatchingResultCache _cache;
		private int _index;
		private int _sourceIndex;
		private int _sourceLine;
		private int _sourceLineIndex;
		private int _part;
		private int _partEnd;

		public MatchingContext(string source)
			: this(source, new SourceRange[] { new SourceRange(0, source.Length, 1, 0) })
		{ }

		public MatchingContext(string source, SourceRange[] parts) {
			if(null == source) throw ExceptionBecause.ArgumentNull(() => source);
			if(null == parts) throw ExceptionBecause.ArgumentNull(() => parts);

			_consumable = source;
			_parts = parts;
			_cache = new MatchingResultCache(source.Length + 1);
			_index = 0;
			_part = 0;
			_partEnd = _parts[_part].Length;
			_sourceIndex = _parts[_part].Index;
			_sourceLine = _parts[_part].Line;
			_sourceLineIndex = _parts[_part].LineIndex;
		}

		private MatchingContext(MatchingContext prototype) {
			_consumable = prototype._consumable;
			_parts = prototype._parts;
			_cache = prototype._cache;
			_index = prototype._index;
			_part = prototype._part;
			_partEnd = prototype._partEnd;
			_sourceIndex = prototype._sourceIndex;
			_sourceLine = prototype._sourceLine;
			_sourceLineIndex = prototype._sourceLineIndex;
		}

		/// <remarks>
		/// Useful for inspection in the debugger.
		/// </remarks>
		protected string Unconsumed { get { return _consumable.Substring(_index); } }

		public int Index { get { return _index; } }

		public int SourceIndex { get { return _sourceIndex; } }

		public int SourceLine { get { return _sourceLine; } }
	
		public int SourceLineIndex { get { return _sourceLineIndex; } }

		private void Consume(int length) {
			for(; length != 0; length--) {
				var c = _consumable[_index];
				
				_index++;
				_sourceIndex++;

				// If we have reached the end of the current part then our source location
				// is the beginning of the next part
				if(_index == _partEnd && _consumable.Length > _index) {
					var newPart = _parts[++_part];

					_partEnd = _partEnd + newPart.Length;
					_sourceIndex = newPart.Index;
					_sourceLine = newPart.Line;
					_sourceLineIndex = newPart.LineIndex;
				} else {
					if('\n' == c) {
						_sourceLine++;
						_sourceLineIndex = 0;
					} else {
						_sourceLineIndex++;
					}
				}
			}
		}

		public string Substring(int index, int length) {
			return _consumable.Substring(index, length);
		}

		public bool ConsumesMatching(string literal) {
			if(null == literal) throw ExceptionBecause.ArgumentNull(() => literal);
			
			var len = literal.Length;
			if(len > _consumable.Length - _index)
				return false;

			if(0 == string.Compare(_consumable, _index, literal, 0, len)) {
				Consume(len);
				return true;
			} else {
				return false;
			}
		}

		public bool ConsumesMatching(Regex regex) {
			if(null == regex) throw ExceptionBecause.ArgumentNull(() => regex);

			var match = regex.Match(_consumable, _index);
			if(match.Success && match.Index == _index) {
				Consume(match.Length);
				return true;
			}

			return false;
		}

		public bool ConsumesMatching(Regex regex, out Match match) {
			if(null == regex) throw ExceptionBecause.ArgumentNull(() => regex);

			match = regex.Match(_consumable, _index);
			if(match.Success && match.Index == _index) {
				Consume(match.Length);
				return true;
			} else {
				match = null;
				return false;
			}
		}

		public bool ConsumesUnicodeCriteria(UnicodeCriteria criteria, out int length) {
			if(null == criteria) throw ExceptionBecause.ArgumentNull(() => criteria);

			if(AtEndOfInput) {
				length = 0;
				return false;
			}

			if(criteria.Accepts(_consumable, _index, out length)) {
				Consume(length);
				return true;
			} else {
				return false;
			}
		}

		public bool ConsumesCharacter(bool[] acceptanceMap, int offset) {
			if(AtEndOfInput) return false;

			var cv = _consumable[_index] - offset;
			if(0 <= cv && cv < acceptanceMap.Length && acceptanceMap[cv]) {
				Consume(1);
				return true;
			} else {
				return false;
			}
		}

		public bool ConsumesAnyCharacter(out char c) {
			if(AtEndOfInput) {
				c = (char)0;
				return false;
			} else {
				c = _consumable[_index];
				Consume(1);
				return true;
			}
		}

		public bool ConsumesAnyCharacter() {
			if(AtEndOfInput) return false;

			Consume(1);
			return true;
		}

		public bool AtEndOfInput { get { return _consumable.Length == _index; } }

		public MatchingContext Clone() {
			return new MatchingContext(this);
		}

		public void Assimilate(MatchingContext clone) {
			_index = clone._index;
			_part = clone._part;
			_partEnd = clone._partEnd;
			_sourceIndex = clone._sourceIndex;
			_sourceLine = clone._sourceLine;
			_sourceLineIndex = clone._sourceLineIndex;
		}

		public MatchBuilder<TProduct> GetMatchBuilderFor<TProduct>(IParsingExpression<TProduct> expression) {
			return new MatchBuilder<TProduct>(this, expression);
		}

		public bool ConsumesCachedResultFor<TProduct>(IParsingExpression<TProduct> expression, out IMatchingResult<TProduct> cachedResult) {
			if(!_cache.HasResult(_index, expression, out cachedResult))
				return false;

			Consume(cachedResult.Length);
			return true;
		}

		public void CacheResultFor<TProduct>(IParsingExpression<TProduct> expression, IMatchingResult<TProduct> result) {
			_cache.Store(_index - result.Length, expression, result);
		}
	}
}
