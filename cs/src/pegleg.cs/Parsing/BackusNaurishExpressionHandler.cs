using pegleg.cs.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class BackusNaurishExpressionHandler : IParsingExpressionHandler<string> {
		public class PrecedenceExpressionHandler : IParsingExpressionHandler<int> {
			public int Handle<TBody,TProduct>(AheadParsingExpression<TBody,TProduct> expression) { return 3; }
			public int Handle(CharacterRangeParsingExpression expression) { return 4; }
			public int Handle<TProduct>(DynamicParsingExpression<TProduct> expression) { return 100; }
			public int Handle(EndOfInputParsingExpression expression) { return 4; }
			public int Handle<TProduct>(LiteralParsingExpression<TProduct> expression) { return 4; }
			public int Handle<TProduct>(NamedParsingExpression<TProduct> expression) { return 4; }
			public int Handle(NotAheadParsingExpression expression) { return 3; }
			public int Handle<TBody,TProduct>(OptionalParsingExpression<TBody,TProduct> expression) { return 2; }
			public int Handle<TChoice,TProduct>(OrderedChoiceParsingExpression<TChoice,TProduct> expression) { return 0; }
			public int Handle(PredicateParsingExpression expression) { return 4; }
			public int Handle<TReferenced,TProduct>(ReferenceParsingExpression<TReferenced,TProduct> expression) { return 100; }
			public int Handle<TProduct>(RegexParsingExpression<TProduct> expression) { return 4; }
			public int Handle<TBody,TProduct>(RepetitionParsingExpression<TBody,TProduct> expression) { return 2; }
			public int Handle<TProduct>(SequenceParsingExpression<TProduct> expression) { return 1; }
			public int Handle<TChoice,TProduct>(UnorderedChoiceParsingExpression<TChoice,TProduct> expression) { return 0; }
			public int Handle(WildcardParsingExpression expression) { return 4; }
		}

		private readonly PrecedenceExpressionHandler _precedenceHandler = new PrecedenceExpressionHandler();

		private string Parenthesized(string s) {
			return string.Concat("( ", s, " )");
		}

		private int PrecedenceOf(IParsingExpression expression) {
			return expression.HandleWith(_precedenceHandler);
		}

		public string Handle<TChoice,TProduct>(OrderedChoiceParsingExpression<TChoice,TProduct> expression) {
			var precedence = expression.HandleWith(_precedenceHandler);
			var choiceStrings = expression.Choices.Select(childExpression => {
				var childPrecedence = childExpression.HandleWith(_precedenceHandler);
				var childString = childExpression.HandleWith(this);

				return precedence >= childPrecedence
					? Parenthesized(childString)
					: childString;
			});

			return string.Join(" / ", choiceStrings.ToArray());
		}

		public string Handle<TChoice,TProduct>(UnorderedChoiceParsingExpression<TChoice,TProduct> expression) {
			var precedence = expression.HandleWith(_precedenceHandler);
			var choiceStrings = expression.Choices.Select(childExpression => {
				var childPrecedence = childExpression.HandleWith(_precedenceHandler);
				var childString = childExpression.HandleWith(this);
				
				return precedence >= childPrecedence
					? Parenthesized(childString)
					: childString;
			});

			return string.Join(" | ", choiceStrings.ToArray());
		}

		public string Handle<TProduct>(LiteralParsingExpression<TProduct> expression) {
			return StringUtils.LiteralEncode(expression.Literal);
		}

		public string Handle<TBody,TProduct>(OptionalParsingExpression<TBody,TProduct> expression) {
			var precedence = expression.HandleWith(_precedenceHandler);
			var childPrecedence = expression.Body.HandleWith(_precedenceHandler);
			var childString = expression.Body.HandleWith(this);

			return precedence > childPrecedence
				? string.Concat(Parenthesized(childString), "?")
				: string.Concat(childString, "?");
		}

		public string Handle<TProduct>(RegexParsingExpression<TProduct> expression) {
			return string.Concat("/", expression.Regex.ToString(), "/");
		}

		public string Handle<TProduct>(SequenceParsingExpression<TProduct> expression) {
			var precedence = expression.HandleWith(_precedenceHandler);
			var childStrings = expression.Sequence.Select(childExpression => {
				var childPrecedence = childExpression.HandleWith(_precedenceHandler);
				var childString = childExpression.HandleWith(this);

				return precedence > childPrecedence
					? Parenthesized(childString)
					: childString;
			});

			return string.Join(" ", childStrings.ToArray());
		}

		public string Handle<TBody,TProduct>(AheadParsingExpression<TBody,TProduct> expression) {
			var childString = expression.Body.HandleWith(this);

			if(PrecedenceOf(expression) > PrecedenceOf(expression.Body))
				childString = Parenthesized(childString);

			return string.Concat("&", childString);
		}


		public string Handle(NotAheadParsingExpression expression) {
			var childString = expression.Body.HandleWith(this);

			if(PrecedenceOf(expression) > PrecedenceOf(expression.Body))
				childString = Parenthesized(childString);

			return string.Concat("!", childString);
		}

		public string Handle<TProduct>(NamedParsingExpression<TProduct> expression) {
			return string.Concat(expression.Name);
		}

		public string Handle<TBody,TProduct>(RepetitionParsingExpression<TBody,TProduct> expression) {
			var childString = expression.Body.HandleWith(this);

			if(PrecedenceOf(expression) > PrecedenceOf(expression.Body))
				childString = Parenthesized(childString);

			if(0 == expression.MaxOccurs) {
				if(0 == expression.MinOccurs) return string.Concat(childString, "*");
				if(1 == expression.MinOccurs) return string.Concat(childString, "+");
				return string.Concat(childString, "{", expression.MinOccurs, ",}");
			}

			return string.Concat(childString, "{", expression.MinOccurs, ",", expression.MaxOccurs, "}");
		}

		public string Handle<TProduct>(DynamicParsingExpression<TProduct> expression) {
			return "<DYNAMIC>";
		}

		public string Handle<TReferenced,TProduct>(ReferenceParsingExpression<TReferenced,TProduct> expression) {
			return expression.Referenced.HandleWith(this);
		}

		public string Handle(EndOfInputParsingExpression expression) {
			return "<EOF>";
		}

		public string Handle(WildcardParsingExpression expression) {
			return "<.>";
		}

		public string Handle(CharacterRangeParsingExpression expression) {
			return string.Concat(Charcode(expression.RangeStart), "-", Charcode(expression.RangeEnd));
		}

		private string Charcode(char c) {
			if(char.IsLetterOrDigit(c))
				return string.Concat("'", c, "'");
			return string.Concat("\\", (int)c);
		}

		public string Handle(PredicateParsingExpression expression) {
			return "<PREDICATE>";
		}
	}
}
