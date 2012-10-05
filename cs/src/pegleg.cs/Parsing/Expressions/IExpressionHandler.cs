using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface IExpressionHandler<T> {
		T Handle(CharacterRangeExpression expression);
		T Handle(ChoiceExpression expression);
		T Handle(DynamicExpression expression);
		T Handle(LiteralExpression expression);
		T Handle(AheadExpression expression);
		T Handle(EndOfInputExpression expression);
		T Handle(NamedExpression expression);
		T Handle(NotAheadExpression expression);
		T Handle(NothingExpression expression);
		T Handle(OptionalExpression expression);
		T Handle(PredicateExpression expression);
		T Handle(ReferenceExpression expression);
		T Handle(RegexExpression expression);
		T Handle(RepetitionExpression expression);
		T Handle(SequenceExpression expression);
		T Handle(WildcardExpression expression);
	}
}
