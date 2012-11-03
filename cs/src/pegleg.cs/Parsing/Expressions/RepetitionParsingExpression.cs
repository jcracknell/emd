using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class RepetitionParsingExpression : BaseParsingExpression {
		public const uint UNBOUNDED = 0;

		protected readonly uint _minOccurs;
		protected readonly uint _maxOccurs;
		protected readonly IParsingExpression _body;

		public RepetitionParsingExpression(uint minOccurs, uint maxOccurs, IParsingExpression body)
			: base(ParsingExpressionKind.Repetition)
		{
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsValid(() => maxOccurs, UNBOUNDED == maxOccurs || maxOccurs >= minOccurs, "must be greater than or equal to minOccurs");

			_minOccurs = minOccurs;
			_maxOccurs = maxOccurs;
			_body = body;
		}

		public uint MinOccurs { get { return _minOccurs; } }
		public uint MaxOccurs { get { return _maxOccurs; } }
		public IParsingExpression Body { get { return _body; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingRepetitionParsingExpression<TBody> : RepetitionParsingExpression, IParsingExpression<IEnumerable<TBody>> {
		public NonCapturingRepetitionParsingExpression(uint minOccurs, uint maxOccurs, IParsingExpression body)
			: base(minOccurs, maxOccurs, body)
		{ }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			uint iterationCount = 0;
			var iterationProducts = new LinkedList<TBody>();
			while(true) {
				var iterationContext = context.Clone();
				var iterationResult = _body.Matches(iterationContext);

				if(iterationResult.Succeeded) {
					iterationProducts.AddLast((TBody)iterationResult.Product);
					context.Assimilate(iterationContext);
					iterationCount++;

					if(_maxOccurs == iterationCount)
						break;
				} else {
					if(_minOccurs > iterationCount)
						return new UnsuccessfulMatchingResult();
					break;
				}
			}

			return new SuccessfulMatchingResult(iterationProducts);
		}
	}

	public class CapturingRepetitionParsingExpression<TBody, TProduct> : RepetitionParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IMatch<IEnumerable<TBody>>, TProduct> _matchAction;

		public CapturingRepetitionParsingExpression(uint minOccurs, uint maxOccurs, IParsingExpression<TBody> body, Func<IMatch<IEnumerable<TBody>>, TProduct> matchAction)
			: base(minOccurs, maxOccurs, body)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;	
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();
			
			uint iterationCount = 0;
			var iterationProducts = new LinkedList<TBody>();
			while(true) {
				var iterationContext = context.Clone();
				var iterationResult = _body.Matches(iterationContext);

				if(iterationResult.Succeeded) {
					iterationProducts.AddLast((TBody)iterationResult.Product);
					context.Assimilate(iterationContext);
					iterationCount++;

					if(_maxOccurs == iterationCount)
						break;
				} else {
					if(_minOccurs > iterationCount)
						return new UnsuccessfulMatchingResult();
					break;
				}
			}

			var product = _matchAction(matchBuilder.CompleteMatch(this, iterationProducts));
			return new SuccessfulMatchingResult(product);
		}
	}
}
