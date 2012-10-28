using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface UnorderedChoiceParsingExpression : IParsingExpression {
		IEnumerable<IParsingExpression> Choices { get; }
	}

	public class UnorderedChoiceParsingExpression<TProduct> : UnorderedChoiceParsingExpression, IParsingExpression<TProduct> {
		private readonly IParsingExpression[] _choices;
		private readonly int _choiceCount;
		private readonly int[] _choiceOrder;
		private readonly uint[] _choiceHits;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;

		public UnorderedChoiceParsingExpression(IParsingExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => choices, choices);
			CodeContract.ArgumentIsValid(() => choices, choices.Length >= 2, "must have length of at least two");
			
			_choices = choices;
			_choiceCount = choices.Length;
			_choiceOrder = new int[_choiceCount];
			_choiceHits = new uint[_choiceCount];
			_matchAction = matchAction;

			for(int i = 0; i != _choiceCount; i++)
				_choiceOrder[i] = i;
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.UnorderedChoice; } }

		public IEnumerable<IParsingExpression> Choices { get { return _choices; } }

		/// <summary>
		/// The current ordering of choices by success count.
		/// Mostly for inspection while debugging.
		/// </summary>
		public IEnumerable<Tuple<uint, IParsingExpression>> CurrentChoiceOrder {
			get { return _choiceOrder.Select(i => Tuple.Create(_choiceHits[i], _choices[i])).ToArray(); }
		}

		public IMatchingResult Match(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			for(var i = 0; i != _choiceCount; i++) {
				// Ordering of choices is determined by the choice order array
				var currentChoice = _choices[_choiceOrder[i]];

				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = currentChoice.Match(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					UpdateChoiceOrder(i);

					context.Assimilate(choiceMatchingContext);
					return new SuccessfulMatchingResult(
						null != _matchAction
							? _matchAction(matchBuilder.CompleteMatch(this, choiceMatchingResult.Product))
							: choiceMatchingResult.Product
					);
				}
			}

			return new UnsuccessfulMatchingResult();
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

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
