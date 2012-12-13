using emd.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace emd.cs.Nodes{
	public class ReferenceId {
		private static readonly Regex NOTHINGS = new Regex(@"[""<>'@#$%\^&*()=/_+{}\[\;\]]+", RegexOptions.Compiled);
		private static readonly Regex TODASH = new Regex(@"[^a-z0-9]+", RegexOptions.Compiled);

		private readonly string _value;

		public string Value { get { return _value; } }
		
		private ReferenceId(string value) {
			_value = value;
		}

		public static ReferenceId FromText(string text) {
			if(null == text) return null;

			text = TextUtils.RemoveDiacritics(text).ToLower();
			text = NOTHINGS.Replace(text, "");
			text = TODASH.Replace(text, "-");
			text = string.Join("-", text.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries));

			if(string.Empty.Equals(text))
				return null;

			return new ReferenceId(text);
		}

		public override int GetHashCode() {
			return _value.GetHashCode();
		}

		public override bool Equals(object obj) {
			var other = obj as ReferenceId;
			return null != other && this.Value.Equals(other.Value, StringComparison.Ordinal);
		}

		public override string ToString() {
			return typeof(ReferenceId).Name + "(" + _value + ")";
		}
	}
}
