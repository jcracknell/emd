using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs {
	public class RuleApplicationContext : IRuleApplicationContext {
		private string _inputString;
		private int _remaining;
		private int _index = 0;
		private int _line = 0;
		private int _lineIndex = 0;

		public RuleApplicationContext(string inputString) {
			CodeContract.ArgumentIsNotNull(() => inputString, inputString);

			_inputString = inputString;
			_remaining = _inputString.Length;
		}

		private RuleApplicationContext(string inputString, int remaining, int index, int line, int lineIndex) {
			CodeContract.ArgumentIsNotNull(() => inputString, inputString);

			_inputString = inputString;
			_remaining = remaining;
			_index = index;
			_line = line;
			_lineIndex = lineIndex;
		}

		public void Consume(int n) {
			CodeContract.ArgumentIsValid(() => n, n > 0, "must be a positive integer");

			if(n > _remaining)
				throw new Exception();

			// Walk throught the consumed characters to keep track of our position
			// in the input string
			int end = _index + n;
			while(_index != end) {
				char c = _inputString[_index++];
				if('\n' == c) {
					_line++;
					_lineIndex = 0;
				} else {
					_lineIndex++;
				}
			}

			_remaining -= n;
		}

		public bool Matches(string literal) {
			CodeContract.ArgumentIsNotNull(() => literal, literal);

			throw new NotImplementedException();
		}

		public bool Matches(Regex re) {
			Match m;
			return Matches(re, out m);
		}

		public bool Matches(Regex re, out Match match) {
			match = re.Match(_inputString, _index, _remaining);
			return match.Success;
		}

		public IRuleApplicationContext Clone() {
			return new RuleApplicationContext(_inputString, _remaining, _index, _line, _lineIndex);
		}
	}
}
