using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing {
	public class MatchingContext : IMatchingContext {
		private readonly string _consumable;
		private readonly SourceRange[] _parts;
		private readonly bool _isRootContext;
		private int _consumed;
		private int _sourceIndex;
		private int _sourceLine;
		private int _sourceLineIndex;
		private int _part;
		private int _partEnd;

		public MatchingContext(string source)
			: this(source, new SourceRange[] { new SourceRange(0, source.Length, 1, 0) })
		{ }

		public MatchingContext(string source, SourceRange[] parts) {
			CodeContract.ArgumentIsNotNull(() => source, source);
			CodeContract.ArgumentIsNotNull(() => parts, parts);

			_isRootContext = true;
			_consumable = source;
			_parts = parts;
			_consumed = 0;
			_part = 0;
			_partEnd = _parts[_part].Length;
			_sourceIndex = _parts[_part].Index;
			_sourceLine = _parts[_part].Line;
			_sourceLineIndex = _parts[_part].LineIndex;
		}

		private MatchingContext(
			string source, SourceRange[] parts, int consumed, int part, int partEnd,
			int sourceIndex, int sourceLine, int sourceLineIndex)
		{
			_isRootContext = false;
			_consumable = source;
			_parts = parts;
			_consumed = consumed;
			_part = part;
			_partEnd = partEnd;
			_sourceIndex = sourceIndex;
			_sourceLine = sourceLine;
			_sourceLineIndex = sourceLineIndex;
		}

		// mostly for debugging purposes
		public string Unconsumed { get { return _consumable.Substring(_consumed); } }

		public SourceLocation SourceLocation { get { return new SourceLocation(_sourceIndex, _sourceLine, _sourceLineIndex); } }

		public int Consumed { get { return _consumed; } }

		private void Consume(int length) {
			for(; length != 0; length--) {
				var c = _consumable[_consumed];
				
				_consumed++;
				_sourceIndex++;

				// If we have reached the end of the current part then our source location
				// is the beginning of the next part
				if(_consumed == _partEnd && _consumable.Length > _consumed) {
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

		public bool ConsumesMatching(string literal, StringComparison comparison) {
			CodeContract.ArgumentIsNotNull(() => literal, literal);
			
			var len = literal.Length;
			if(len > _consumable.Length - _consumed)
				return false;

			if(0 == string.Compare(_consumable, _consumed, literal, 0, len, comparison)) {
				Consume(len);
				return true;
			} else {
				return false;
			}
		}

		public bool ConsumesMatching(Regex regex) {
			CodeContract.ArgumentIsNotNull(() => regex, regex);

			var match = regex.Match(_consumable, _consumed);
			if(match.Success && match.Index == _consumed) {
				Consume(match.Length);
				return true;
			}

			return false;
		}

		public bool ConsumesMatching(Regex regex, out Match match) {
			CodeContract.ArgumentIsNotNull(() => regex, regex);

			match = regex.Match(_consumable, _consumed);
			if(match.Success && match.Index == _consumed) {
				Consume(match.Length);
				return true;
			} else {
				match = null;
				return false;
			}
		}

		public bool ConsumesCharInRange(char start, char end) {
			if(AtEndOfInput) return false;

			var c = _consumable[_consumed];

			if(start <= c && c <= end) {
				Consume(1);
				return true;
			}

			return false;
		}

		public bool ConsumesCharInRange(char start, char end, out char matched) {
			matched = (char)0;

			if(AtEndOfInput)
				return false;

			var c = _consumable[_consumed];
			if(start <= c && c <= end) {
				matched = c;
				Consume(1);
				return true;
			}

			return false;
		}

		public bool ConsumesAnyCharacter(out char c) {
			if(AtEndOfInput) {
				c = (char)0;
				return false;
			} else {
				c = _consumable[_consumed];
				Consume(1);
				return true;
			}
		}

		public bool ConsumesAnyCharacter() {
			if(AtEndOfInput) return false;

			Consume(1);
			return true;
		}

		public bool AtEndOfInput { get { return _consumable.Length == _consumed; } }

		public IMatchingContext Clone() {
			return new MatchingContext(
				_consumable, _parts, _consumed, _part, _partEnd, _sourceIndex, _sourceLine, _sourceLineIndex);
		}

		public void Assimilate(IMatchingContext clone) {
			Consume(clone.Consumed - _consumed);
		}

		public IMatchBuilder StartMatch() {
			return new MatchBuilder(this);
		}

	}
}
