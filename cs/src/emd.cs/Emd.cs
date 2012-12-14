using emd.cs.Grammar;
using emd.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs {
	public static class Emd {
		public static DocumentNode ParseDocument(string input) {
			if(null == input) throw ExceptionBecause.ArgumentNull(() => input);

			return EmdGrammar.Document.Matches(new pegleg.cs.Parsing.MatchingContext(input)).Product;
		}
	}
}
