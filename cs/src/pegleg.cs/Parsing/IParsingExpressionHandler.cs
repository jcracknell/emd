using pegleg.cs.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public interface IParsingExpressionHandler<T> {
		T Handle<TBody, TProduct>(AheadParsingExpression<TBody,TProduct> expression);
		T Handle(CharacterSetParsingExpression expression);
		T Handle<TProduct>(DynamicParsingExpression<TProduct> expression);
		T Handle<TProduct>(LiteralParsingExpression<TProduct> expression);
		T Handle(EndOfInputParsingExpression expression);
		T Handle<TProduct>(NamedParsingExpression<TProduct> expression);
		T Handle(NotAheadParsingExpression expression);
		T Handle<TBody,TProduct>(OptionalParsingExpression<TBody,TProduct> expression);
		T Handle<TChoice,TProduct>(OrderedChoiceParsingExpression<TChoice,TProduct> expression);
		T Handle(PredicateParsingExpression expression);
		T Handle<TReferenced,TProduct>(ReferenceParsingExpression<TReferenced, TProduct> expression);
		T Handle<TProduct>(RegexParsingExpression<TProduct> expression);
		T Handle<TBody,TProduct>(RepetitionParsingExpression<TBody,TProduct> expression);
		T Handle<TProduct>(SequenceParsingExpression<TProduct> expression);
		T Handle<TChoice,TProduct>(UnorderedChoiceParsingExpression<TChoice, TProduct> expression);
		T Handle(WildcardParsingExpression expression);
	}
}
