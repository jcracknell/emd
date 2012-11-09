using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace pegleg.cs.Parsing.Expressions {
	public class SequenceProductsTests {
		public class C1 { }
		public class C2 { }
		public class C3 { }
		public class C4 { }
		public class C5 { }
		public class C6 { }
		public class C7 { }
		public class C8 { }
		public class C9 { }
		public class C10 { }
		public class C11 { }
		public class C12 { }
		public class C13 { }
		public class C14 { }
		public class C15 { }
		public class C16 { }

		[Fact] void SequenceProducts_exercise_2_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2()
			});

			var annotated = sequenceProducts.Annotate<C1,C2>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
		}

		[Fact] void SequenceProducts_exercise_3_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
		}

		[Fact] void SequenceProducts_exercise_4_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
		}

		[Fact] void SequenceProducts_exercise_5_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
		}

		[Fact] void SequenceProducts_exercise_6_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
		}

		[Fact] void SequenceProducts_exercise_7_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
		}

		[Fact] void SequenceProducts_exercise_8_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
		}

		[Fact] void SequenceProducts_exercise_9_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
		}

		[Fact] void SequenceProducts_exercise_10_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
		}

		[Fact] void SequenceProducts_exercise_11_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10(), new C11()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
			annotated.Of11.Should().BeAssignableTo<C11>();
		}

		[Fact] void SequenceProducts_exercise_12_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10(), new C11(), new C12()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
			annotated.Of11.Should().BeAssignableTo<C11>();
			annotated.Of12.Should().BeAssignableTo<C12>();
		}

		[Fact] void SequenceProducts_exercise_13_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10(), new C11(), new C12(), new C13()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12,C13>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
			annotated.Of11.Should().BeAssignableTo<C11>();
			annotated.Of12.Should().BeAssignableTo<C12>();
			annotated.Of13.Should().BeAssignableTo<C13>();
		}

		[Fact] void SequenceProducts_exercise_14_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10(), new C11(), new C12(), new C13(), new C14()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12,C13,C14>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
			annotated.Of11.Should().BeAssignableTo<C11>();
			annotated.Of12.Should().BeAssignableTo<C12>();
			annotated.Of13.Should().BeAssignableTo<C13>();
			annotated.Of14.Should().BeAssignableTo<C14>();
		}

		[Fact] void SequenceProducts_exercise_15_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10(), new C11(), new C12(), new C13(), new C14(), new C15()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12,C13,C14,C15>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
			annotated.Of11.Should().BeAssignableTo<C11>();
			annotated.Of12.Should().BeAssignableTo<C12>();
			annotated.Of13.Should().BeAssignableTo<C13>();
			annotated.Of14.Should().BeAssignableTo<C14>();
			annotated.Of15.Should().BeAssignableTo<C15>();
		}

		[Fact] void SequenceProducts_exercise_16_annotations() {
			var sequenceProducts = new SequenceProducts(new object[] {
				new C1(), new C2(), new C3(), new C4(), new C5(), new C6(), new C7(), new C8(),
				new C9(), new C10(), new C11(), new C12(), new C13(), new C14(), new C15(), new C16()
			});

			var annotated = sequenceProducts.Annotate<C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12,C13,C14,C15,C16>();

			annotated.Of1.Should().BeAssignableTo<C1>();
			annotated.Of2.Should().BeAssignableTo<C2>();
			annotated.Of3.Should().BeAssignableTo<C3>();
			annotated.Of4.Should().BeAssignableTo<C4>();
			annotated.Of5.Should().BeAssignableTo<C5>();
			annotated.Of6.Should().BeAssignableTo<C6>();
			annotated.Of7.Should().BeAssignableTo<C7>();
			annotated.Of8.Should().BeAssignableTo<C8>();
			annotated.Of9.Should().BeAssignableTo<C9>();
			annotated.Of10.Should().BeAssignableTo<C10>();
			annotated.Of11.Should().BeAssignableTo<C11>();
			annotated.Of12.Should().BeAssignableTo<C12>();
			annotated.Of13.Should().BeAssignableTo<C13>();
			annotated.Of14.Should().BeAssignableTo<C14>();
			annotated.Of15.Should().BeAssignableTo<C15>();
			annotated.Of16.Should().BeAssignableTo<C16>();
		}
	}
}
