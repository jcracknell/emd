using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface RepetitionExpression : IExpression {
		uint MinOccurs { get; }
		uint MaxOccurs { get; }
		IExpression Body { get; }
	}

	public class RepetitionExpression<TProduct> : RepetitionExpression, IExpression<TProduct> {
		public const uint UNBOUNDED = 0;
		private readonly uint _minOccurs;
		private readonly uint _maxOccurs;
		private readonly IExpression _body;
		private readonly Func<IExpressionMatch<object[]>, TProduct> _matchAction;

		public RepetitionExpression(uint minOccurs, uint maxOccurs, IExpression body, Func<IExpressionMatch<object[]>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsValid(() => maxOccurs, UNBOUNDED == maxOccurs || maxOccurs >= minOccurs, "must be greater than or equal to minOccurs");

			_minOccurs = minOccurs;
			_maxOccurs = maxOccurs;
			_body = body;
			_matchAction = matchAction;	
		}

		public uint MinOccurs { get { return _minOccurs; } }

		public uint MaxOccurs { get { return _maxOccurs; } }

		public IExpression Body { get { return _body; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Repetition; } }

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var matchBuilder = context.StartMatch();
			
			uint iterationCount = 0;
			var iterationProducts = 0 == _maxOccurs ? new List<object>() : new List<object>((int)_maxOccurs);
			while(true) {
				var iterationContext = context.Clone();
				var iterationResult = _body.Match(iterationContext);

				if(iterationResult.Succeeded) {
					iterationProducts.Add(iterationResult.Product);
					context.Assimilate(iterationContext);
					iterationCount++;

					if(_maxOccurs == iterationCount)
						break;
				} else {
					if(_minOccurs <= iterationCount)
						break;

					return new UnsuccessfulExpressionMatchingResult();
				}
			}

			var product = _matchAction(matchBuilder.CompleteMatch(this, iterationProducts.ToArray()));
			return new SuccessfulExpressionMatchingResult(product);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
