using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
  public abstract class OrderedChoiceParsingExpression<TChoice, TProduct> : CacheingParsingExpression<TProduct> {
    protected readonly IParsingExpression<TChoice>[] _choices;

    public OrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices) {
      if(null == choices) throw Xception.Because.ArgumentNull(() => choices);
      if(!(choices.Length >= 2)) throw Xception.Because.Argument(() => choices, "must have length of at least two");

      _choices = choices;
    }

    public IEnumerable<IParsingExpression<TChoice>> Choices { get { return _choices; }  }

    public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
      return handler.Handle(this);
    }
  }

  public class NonCapturingOrderedChoiceParsingExpression : OrderedChoiceParsingExpression<object, Nil> {
    public NonCapturingOrderedChoiceParsingExpression(IParsingExpression<object>[] choices) : base(choices) { }

    protected override IMatchingResult<Nil> MatchesUncached(MatchingContext context) {
      for(int i = 0; i < _choices.Length; i++) {
        var choice = _choices[i];
        var choiceMatchingContext = context.Clone();
        var choiceMatchingResult = choice.Matches(choiceMatchingContext);

        if(choiceMatchingResult.Succeeded) {
          context.Assimilate(choiceMatchingContext);
          return SuccessfulMatchingResult.Create(Nil.Value, choiceMatchingResult.Length);
        }
      }

      return UnsuccessfulMatchingResult.Create(this);
    }
  }

  public class NonCapturingOrderedChoiceParsingExpression<TChoice> : OrderedChoiceParsingExpression<TChoice, TChoice> {
    public NonCapturingOrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices) : base(choices) { }

    protected override IMatchingResult<TChoice> MatchesUncached(MatchingContext context) {
      for(int i = 0; i < _choices.Length; i++) {
        var choice = _choices[i];
        var choiceMatchingContext = context.Clone();
        var choiceMatchingResult = choice.Matches(choiceMatchingContext);

        if(choiceMatchingResult.Succeeded) {
          context.Assimilate(choiceMatchingContext);
          return choiceMatchingResult;
        }
      }

      return UnsuccessfulMatchingResult.Create(this);
    }
  }

  public class CapturingOrderedChoiceParsingExpression<TChoice, TProduct> : OrderedChoiceParsingExpression<TChoice, TProduct> {
    private Func<IMatch<TChoice>, TProduct> _matchAction;

    public CapturingOrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices, Func<IMatch<TChoice>, TProduct> matchAction)
      : base(choices)
    {
      if(null == matchAction) throw Xception.Because.ArgumentNull(() => matchAction);

      _matchAction = matchAction;
    }

    protected override IMatchingResult<TProduct> MatchesUncached(MatchingContext context) {
      var matchBuilder = context.GetMatchBuilderFor(this);

      for(int i = 0; i < _choices.Length; i++) {
        var choice = _choices[i];
        var choiceMatchingContext = context.Clone();
        var choiceMatchingResult = choice.Matches(choiceMatchingContext);

        if(choiceMatchingResult.Succeeded) {
          context.Assimilate(choiceMatchingContext);
          return matchBuilder.Success(choiceMatchingResult.Product, _matchAction);
        }
      }

      return UnsuccessfulMatchingResult.Create(this);
    }
  }
}
