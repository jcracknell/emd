using markdom.cs.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace markdom.cs.Conversion.Html {
	public static class HtmlConverter {
		public static void Convert(MarkdomDocumentNode document, Stream ostream) {
			if(null == document) throw ExceptionBecause.ArgumentNull(() => document);

			var references = new ReferenceCollection(document);

			using(var xmlWriter = XmlWriter.Create(ostream, new XmlWriterSettings {
				CloseOutput = false,
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\n",
				OmitXmlDeclaration = true
			})) {
				xmlWriter.WriteDocType("html", null, null, null);
				xmlWriter.WriteStartElement("html");
				xmlWriter.WriteStartElement("body");

				document.HandleWith(new XmlWritingNodeHandler(xmlWriter, references));

				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
		}
	}
}
