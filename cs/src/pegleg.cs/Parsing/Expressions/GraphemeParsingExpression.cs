using pegleg.cs.Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
  public class GraphemeParsingExpression : BaseParsingExpression<Nil> {
    private readonly IGraphemeCriteria _criteria;

    public GraphemeParsingExpression(IGraphemeCriteria criteria) {
      if(null == criteria) throw Xception.Because.ArgumentNull(() => criteria);

      _criteria = criteria;
    }

    public override IMatchingResult<Nil> Matches(MatchingContext context) {
      int length;
      if(context.ConsumesGraphemeCriteria(_criteria, out length))
        return SuccessfulMatchingResult.Create(Nil.Value, length);

      return UnsuccessfulMatchingResult.Create(this);
    }

    public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }
}
