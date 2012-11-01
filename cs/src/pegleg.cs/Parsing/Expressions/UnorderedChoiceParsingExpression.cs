﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class UnorderedChoiceParsingExpression : BaseParsingExpression {
		private readonly IParsingExpression[] _choices;
		private readonly int _choiceCount;
		private readonly int[] _choiceOrder;
		private readonly uint[] _choiceHits;

		public UnorderedChoiceParsingExpression(IParsingExpression[] choices)
			: base(ParsingExpressionKind.UnorderedChoice)
		{
			CodeContract.ArgumentIsNotNull(() => choices, choices);
			CodeContract.ArgumentIsValid(() => choices, choices.Length >= 2, "must have length of at least two");
	
			_choices = choices;
			_choiceCount = choices.Length;
			_choiceOrder = new int[_choiceCount];
			_choiceHits = new uint[_choiceCount];

			for(int i = 0; i != _choiceCount; i++)
				_choiceOrder[i] = i;
		}

		public IEnumerable<IParsingExpression> Choices { get { return _choices; } }

		public IEnumerable<Tuple<uint, IParsingExpression>> CurrentChoiceOrder {
			get { return _choiceOrder.Select(i => Tuple.Create(_choiceHits[i], _choices[i])).ToArray(); }
		}

		protected IMatchingResult MatchChoices(IMatchingContext context) {
			for(var i = 0; i != _choiceCount; i++) {
				// Ordering of choices is determined by the choice order array
				var currentChoice = _choices[_choiceOrder[i]];

				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = currentChoice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					UpdateChoiceOrder(i);
					return choiceMatchingResult;
				}
			}

			return null;
		}

		private void UpdateChoiceOrder(int i) {
			var choiceOrderI = _choiceOrder[i];
			var currentChoiceHits = _choiceHits[choiceOrderI];

			// Search for the position where the current choice should go
			// We know that it is somewhere in a sequence of entries with the same hit count
			// (possibly just itself), so we can swap places with the earliest entry with
			// the same hit count
			var j = i;
			while(0 != j && _choiceHits[_choiceOrder[--j]] == currentChoiceHits);
			// |  |  |  |  |  |  i  |  |  |
			// 9, 8, 8, 8, 7, 7, 7, 6, 6, 5
			// |  |  |  j  |  |  |  |  |  |
			if(_choiceHits[_choiceOrder[j]] != currentChoiceHits) j++;
			// |  |  |  |  j  |  |  |  |  |

			// Update the hit count for the choice that matched
			currentChoiceHits++;
			_choiceHits[choiceOrderI] = currentChoiceHits;

			// Swap the positions of i and j in the order
			_choiceOrder[i] = _choiceOrder[j];
			_choiceOrder[j] = choiceOrderI;

			// If we have reached the maximum value for hits, then bitshift the entire
			// hits array to prevent overflow.
			if(uint.MaxValue == currentChoiceHits)
				for(j = 0; j != _choiceCount; j++)
					_choiceHits[j] = _choiceHits[j] >> 1;
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingUnorderedChoiceParsingExpression : UnorderedChoiceParsingExpression, IParsingExpression<Nil> {
		public NonCapturingUnorderedChoiceParsingExpression(IParsingExpression[] choices) : base(choices) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(null == MatchChoices(context))
				return new UnsuccessfulMatchingResult();

			return SuccessfulMatchingResult.NilProduct;
		}
	}

	public class NonCapturingUnorderedChoiceParsingExpression<TChoice> : UnorderedChoiceParsingExpression, IParsingExpression<TChoice> {
		public NonCapturingUnorderedChoiceParsingExpression(IParsingExpression[] choices) : base(choices) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			return MatchChoices(context) ?? new UnsuccessfulMatchingResult();
		}
	}

	public class CapturingUnorderedChoiceParsingExpression<TChoice, TProduct> : UnorderedChoiceParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IMatch<TChoice>, TProduct> _matchAction;

		public CapturingUnorderedChoiceParsingExpression(IParsingExpression[] choices, Func<IMatch<TChoice>, TProduct> matchAction)
			: base(choices)	
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var choiceResult = MatchChoices(context);
			if(null == choiceResult)
				return new UnsuccessfulMatchingResult();

			var product = _matchAction(matchBuilder.CompleteMatch(this, (TChoice)choiceResult.Product));
			return new SuccessfulMatchingResult(product);
		}
	}
}
