using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public enum ExpressionType {
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
		Wildcard
	}
}
