using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pegleg.cs.Parsing {
	[TestClass]
	public class CharacterLocationTests {
		[TestMethod]
		public void CharacterLocation_constructor_works_as_expected() {
			var characterLocation = new SourceLocation(3, 2, 1);

			Assert.AreEqual(3, characterLocation.Index);
			Assert.AreEqual(2, characterLocation.Line);
			Assert.AreEqual(1, characterLocation.LineIndex);
		}
	}
}
