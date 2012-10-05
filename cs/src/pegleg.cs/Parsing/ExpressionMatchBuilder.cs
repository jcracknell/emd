﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class ExpressionMatchBuilder : IExpressionMatchBuilder {
		private readonly IExpressionMatchingContext _matchingContext;
		private readonly SourceLocation _initialSourceLocation;
		private readonly int _initialConsumed;

		public ExpressionMatchBuilder(IExpressionMatchingContext context) {
			_matchingContext = context;
			_initialConsumed = context.Consumed;
			_initialSourceLocation = context.SourceLocation;
		}

		public IExpressionMatch<TProduct> CompleteMatch<TProduct>(IExpression expression, TProduct product) {
			var matchLength = _matchingContext.Consumed - _initialConsumed;
			var matchSourceRange = new SourceRange(
				_initialSourceLocation,
				_matchingContext.SourceLocation.Index - _initialSourceLocation.Index);

			return new ExpressionMatch<TProduct>(_matchingContext, expression, product, _initialConsumed, matchLength, matchSourceRange);
		}
	}
}
