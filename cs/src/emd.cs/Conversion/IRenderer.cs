using emd.cs.Nodes;
using System.IO;

namespace emd.cs.Conversion {
  public interface IRenderer {
    void Render(INode node, Stream ostream);
  }
}
