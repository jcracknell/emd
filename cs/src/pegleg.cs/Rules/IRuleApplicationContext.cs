using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pegleg.cs
{
    public interface IRuleApplicationContext {
			void Consume(int n);
			bool Matches(string literal);
			bool Matches(Regex re);
			bool Matches(Regex re, out Match match);

			/// <summary>
			/// Create a clone of the input context which can be manipulated without altering
			/// its originating context.
			/// </summary>
			/// <returns>A clone of the input context.</returns>
			IRuleApplicationContext Clone();
    }
}