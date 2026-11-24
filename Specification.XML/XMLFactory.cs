using Parser;
using Parser.Tokens;

namespace Specification.XML;

public sealed class XMLFactory (TokenCollection parents)
{
  public IXMLObject Produce ()
  {
    List<IXMLObject> result = [];

    foreach (IToken token in parents)
    {
      IXMLObject? xml = token.Type switch
      {
        "ElementStart" => token is ComplexToken ct ? new XMLElement() { Tag = ct.Name!.Content } : null,
        _ => new XMLContent() { Content = token.Content }
      };

      if (xml is null)
      {
        _ = Err.ThrowBadDef("Constructed xml object was null.");
      }
      result.Add(xml);

    }

    return result[0];
  }
}
