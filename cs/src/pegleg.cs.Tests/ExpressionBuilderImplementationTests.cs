using FluentAssertions;
using pegleg.cs.Parsing.Expressions.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace pegleg.cs {
	public class ExpressionBuilderImplementationTests {
		[Fact] public void ExpressionBuilder_should_not_be_sealed() {
			typeof(ExpressionBuilder).IsSealed
			.Should().BeFalse("because it is useful to be able to extend it to use its methods without having to qualify them");
		}

		[Fact] public void ExpressionBuilder_should_define_only_static_methods() {
			typeof(ExpressionBuilder)
			.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod)
			.Where(method => !method.IsStatic)
			.Should().BeEmpty("because {0} should define static methods only", typeof(ExpressionBuilder).Name);
		}

		[Fact] public void ExpressionBuilder_should_define_only_static_properties() {
			typeof(ExpressionBuilder)
			.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty)
			.Where(property =>
				(null != property.GetGetMethod() && !property.GetGetMethod().IsStatic)
				|| (null != property.GetSetMethod() && !property.GetSetMethod().IsStatic)
			)
			.Should().BeEmpty("because {0} should define static properties only", typeof(ExpressionBuilder).Name);
		}
	}
}
