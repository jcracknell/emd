using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class BackusNaurishExpressionHandler : IExpressionHandler<string> {
		public class PrecedenceExpressionHandler : IExpressionHandler<int> {
			public int Handle(DynamicExpression expression) { return 100; }
			public int Handle(ReferenceExpression expression) { return 100; }
			public int Handle(PredicateExpression expression) { return 4; }
			public int Handle(CharacterRangeExpression expression) { return 4; }
			public int Handle(LiteralExpression expression) { return 4; }
			public int Handle(NothingExpression expression) { return 4; }
			public int Handle(EndOfInputExpression expression) { return 4; }
			public int Handle(WildcardExpression expression) { return 4; }
			public int Handle(RegexExpression expression) { return 4; }
			public int Handle(NamedExpression expression) { return 4; }
			public int Handle(AheadExpression expression) { return 3; }
			public int Handle(NotAheadExpression expression) { return 3; }
			public int Handle(OptionalExpression expression) { return 2; }
			public int Handle(RepetitionExpression expression) { return 2; }
			public int Handle(SequenceExpression expression) { return 1; }
			public int Handle(OrderedChoiceExpression expression) { return 0; }
			public int Handle(UnorderedChoiceExpression expression) { return 0; }
		}

		private readonly PrecedenceExpressionHandler _precedenceHandler = new PrecedenceExpressionHandler();

		private string Parenthesized(string s) {
			return string.Concat("( ", s, " )");
		}

		private int PrecedenceOf(IExpression expression) {
			return expression.HandleWith(_precedenceHandler);
		}

		public string Handle(OrderedChoiceExpression expression) {
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

		public string Handle(UnorderedChoiceExpression expression) {
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

		public string Handle(LiteralExpression expression) {
			return StringUtils.LiteralEncode(expression.Literal);
		}

		public string Handle(NothingExpression expression) {
			return "<0>";
		}

		public string Handle(OptionalExpression expression) {
			var precedence = expression.HandleWith(_precedenceHandler);
			var childPrecedence = expression.Body.HandleWith(_precedenceHandler);
			var childString = expression.Body.HandleWith(this);

			return precedence > childPrecedence
				? string.Concat(Parenthesized(childString), "?")
				: string.Concat(childString, "?");
		}

		public string Handle(RegexExpression expression) {
			return string.Concat("/", expression.Regex.ToString(), "/");
		}

		public string Handle(SequenceExpression expression) {
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


		public string Handle(NotAheadExpression expression) {
			var childString = expression.Body.HandleWith(this);

			if(PrecedenceOf(expression) > PrecedenceOf(expression.Body))
				childString = Parenthesized(childString);

			return string.Concat("!", childString);
		}

		public string Handle(NamedExpression expression) {
			return string.Concat(expression.Name);
		}

		public string Handle(RepetitionExpression expression) {
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

		public string Handle(DynamicExpression expression) {
			return "<DYNAMIC>";
		}

		public string Handle(ReferenceExpression expression) {
			return expression.Referenced.HandleWith(this);
		}

		public string Handle(EndOfInputExpression expression) {
			return "<EOF>";
		}

		public string Handle(WildcardExpression expression) {
			return "<.>";
		}

		public string Handle(CharacterRangeExpression expression) {
			return string.Concat(Charcode(expression.RangeStart), "-", Charcode(expression.RangeEnd));
		}

		private string Charcode(char c) {
			if(char.IsLetterOrDigit(c))
				return string.Concat("'", c, "'");
			return string.Concat("\\", (int)c);
		}

		public string Handle(PredicateExpression expression) {
			return "<PREDICATE>";
		}
	}
}
