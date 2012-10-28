using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class BackusNaurishExpressionHandler : IParsingExpressionHandler<string> {
		public class PrecedenceExpressionHandler : IParsingExpressionHandler<int> {
			public int Handle(DynamicParsingExpression expression) { return 100; }
			public int Handle(ReferenceParsingExpression expression) { return 100; }
			public int Handle(PredicateParsingExpression expression) { return 4; }
			public int Handle(CharacterRangeParsingExpression expression) { return 4; }
			public int Handle(LiteralParsingExpression expression) { return 4; }
			public int Handle(NothingParsingExpression expression) { return 4; }
			public int Handle(EndOfInputParsingExpression expression) { return 4; }
			public int Handle(WildcardParsingExpression expression) { return 4; }
			public int Handle(RegexParsingExpression expression) { return 4; }
			public int Handle(NamedParsingExpression expression) { return 4; }
			public int Handle(AheadExpression expression) { return 3; }
			public int Handle(NotAheadParsingExpression expression) { return 3; }
			public int Handle(OptionalParsingExpression expression) { return 2; }
			public int Handle(RepetitionParsingExpression expression) { return 2; }
			public int Handle(SequenceParsingExpression expression) { return 1; }
			public int Handle(OrderedChoiceParsingExpression expression) { return 0; }
			public int Handle(UnorderedChoiceParsingExpression expression) { return 0; }
		}

		private readonly PrecedenceExpressionHandler _precedenceHandler = new PrecedenceExpressionHandler();

		private string Parenthesized(string s) {
			return string.Concat("( ", s, " )");
		}

		private int PrecedenceOf(IParsingExpression expression) {
			return expression.HandleWith(_precedenceHandler);
		}

		public string Handle(OrderedChoiceParsingExpression expression) {
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

		public string Handle(UnorderedChoiceParsingExpression expression) {
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

		public string Handle(LiteralParsingExpression expression) {
			return StringUtils.LiteralEncode(expression.Literal);
		}

		public string Handle(NothingParsingExpression expression) {
			return "<0>";
		}

		public string Handle(OptionalParsingExpression expression) {
			var precedence = expression.HandleWith(_precedenceHandler);
			var childPrecedence = expression.Body.HandleWith(_precedenceHandler);
			var childString = expression.Body.HandleWith(this);

			return precedence > childPrecedence
				? string.Concat(Parenthesized(childString), "?")
				: string.Concat(childString, "?");
		}

		public string Handle(RegexParsingExpression expression) {
			return string.Concat("/", expression.Regex.ToString(), "/");
		}

		public string Handle(SequenceParsingExpression expression) {
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

		public string Handle(AheadExpression expression) {
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

		public string Handle(NamedParsingExpression expression) {
			return string.Concat(expression.Name);
		}

		public string Handle(RepetitionParsingExpression expression) {
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

		public string Handle(DynamicParsingExpression expression) {
			return "<DYNAMIC>";
		}

		public string Handle(ReferenceParsingExpression expression) {
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
