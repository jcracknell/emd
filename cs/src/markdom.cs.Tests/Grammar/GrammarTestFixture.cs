using markdom.cs.Model;
using markdom.cs.Model.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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


		protected void AssertNodesAreEqual(INode expected, object actual) {
			((INode)actual).HandleWith(new NodeEqualityTestingHandler(expected));
		}
	}
}
