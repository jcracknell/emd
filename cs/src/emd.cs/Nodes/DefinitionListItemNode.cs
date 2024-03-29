﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
  public class DefinitionListItemNode : INodelike {
    private readonly DefinitionListTermNode _term;
    private readonly DefinitionListDefinitionNode[] _definitions;
    private readonly SourceRange _sourceRange;

    public DefinitionListItemNode(DefinitionListTermNode term, DefinitionListDefinitionNode[] definitions, SourceRange sourceRange) {
      if(null == term) throw Xception.Because.ArgumentNull(() => term);
      if(null == definitions) throw Xception.Because.ArgumentNull(() => definitions);
      if(null == sourceRange) throw Xception.Because.ArgumentNull(() => sourceRange);
      if(!definitions.Any()) throw Xception.Because.Argument(() => definitions, "cannot be empty");

      _term = term;
      _definitions = definitions;
      _sourceRange = sourceRange;
    }

    public DefinitionListTermNode Term { get { return _term; } }

    public IEnumerable<DefinitionListDefinitionNode> Definitions { get { return _definitions; } }

    public SourceRange SourceRange { get { return _sourceRange; } }
  }
}
