using emd.cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remd {
	class Program {
		static void Main(string[] args) {
			string input;
			using(var istream = Console.OpenStandardInput())
			using(var reader = new StreamReader(istream, Console.InputEncoding))
				input = reader.ReadToEnd();

			var document = Emd.ParseDocument(input);

			var renderer = new emd.cs.Conversion.StructuralRenderer();
			using(var ostream = Console.OpenStandardOutput())
				renderer.Render(document, ostream);
		}
	}
}
