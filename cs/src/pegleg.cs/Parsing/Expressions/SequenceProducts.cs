using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class SequenceProducts : IEnumerable<object> {
		protected readonly object[] _products;

		public SequenceProducts(object[] products) {
			CodeContract.ArgumentIsNotNull(() => products, products);

			_products = products;	
		}

		public object this[int index] {
			get { return _products[index]; }
		}

		public IEnumerator<object> GetEnumerator() {
			return _products.AsEnumerable().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _products.GetEnumerator();	
		}

		public SequenceProducts<T1,T2> Upcast<T1,T2>() { return new SequenceProducts<T1,T2>(_products); }
		public SequenceProducts<T1,T2,T3> Upcast<T1,T2,T3>() { return new SequenceProducts<T1,T2,T3>(_products); }
		public SequenceProducts<T1,T2,T3,T4> Upcast<T1,T2,T3,T4>() { return new SequenceProducts<T1,T2,T3,T4>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5> Upcast<T1,T2,T3,T4,T5>() { return new SequenceProducts<T1,T2,T3,T4,T5>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6> Upcast<T1,T2,T3,T4,T5,T6>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7> Upcast<T1,T2,T3,T4,T5,T6,T7>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8> Upcast<T1,T2,T3,T4,T5,T6,T7,T8>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(_products); }
		public SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> Upcast<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>() { return new SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(_products); }
	}

	// There is a lot of duplication here that could be eliminated through inheritance,
	// however it seems prudent to do it this way for performance
	public class SequenceProducts<T1,T2> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
	}

	public class SequenceProducts<T1,T2,T3> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
		public T11 Of11 { get { return (T11)_products[10]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
		public T11 Of11 { get { return (T11)_products[10]; } }
		public T12 Of12 { get { return (T12)_products[11]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
		public T11 Of11 { get { return (T11)_products[10]; } }
		public T12 Of12 { get { return (T12)_products[11]; } }
		public T13 Of13 { get { return (T13)_products[12]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
		public T11 Of11 { get { return (T11)_products[10]; } }
		public T12 Of12 { get { return (T12)_products[11]; } }
		public T13 Of13 { get { return (T13)_products[12]; } }
		public T14 Of14 { get { return (T14)_products[13]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
		public T11 Of11 { get { return (T11)_products[10]; } }
		public T12 Of12 { get { return (T12)_products[11]; } }
		public T13 Of13 { get { return (T13)_products[12]; } }
		public T14 Of14 { get { return (T14)_products[13]; } }
		public T15 Of15 { get { return (T15)_products[14]; } }
	}

	public class SequenceProducts<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> : SequenceProducts {
		public SequenceProducts(object[] products) : base(products) { }
		public T1 Of1 { get { return (T1)_products[0]; } }
		public T2 Of2 { get { return (T2)_products[1]; } }
		public T3 Of3 { get { return (T3)_products[2]; } }
		public T4 Of4 { get { return (T4)_products[3]; } }
		public T5 Of5 { get { return (T5)_products[4]; } }
		public T6 Of6 { get { return (T6)_products[5]; } }
		public T7 Of7 { get { return (T7)_products[6]; } }
		public T8 Of8 { get { return (T8)_products[7]; } }
		public T9 Of9 { get { return (T9)_products[8]; } }
		public T10 Of10 { get { return (T10)_products[9]; } }
		public T11 Of11 { get { return (T11)_products[10]; } }
		public T12 Of12 { get { return (T12)_products[11]; } }
		public T13 Of13 { get { return (T13)_products[12]; } }
		public T14 Of14 { get { return (T14)_products[13]; } }
		public T15 Of15 { get { return (T15)_products[14]; } }
		public T16 Of16 { get { return (T16)_products[15]; } }
	}
}
