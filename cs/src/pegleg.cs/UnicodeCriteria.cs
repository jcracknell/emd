using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs {
	/// <summary>
	/// Class which can be configured to accept or reject unicode graphemes by <see cref="UnicodeCategory"/> or value.
	/// </summary>
	public class UnicodeCriteria {
		protected static readonly int MAX_UNICODE_CATEGORY = Enum.GetValues(typeof(UnicodeCategory)).Cast<int>().Max();
		protected static readonly UnicodeCategory[] UNICODE_CATEGORIES = Enum.GetValues(typeof(UnicodeCategory)).Cast<UnicodeCategory>().ToArray();

		/// <summary>
		/// Get a base <see cref="UnicodeCriteria"/> instance accepting any unicode grapheme.
		/// </summary>
		public static UnicodeCriteria AnyCharacter {
			get { return new UnicodeCriteria(false); }
		}

		/// <summary>
		/// Get a base <see cref="UnicodeCriteria"/> instance accepting no unicode graphemes.
		/// </summary>
		public static UnicodeCriteria NoCharacter { 
			get { return new UnicodeCriteria(true); }
		}

		private readonly bool _excepted;
		private readonly bool[] _exceptedCategories = new bool[MAX_UNICODE_CATEGORY + 1];
		private int _exceptedCharactersLength;
		private int _exceptedCharactersOffset;
		private bool[] _exceptedCharacters = null;
		private Trie<char, string> _exceptedGraphemes = null;

		private UnicodeCriteria(bool excepted) {
			_excepted = excepted;
		}

		private UnicodeCriteria(UnicodeCriteria prototype) {
			_excepted = prototype._excepted;

			for(var i = 0; i < _exceptedCategories.Length; i++)
				_exceptedCategories[i] = prototype._exceptedCategories[i];

			if(null != prototype._exceptedCharacters) {
				_exceptedCharactersLength = prototype._exceptedCharactersLength;
				_exceptedCharacters = new bool[prototype._exceptedCharactersLength];
				_exceptedCharactersOffset = prototype._exceptedCharactersOffset;

				for(var i = 0; i < _exceptedCharactersLength; i++)
					_exceptedCharacters[i] = prototype._exceptedCharacters[i];
			}

			if(null != prototype._exceptedGraphemes)
				_exceptedGraphemes = prototype._exceptedGraphemes.Clone();
		}

		/// <summary>
		/// Accepts or rejects the unicode grapheme located at <paramref name="index"/> in <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The string from which a grapheme should be accepted or rejected.</param>
		/// <param name="index">The location of the grapheme in <paramref name="str"/> which should be accepted or rejected.</param>
		/// <param name="length">Assigned the length of the grapheme which was accepted or rejected at <paramref name="index"/>.</param>
		/// <returns><code>true</code> if the unicode grapheme at <paramref name="index"/> in <paramref name="str"/> is accepted, <code>false</code> otherwise.</returns>
		public bool Accepts(string str, int index, out int length) {
			if(null == str) throw Xception.Because.ArgumentNull(() => str);

			UnicodeCategory category = UnicodeUtils.GetGraphemeInfo(str, index, out length);

			if(_exceptedCategories[(int)category])
				return _excepted;

			if(1 == length) {
				if(null == _exceptedCharacters)
					return !_excepted;

				var offsetCharValue = str[index] - _exceptedCharactersOffset;
				return offsetCharValue < _exceptedCharactersLength && offsetCharValue >= 0 && _exceptedCharacters[offsetCharValue]
					? _excepted
					: !_excepted;
			} else {
				if(null == _exceptedGraphemes)
					return !_excepted;

				var subTrie = _exceptedGraphemes;
				var endIndex = index + length;
				while(index != endIndex)
					if(!subTrie.TryGetSubtrie(str[index++], out subTrie))
						return !_excepted;

				return subTrie.HasValue ? _excepted : !_excepted;
			}
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="characters">Characters to be excepted from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Except(params char[] characters) {
			if(null == characters) throw Xception.Because.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).ExceptInternal(characters);
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="characters">Characters to be excepted from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Except(params IEnumerable<char>[] characters) {
			if(null == characters) throw Xception.Because.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).ExceptInternal(characters.Flatten());
		}

		private UnicodeCriteria ExceptInternal(IEnumerable<char> characters) {
			if(!characters.Any()) throw Xception.Because.Argument(() => characters, "cannot be empty");

			int min = char.MaxValue;
			int max = char.MinValue;
			foreach(var c in characters) {
				if(c < min) min = c;
				if(c > max) max = c;
			}

			if(null == _exceptedCharacters) {
				_exceptedCharactersLength = max - min + 1;
				_exceptedCharacters = new bool[_exceptedCharactersLength];
				_exceptedCharactersOffset = min;
			} else {
				var oldMax = _exceptedCharactersOffset + _exceptedCharacters.Length - 1;
				if(min < _exceptedCharactersOffset || max > oldMax) {
					// _exceptedCharacters array needs resizing

					// Calculate the correct min and max values for a replacement saveCharacters array
					// (min will be the new _saveCharactersOffset)
					if(_exceptedCharactersOffset < min) min = _exceptedCharactersOffset;
					if(oldMax > max) max = oldMax;

					var exceptedCharactersReplacementLength = max - min + 1;
					var exceptedCharactersReplacement = new bool[exceptedCharactersReplacementLength];

					// Calculate the increase in offset required for existing saveCharacters values
					var translation = _exceptedCharactersOffset - min;

					for(var i = 0; i < _exceptedCharactersLength; i++)
						exceptedCharactersReplacement[i + translation] = _exceptedCharacters[i];

					_exceptedCharactersLength = exceptedCharactersReplacementLength;
					_exceptedCharacters = exceptedCharactersReplacement;
					_exceptedCharactersOffset = min;
				}
			}

			foreach(var c in characters)
				_exceptedCharacters[c - _exceptedCharactersOffset] = true;

			return this;
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.
		/// Provided <paramref name="characters"/> must be valid single unicode graphemes.
		/// </summary>
		/// <param name="characters">Characters to be excepted from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Except(params string[] characters) {
			if(null == characters) throw Xception.Because.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).ExceptInternal(characters);
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.
		/// Provided <paramref name="characters"/> must be valid single unicode graphemes.
		/// </summary>
		/// <param name="characters">Characters to be excepted from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excepting the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Except(params IEnumerable<string>[] characters) {
			if(null == characters) throw Xception.Because.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).ExceptInternal(characters.Flatten());
		}

		private UnicodeCriteria ExceptInternal(IEnumerable<string> characters) {
			foreach(var character in characters) {
				if(null == character) throw Xception.Because.Argument(() => characters, "contains null values");

				var charLen = character.Length;
				if(1 == charLen) continue;
				if(0 == charLen) throw Xception.Because.Argument(() => characters, "contains empty values");

				if(!UnicodeUtils.IsSingleGrapheme(character))
					throw Xception.Because.Argument(() => characters, "contains value " + StringUtils.LiteralEncode(character) + ", which is not a unicode grapheme");

				if(null == _exceptedGraphemes)
					_exceptedGraphemes = new Trie<char,string>();

				_exceptedGraphemes.SetValue(character, character);
			}

			var singleCharacters = characters.Where(c => 1 == c.Length).Select(c => c[0]);
			if(singleCharacters.Any())
				return ExceptInternal(singleCharacters);
			else
				return this;
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excepting the provided unicode <paramref name="categories"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="categories">Unicode categories to be excepted from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excepting the provided unicode <paramref name="categories"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Except(params UnicodeCategory[] categories) {
			if(null == categories) throw Xception.Because.ArgumentNull(() => categories);

			return new UnicodeCriteria(this).ExceptInternal(categories);
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excepting the provided unicode <paramref name="categories"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="categories">Unicode categories to be excepted from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excepting the provided unicode <paramref name="categories"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Except(params IEnumerable<UnicodeCategory>[] categories) {
			if(null == categories) throw Xception.Because.ArgumentNull(() => categories);

			return new UnicodeCriteria(this).ExceptInternal(categories.Flatten());
		}

		private UnicodeCriteria ExceptInternal(IEnumerable<UnicodeCategory> categories) {
			foreach(var category in categories)
				_exceptedCategories[(int)category] = true;

			return this;
		}
	}
}
