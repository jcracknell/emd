using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class LiteralSetParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		protected readonly Trie<char, string> _trie;

		public LiteralSetParsingExpression(IEnumerable<string> literals) {
			if(null == literals) throw ExceptionBecause.ArgumentNull(() => literals);
			if(!literals.Any()) throw ExceptionBecause.Argument(() => literals, "cannot be empty");

			_trie = new Trie<char, string>();
			foreach(var literal in literals)
				_trie.SetValue(literal, literal);
		}

		public IEnumerable<string> Literals { get { return _trie.Values.OrderByDescending(v => v.Length); } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingLiteralSetParsingExpression : LiteralSetParsingExpression<Nil> {
		public NonCapturingLiteralSetParsingExpression(IEnumerable<string> literals) : base(literals) { }

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);
			var matchingContext = context.Clone();

			char c;
			string matched = null;
			var suffixTrie = _trie;
			while(matchingContext.ConsumesAnyCharacter(out c) && suffixTrie.TryGetSubtrie(c, out suffixTrie)) {
				// If we encounter a trie node which has value, we have reached the end of a literal in the set
				// however there may be a longer literal which matches
				if(suffixTrie.HasValue) {
					context.Assimilate(matchingContext);
					matched = suffixTrie.Value;
				}
			}

			return null != matched
				? matchBuilder.Success(Nil.Value)
				: UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class CapturingLiteralSetParsingExpression<TProduct> : LiteralSetParsingExpression<TProduct> {
		private readonly Func<IMatch<string>, TProduct> _matchAction;

		public CapturingLiteralSetParsingExpression(IEnumerable<string> literals, Func<IMatch<string>, TProduct> matchAction)
			: base(literals)
		{
			if(null == matchAction) throw ExceptionBecause.ArgumentNull(() => matchAction);

			_matchAction = matchAction;
		}

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);
			var matchingContext = context.Clone();

			char c;
			string matched = null;
			var suffixTrie = _trie;
			while(matchingContext.ConsumesAnyCharacter(out c) && suffixTrie.TryGetSubtrie(c, out suffixTrie)) {
				if(suffixTrie.HasValue) {
					context.Assimilate(matchingContext);
					matched = suffixTrie.Value;
				}
			}

			return null != matched
				? matchBuilder.Success(matched, _matchAction)
				: UnsuccessfulMatchingResult.Create(this);
		}
	}
}
