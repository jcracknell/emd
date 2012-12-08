using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class SequenceParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		protected readonly IParsingExpression[] _sequence;

		public SequenceParsingExpression(IParsingExpression[] sequence) {
			if(null == sequence) throw ExceptionBecause.ArgumentNull(() => sequence);
			if(!(sequence.Length >= 2)) throw ExceptionBecause.Argument(() => sequence, "must have length of at least two");

			_sequence = sequence;
		}

		public IEnumerable<IParsingExpression> Sequence { get { return _sequence; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingSequenceParsingExpression : SequenceParsingExpression<Nil> {
		public NonCapturingSequenceParsingExpression(IParsingExpression[] sequence) : base(sequence) { }

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);

			for(int i = 0; i < _sequence.Length; i++) {
				var currentExpression = _sequence[i];
				var currentExpressionApplicationResult = currentExpression.Matches(context);

				if(!currentExpressionApplicationResult.Succeeded)
					return UnsuccessfulMatchingResult.Create(this);
			}

			return matchBuilder.Success(Nil.Value);
		}
	}

	public class CapturingSequenceParsingExpression<TProduct> : SequenceParsingExpression<TProduct> {
		private readonly Func<IMatch<SequenceProducts>, TProduct> _matchAction;

		public CapturingSequenceParsingExpression(IParsingExpression[] sequence, Func<IMatch<SequenceProducts>, TProduct> matchAction)
			: base(sequence)
		{
			if(null == matchAction) throw ExceptionBecause.ArgumentNull(() => matchAction);

			_matchAction = matchAction;
		}

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);

			var expressionProducts = new object[_sequence.Length];
			for(int i = 0; i < _sequence.Length; i++) {
				var currentExpression = _sequence[i];

				var currentExpressionApplicationResult = currentExpression.Matches(context);
				
				if(!currentExpressionApplicationResult.Succeeded)
					return UnsuccessfulMatchingResult.Create(this);

				expressionProducts[i] = currentExpressionApplicationResult.Product;
			}

			return matchBuilder.Success(new SequenceProducts(expressionProducts), _matchAction);
		}
	}
}
