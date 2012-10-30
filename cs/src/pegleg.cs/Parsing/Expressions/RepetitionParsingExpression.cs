using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class RepetitionParsingExpression : BaseParsingExpression {
		public RepetitionParsingExpression() : base(ParsingExpressionKind.Repetition) { }

		public abstract uint MinOccurs { get; }
		public abstract uint MaxOccurs { get; }
		public abstract IParsingExpression Body { get; }
	}

	public class RepetitionParsingExpression<TProduct> : RepetitionParsingExpression, IParsingExpression<TProduct> {
		public const uint UNBOUNDED = 0;
		private readonly uint _minOccurs;
		private readonly uint _maxOccurs;
		private readonly IParsingExpression _body;
		private readonly Func<IExpressionMatch<object[]>, TProduct> _matchAction;

		public RepetitionParsingExpression(uint minOccurs, uint maxOccurs, IParsingExpression body, Func<IExpressionMatch<object[]>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsValid(() => maxOccurs, UNBOUNDED == maxOccurs || maxOccurs >= minOccurs, "must be greater than or equal to minOccurs");

			_minOccurs = minOccurs;
			_maxOccurs = maxOccurs;
			_body = body;
			_matchAction = matchAction;	
		}

		public override uint MinOccurs { get { return _minOccurs; } }

		public override uint MaxOccurs { get { return _maxOccurs; } }

		public override IParsingExpression Body { get { return _body; } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();
			
			uint iterationCount = 0;
			var iterationProducts = 0 == _maxOccurs ? new List<object>() : new List<object>((int)_maxOccurs);
			while(true) {
				if(iterationCount > 1000)
					"this would be a good place to put a breakpoint".ToString();

				var iterationContext = context.Clone();
				var iterationResult = _body.Matches(iterationContext);

				if(iterationResult.Succeeded) {
					iterationProducts.Add(iterationResult.Product);
					context.Assimilate(iterationContext);
					iterationCount++;

					if(_maxOccurs == iterationCount)
						break;
				} else {
					if(_minOccurs <= iterationCount)
						break;

					return new UnsuccessfulMatchingResult();
				}
			}

			var product = _matchAction(matchBuilder.CompleteMatch(this, iterationProducts.ToArray()));
			return new SuccessfulMatchingResult(product);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
