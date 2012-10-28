using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions.Builders {
	public class UpcastedExpressionMatch<TSource, TTarget> : IExpressionMatch<TTarget> {
		private readonly IExpressionMatch<TSource> _source;
		private readonly Func<TSource, TTarget> _upcast;
		private TTarget _upcastedProduct;
		private bool _cached = false;

		public UpcastedExpressionMatch(IExpressionMatch<TSource> source, Func<TSource, TTarget> upcast) {
			CodeContract.ArgumentIsNotNull(() => source, source);
			CodeContract.ArgumentIsNotNull(() => upcast, upcast);

			_source = source;
			_upcast = upcast;
		}

		public SourceRange SourceRange { get { return _source.SourceRange; } }

		public int Index { get { return _source.Index; } }

		public int Length { get { return _source.Length; } }

		public TTarget Product { 
			get {
				if(!_cached) {
					_upcastedProduct = _upcast(_source.Product);
					_cached = true;
				}

				return _upcastedProduct;
			}
		}

		public IParsingExpression Expression { get { return _source.Expression; } }

		public string String { get { return _source.String; } }
	}
}
