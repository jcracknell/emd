using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public enum ParsingExpressionKind {
		CharacterRange,
		Closure,
		EndOfInput,
		Literal,
		Lookahead,
		NegativeLookahead,
		Named,
		Nothing,
		Optional,
		OrderedChoice,
		Predicate,
		Reference,
		Regex,
		Repetition,
		Sequence,
		UnorderedChoice,
		UntilChar,
		UntilString,
		Wildcard
	}
}
