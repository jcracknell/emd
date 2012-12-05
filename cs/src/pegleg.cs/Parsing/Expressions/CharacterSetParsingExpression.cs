using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class CharacterSetParsingExpression : BaseParsingExpression<Nil> {
		private readonly bool[] _acceptanceMap;
		private readonly int _offset;

		public CharacterSetParsingExpression(IEnumerable<char> characters) {
			CodeContract.ArgumentIsNotNull(() => characters, characters);
			CodeContract.ArgumentIsValid(() => characters, characters.Any(), "cannot be empty");

			var max = -1;
			var min = int.MaxValue;
			foreach(var c in characters) {
				if(c > max) max = c;
				if(c < min) min = c;
			}

			_offset = min;
			_acceptanceMap = new bool[max - min + 1];
			foreach(var c in characters)
				_acceptanceMap[c - _offset] = true;
		}

		public IEnumerable<char> Characters {
			get {
				for(var i = 0; i < _acceptanceMap.Length; i++)
					if(_acceptanceMap[i])
						yield return (char)(i + _offset);
			}
		}

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			if(context.ConsumesCharacter(_acceptanceMap, _offset))
				return SuccessfulMatchingResult.Create(Nil.Value, 1);
			else
				return UnsuccessfulMatchingResult.Create(this);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
