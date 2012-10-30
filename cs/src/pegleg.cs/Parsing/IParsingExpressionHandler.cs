using pegleg.cs.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public interface IParsingExpressionHandler<T> {
		T Handle(CharacterRangeParsingExpression expression);
		T Handle(UnorderedChoiceParsingExpression expression);
		T Handle(OrderedChoiceParsingExpression expression);
		T Handle(DynamicParsingExpression expression);
		T Handle(LiteralParsingExpression expression);
		T Handle(AheadExpression expression);
		T Handle(EndOfInputParsingExpression expression);
		T Handle(NamedParsingExpression expression);
		T Handle(NotAheadParsingExpression expression);
		T Handle(OptionalParsingExpression expression);
		T Handle(PredicateParsingExpression expression);
		T Handle(ReferenceParsingExpression expression);
		T Handle(RegexParsingExpression expression);
		T Handle(RepetitionParsingExpression expression);
		T Handle(SequenceParsingExpression expression);
		T Handle(WildcardParsingExpression expression);
	}
}
