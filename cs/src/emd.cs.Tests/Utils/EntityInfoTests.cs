using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace emd.cs.Utils {
  public class EntityInfoTests {
    [Fact] public void EntityInfo_IsEntityName_Aacute_should_be_true() {
      EntityInfo.IsEntityName("Aacute").Should().BeTrue();
    }

    [Fact] public void EntityInfo_TryGetEntityName_should_succeed_for_193() {
      string entityName;
      EntityInfo.TryGetEntityName(193, out entityName).Should().BeTrue();
      entityName.Should().Be("Aacute");
    }

    [Fact] public void EntityInfo_TryGetEntityValue_should_work_for_Aacute() {
      string value;
      EntityInfo.TryGetEntityValue("Aacute", out value).Should().BeTrue();
      value.Should().Be("\u00c1");
    }

    [Fact] public void EntityInfo_TryGetEntityValue_should_work_for_aacute() {
      string value;
      EntityInfo.TryGetEntityValue("aacute", out value).Should().BeTrue();
      value.Should().Be("\u00e1");
    }
  }
}
