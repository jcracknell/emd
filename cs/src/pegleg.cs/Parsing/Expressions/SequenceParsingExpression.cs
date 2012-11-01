﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class SequenceParsingExpression : BaseParsingExpression {
		protected readonly IParsingExpression[] _sequence;

		public SequenceParsingExpression(IParsingExpression[] sequence)
			: base(ParsingExpressionKind.OrderedChoice)
		{
			CodeContract.ArgumentIsNotNull(() => sequence, sequence);
			CodeContract.ArgumentIsValid(() => sequence, sequence.Length >= 2, "must have length of at least two");

			_sequence = sequence;
		}

		public IEnumerable<IParsingExpression> Sequence { get { return _sequence; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingSequenceParsingExpression : SequenceParsingExpression, IParsingExpression<Nil> {
		public NonCapturingSequenceParsingExpression(IParsingExpression[] sequence) : base(sequence) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			for(int i = 0; i < _sequence.Length; i++) {
				var currentExpression = _sequence[i];
				var currentExpressionApplicationResult = currentExpression.Matches(context);

				if(!currentExpressionApplicationResult.Succeeded)
					return currentExpressionApplicationResult;
			}

			return SuccessfulMatchingResult.NilProduct;
		}
	}

	public class CapturingSequenceParsingExpression<TProduct> : SequenceParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IMatch<SequenceProducts>, TProduct> _matchAction;

		public CapturingSequenceParsingExpression(IParsingExpression[] sequence, Func<IMatch<SequenceProducts>, TProduct> matchAction)
			: base(sequence)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var expressionProducts = new object[_sequence.Length];
			for(int i = 0; i < _sequence.Length; i++) {
				var currentExpression = _sequence[i];

				var currentExpressionApplicationResult = currentExpression.Matches(context);
				
				if(!currentExpressionApplicationResult.Succeeded)
					return currentExpressionApplicationResult;

				expressionProducts[i] = currentExpressionApplicationResult.Product;
			}

			var product = _matchAction(matchBuilder.CompleteMatch(this, new SequenceProducts(expressionProducts)));
			return new SuccessfulMatchingResult(product);
		}
	}
}