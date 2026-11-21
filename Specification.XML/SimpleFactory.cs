using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Parser;
using Parser.Tokens;

namespace Specification.XML;

public abstract class SimpleFactory<TOut> () : IObjectFactory<TOut> where TOut : notnull
{
  public required XParser Parser { get; init; }
  public Spec Spec => Parser.Spec ?? Parser.LocalDefaultSpec;

  [SetsRequiredMembers]
  protected SimpleFactory (XParser parser) : this() => Parser = parser;

  public abstract TOut Produce (IToken input);
  public virtual IEnumerable<TOut> ProduceAll (TokenCollection tokens) => tokens.Select(Produce);
}
