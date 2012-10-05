using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	/// <summary>
	/// Interface describing a successful match for an expression.
	/// </summary>
	/// <remarks>
	/// This interface uses methods instead of properties in order to improve syntactical
	/// compatibility with languages which do not support properties.
	/// </remarks>
	public interface IExpressionMatch<out TProduct> {
		SourceRange SourceRange { get; }
		int Index { get; }
		int Length { get; }

		TProduct Product { get; }

		/// <summary>
		/// The <see cref="IExpression"/> which was matched.
		/// </summary>
		IExpression Expression { get; }

		/// <summary>
		/// The section of raw input matched by the expression.
		/// </summary>
		string String { get; }
	}
}
