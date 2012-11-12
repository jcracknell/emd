using markdom.cs.Nodes;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace markdom.cs.Grammar {
	public abstract class GrammarTestFixture {
		protected readonly MarkdomGrammar Grammar = new MarkdomGrammar();
	}
}
