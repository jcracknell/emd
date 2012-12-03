using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace markdom.cs.Utils {
	public class EntityInfoTests {
		[Fact] public void EntityInfo_IsEntityName_Aacute_should_be_true() {
			EntityInfo.IsEntityName("Aacute").Should().BeTrue();
		}
	}
}
