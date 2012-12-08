using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Utils {
	public class Trie<TKey, TValue> {
		private bool _hasValue = false;
		private TValue _value = default(TValue);

		private int _numChildren = 0;
		private TKey _childKey = default(TKey);
		private Trie<TKey, TValue> _childTrie = null;
		private IDictionary<TKey, Trie<TKey, TValue>> _children = null;

		public Trie() { }

		private Trie(Trie<TKey, TValue> prototype) {
			_hasValue = prototype._hasValue;
			_value = prototype._value;
			_numChildren = prototype._numChildren;
			if(1 == _numChildren) {
				_childKey = prototype._childKey;
				_childTrie = prototype._childTrie.Clone();
			} else if(0 != _numChildren) {
				_children = new Dictionary<TKey, Trie<TKey, TValue>>(prototype._children.Count);
				foreach(var prototypeChild in prototype._children)
					_children[prototypeChild.Key] = prototypeChild.Value.Clone();
			}
		}

		public bool HasValue { get { return _hasValue; } }
		
		public TValue Value { get { return _value; } }

		private IEnumerable<Trie<TKey, TValue>> Subtries {
			get {
				if(0 == _numChildren)
					return Enumerable.Empty<Trie<TKey, TValue>>();
				if(1 == _numChildren)
					return _childTrie.InEnumerable();
				return _children.Values;
			}
		}

		public IEnumerable<TValue> Values {
			get {
				if(_hasValue) {
					return _value.InEnumerable().Concat(Subtries.SelectMany(subtrie => subtrie.Values));
				} else {
					return Subtries.SelectMany(subtrie => subtrie.Values);
				}
			}
		}

		public void SetValue(IEnumerable<TKey> path, TValue value) {
			SetValue(path.GetEnumerator(), value);
		}

		private void SetValue(IEnumerator<TKey> path, TValue value) {
			if(!path.MoveNext()) {
				_hasValue = true;
				_value = value;
				return;
			}

			Trie<TKey, TValue> child;
			var key = path.Current;

			if(0 == _numChildren) {
				// Store single child
				_childKey = key;
				child = _childTrie = new Trie<TKey,TValue>();
				_numChildren++;
			} else if(1 == _numChildren && _childKey.Equals(key)) {
				// Overwrite existing single child
				child = _childTrie;
			} else {
				if(null == _children) {
					// Switch to storing multiple children in a dictionary
					_children = new Dictionary<TKey, Trie<TKey, TValue>>();
					_children.Add(_childKey, _childTrie);
				}
				// Try and retrieve existing child from dictionary
				if(!_children.TryGetValue(key, out child)) {
					// Add missing child to dictionary
					child = _children[key] = new Trie<TKey, TValue>();
					_numChildren++;
				}
			}

			child.SetValue(path, value);
		}

		public bool Contains(IEnumerable<TKey> path) {
			TValue value;
			return TryGetValue(path, out value);
		}

		public bool TryGetValue(IEnumerable<TKey> path, out TValue value) {
			return TryGetValue(path.GetEnumerator(), out value);
		}

		private bool TryGetValue(IEnumerator<TKey> path, out TValue value) {
			if(!path.MoveNext()) {
				if(_hasValue) {
					value = _value;
					return true;
				} else {
					value = default(TValue);
					return false;
				}
			}

			Trie<TKey, TValue> child;
			if(!TryGetSubtrie(path.Current, out child)) {
				value = default(TValue);
				return false;
			}
			
			return child.TryGetValue(path, out value);
		}

		public bool TryGetSubtrie(TKey key, out Trie<TKey, TValue> subtrie) {
			if(0 == _numChildren) {
				subtrie = null;
				return false;
			}
			
			if(1 == _numChildren) {
				if(_childKey.Equals(key)) {
					subtrie = _childTrie;
					return true;
				} else {
					subtrie = null;
					return false;
				}
			}

			return _children.TryGetValue(key, out subtrie);
		}

		public Trie<TKey, TValue> Clone() {
			return new Trie<TKey,TValue>(this);
		}
	}
}
