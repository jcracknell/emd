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

		private readonly bool _saved;
		private readonly bool[] _saveCategories = new bool[MAX_UNICODE_CATEGORY + 1];
		private int _saveCharactersLength;
		private int _saveCharactersOffset;
		private bool[] _saveCharacters = null;
		private Trie<char, string> _saveGraphemes = null;

		private UnicodeCriteria(bool saved) {
			_saved = saved;
		}

		private UnicodeCriteria(UnicodeCriteria prototype) {
			_saved = prototype._saved;

			for(var i = 0; i < _saveCategories.Length; i++)
				_saveCategories[i] = prototype._saveCategories[i];

			if(null != prototype._saveCharacters) {
				_saveCharactersLength = prototype._saveCharactersLength;
				_saveCharacters = new bool[prototype._saveCharactersLength];
				_saveCharactersOffset = prototype._saveCharactersOffset;

				for(var i = 0; i < _saveCharactersLength; i++)
					_saveCharacters[i] = prototype._saveCharacters[i];
			}

			if(null != prototype._saveGraphemes)
				_saveGraphemes = prototype._saveGraphemes.Clone();
		}

		/// <summary>
		/// Accepts or rejects the unicode grapheme located at <paramref name="index"/> in <paramref name="str"/>.
		/// </summary>
		/// <param name="str">The string from which a grapheme should be accepted or rejected.</param>
		/// <param name="index">The location of the grapheme in <paramref name="str"/> which should be accepted or rejected.</param>
		/// <param name="length">Assigned the length of the grapheme which was accepted or rejected at <paramref name="index"/>.</param>
		/// <returns><code>true</code> if the unicode grapheme at <paramref name="index"/> in <paramref name="str"/> is accepted, <code>false</code> otherwise.</returns>
		public bool Accepts(string str, int index, out int length) {
			if(null == str) throw ExceptionBecause.ArgumentNull(() => str);

			UnicodeCategory category = UnicodeUtils.GetGraphemeInfo(str, index, out length);

			if(_saveCategories[(int)category])
				return _saved;

			if(1 == length) {
				if(null == _saveCharacters)
					return !_saved;

				var offsetCharValue = str[index] - _saveCharactersOffset;
				return offsetCharValue < _saveCharactersLength && offsetCharValue >= 0 && _saveCharacters[offsetCharValue]
					? _saved
					: !_saved;
			} else {
				if(null == _saveGraphemes)
					return !_saved;

				var subTrie = _saveGraphemes;
				var endIndex = index + length;
				while(index != endIndex)
					if(!subTrie.TryGetSubtrie(str[index++], out subTrie))
						return !_saved;

				return subTrie.HasValue ? _saved : !_saved;
			}
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="characters">Characters to be excluded from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Save(params char[] characters) {
			if(null == characters) throw ExceptionBecause.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).SaveInternal(characters);
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="characters">Characters to be excluded from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Save(params IEnumerable<char>[] characters) {
			if(null == characters) throw ExceptionBecause.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).SaveInternal(characters.Flatten());
		}

		private UnicodeCriteria SaveInternal(IEnumerable<char> characters) {
			if(!characters.Any()) throw ExceptionBecause.Argument(() => characters, "cannot be empty");

			int min = char.MaxValue;
			int max = char.MinValue;
			foreach(var c in characters) {
				if(c < min) min = c;
				if(c > max) max = c;
			}

			if(null == _saveCharacters) {
				_saveCharactersLength = max - min + 1;
				_saveCharacters = new bool[_saveCharactersLength];
				_saveCharactersOffset = min;
			} else {
				var oldMax = _saveCharactersOffset + _saveCharacters.Length - 1;
				if(min < _saveCharactersOffset || max > oldMax) {
					// _saveCharacters array needs resizing

					// Calculate the correct min and max values for a replacement saveCharacters array
					// (min will be the new _saveCharactersOffset)
					if(_saveCharactersOffset < min) min = _saveCharactersOffset;
					if(oldMax > max) max = oldMax;

					var saveCharactersReplacementLength = max - min + 1;
					var saveCharactersReplacement = new bool[saveCharactersReplacementLength];

					// Calculate the increase in offset required for existing saveCharacters values
					var translation = _saveCharactersOffset - min;

					for(var i = 0; i < _saveCharactersLength; i++)
						saveCharactersReplacement[i + translation] = _saveCharacters[i];

					_saveCharactersLength = saveCharactersReplacementLength;
					_saveCharacters = saveCharactersReplacement;
					_saveCharactersOffset = min;
				}
			}

			foreach(var c in characters)
				_saveCharacters[c - _saveCharactersOffset] = true;

			return this;
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.
		/// Provided <paramref name="characters"/> must be valid single unicode graphemes.
		/// </summary>
		/// <param name="characters">Characters to be excluded from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Save(params string[] characters) {
			if(null == characters) throw ExceptionBecause.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).SaveInternal(characters);
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.
		/// Provided <paramref name="characters"/> must be valid single unicode graphemes.
		/// </summary>
		/// <param name="characters">Characters to be excluded from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excluding the provided <paramref name="characters"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Save(params IEnumerable<string>[] characters) {
			if(null == characters) throw ExceptionBecause.ArgumentNull(() => characters);

			return new UnicodeCriteria(this).SaveInternal(characters.Flatten());
		}

		private UnicodeCriteria SaveInternal(IEnumerable<string> characters) {
			foreach(var character in characters) {
				if(null == character) throw ExceptionBecause.Argument(() => characters, "contains null values");

				var charLen = character.Length;
				if(1 == charLen) continue;
				if(0 == charLen) throw ExceptionBecause.Argument(() => characters, "contains empty values");

				if(!UnicodeUtils.IsSingleGrapheme(character))
					throw ExceptionBecause.Argument(() => characters, "contains value " + StringUtils.LiteralEncode(character) + ", which is not a unicode grapheme");

				if(null == _saveGraphemes)
					_saveGraphemes = new Trie<char,string>();

				_saveGraphemes.SetValue(character, character);
			}

			var singleCharacters = characters.Where(c => 1 == c.Length).Select(c => c[0]);
			if(singleCharacters.Any())
				SaveInternal(singleCharacters);

			return this;
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excluding the provided unicode <paramref name="categories"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="categories">Unicode categories to be excluded from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excluding the provided unicode <paramref name="categories"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Save(params UnicodeCategory[] categories) {
			if(null == categories) throw ExceptionBecause.ArgumentNull(() => categories);

			return Save(categories.AsEnumerable());
		}

		/// <summary>
		/// Creates a new <see cref="UnicodeCriteria"/> excluding the provided unicode <paramref name="categories"/> from the base acceptance criteria.
		/// </summary>
		/// <param name="categories">Unicode categories to be excluded from the base acceptance criteria.</param>
		/// <returns>A new <see cref="UnicodeCriteria"/> excluding the provided unicode <paramref name="categories"/> from the base acceptance criteria.</returns>
		public UnicodeCriteria Save(params IEnumerable<UnicodeCategory>[] categories) {
			if(null == categories) throw ExceptionBecause.ArgumentNull(() => categories);

			return new UnicodeCriteria(this).SaveInternal(categories.Flatten());
		}

		private UnicodeCriteria SaveInternal(IEnumerable<UnicodeCategory> categories) {
			foreach(var category in categories)
				_saveCategories[(int)category] = true;

			return this;
		}
	}
}
