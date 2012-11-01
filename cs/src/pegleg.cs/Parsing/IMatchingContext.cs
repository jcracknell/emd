using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing {
	public interface IMatchingContext {
		SourceLocation SourceLocation { get; }
		int Consumed { get; }	
		string Substring(int index, int length);
		bool ConsumesAnyCharacter(out char c);
		bool ConsumesAnyCharacter();
		bool ConsumesMatching(string literal);
		bool ConsumesMatching(Regex regex, out Match match);
		bool ConsumesMatchingCharInRange(char start, char end, out char matched);
		bool AtEndOfInput { get; }
		IMatchingContext Clone();
		void Assimilate(IMatchingContext clone);
		IMatchBuilder StartMatch();
	}
}
