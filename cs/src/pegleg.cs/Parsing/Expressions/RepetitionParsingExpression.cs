using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class RepetitionParsingExpression : BaseParsingExpression {
		public RepetitionParsingExpression() : base(ParsingExpressionKind.Repetition) { }

		public const uint UNBOUNDED = 0;

		public abstract uint MinOccurs { get; }
		public abstract uint MaxOccurs { get; }
		public abstract IParsingExpression Body { get; }
	}

	public class RepetitionParsingExpression<TBody, TProduct> : RepetitionParsingExpression, IParsingExpression<TProduct> {
		private readonly uint _minOccurs;
		private readonly uint _maxOccurs;
		private readonly IParsingExpression<TBody> _body;
		private readonly Func<IMatch<TBody[]>, TProduct> _matchAction;

		public RepetitionParsingExpression(uint minOccurs, uint maxOccurs, IParsingExpression<TBody> body, Func<IMatch<TBody[]>, TProduct> matchAction) {
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
			var iterationProducts = new LinkedList<TBody>();
			while(true) {
				if(iterationCount > 1000)
					"this would be a good place to put a breakpoint".ToString();

				var iterationContext = context.Clone();
				var iterationResult = _body.Matches(iterationContext);

				if(iterationResult.Succeeded) {
					iterationProducts.AddLast((TBody)iterationResult.Product);
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

			var iterationProductsArray = iterationProducts.ToArray();

			if(null == _matchAction)
				return new SuccessfulMatchingResult(iterationProductsArray);

			var product = _matchAction(matchBuilder.CompleteMatch(this, iterationProductsArray));
			return new SuccessfulMatchingResult(product);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
