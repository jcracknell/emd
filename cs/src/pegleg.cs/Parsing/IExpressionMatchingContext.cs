using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing {
	public interface IExpressionMatchingContext {
		SourceLocation SourceLocation { get; }
		int Consumed { get; }	
		string Substring(int index, int length);
		bool TryConsumeAnyCharacter(out char c);
		bool TryConsumeMatching(string literal);
		bool TryConsumeMatching(Regex regex, out Match match);
		bool TryConsumeMatchingCharInRange(char start, char end, out char matched);
		bool AtEndOfInput { get; }
		IExpressionMatchingContext Clone();
		void Assimilate(IExpressionMatchingContext clone);
		IExpressionMatchBuilder StartMatch();
	}
}
