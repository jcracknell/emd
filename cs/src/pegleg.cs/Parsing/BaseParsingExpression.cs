﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public abstract class BaseParsingExpression<TProduct> : IParsingExpression<TProduct> {
		private readonly int _id;
		private readonly ParsingExpressionKind _kind;

		public BaseParsingExpression(ParsingExpressionKind kind) {
			_id = ParsingExpressionIdGenerator.GenerateId();
			_kind = kind;
		}

		public int Id { get { return _id; } }

		public ParsingExpressionKind Kind { get { return _kind; } }

		public abstract IMatchingResult<TProduct> Matches(MatchingContext context);

		IMatchingResult IParsingExpression.Matches(MatchingContext context) {
			return Matches(context);
		}

		public abstract T HandleWith<T>(IParsingExpressionHandler<T> handler);

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
