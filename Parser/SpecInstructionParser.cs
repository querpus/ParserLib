#pragma warning disable CA1710 // Rename Parser.Library to end in either 'Dictionary' or 'Collection'

using System.Xml.Linq;

using Parser.Inference;

namespace Parser;

public static class SpecInstructionParser
{
  private static readonly XNamespace Ns = "Parser/Spec";
  public static IEnumerable<Spec> LoadSpecFile (string path)
  {
    if (Path.GetExtension(path).Like("xml"))
    {
      return LoadSpecXML(path);
    }

    if (Path.GetExtension(path).Like("spec"))
    {
      //return LoadSpecExt(path);
    }
    Log(MsgClass.Warning, $"Loading spec file '{path}' failed.", "SpecInstructionParser");
    return [];
  }
  public static IEnumerable<Spec> LoadSpecXML (string path)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(path);

    XDocument doc = XDocument.Load(path);

    IEnumerable<XElement> specElements =
        doc.Root?
            .Elements(Ns + "Spec")
        ?? throw new InvalidDataException(
            "Definition does not contain a Spec element.");

    foreach (XElement specElement in specElements)
    {
      yield return new Spec
      {
        Name = GetRequiredString(specElement, "Name"),
        IsTextFile = GetBool(specElement, "TextFile"),
        RxOpt = ParseRegex(specElement.Element(Ns + "RegexOptions")?.Value ?? ""),
        DefaultRuleSet = ParseRuleTypes(specElement.Element(Ns + "DefaultRuleSet")?.Value ?? ""),
        TokenRules = ParseRules(specElement.Element(Ns + "TokenRules")),
        GroupTokenRules = ParseRules(specElement.Element(Ns + "GroupTokenRules")),
        //TokenCompatLookup = ParseTokenLookups(specElement.Element(Ns + "TokenLookups")),
        FileInferences = [InferenceNodeFactory.Produce(specElement.Element(Ns + "FileInferences"))],
        Operations = [.. specElement.Element(Ns + "Instructions")?.Elements()?.Select(OperationFactory.Produce).Where(c => c is not null).Cast<IOperation>() ?? []]
      };
    }
  }
  private static string GetRequiredString (XElement parent, string name) =>
    parent.Element(Ns + name)?.Value.Trim()
        ?? throw new InvalidDataException($"Missing required element '{name}'.");

  private static bool GetBool (XElement parent, string name, bool defaultIfAbsent = false, bool defaultIfEmpty = true)
  {
    XElement? node = parent.Element(Ns + name);

    if (node is null) // Absent
      return defaultIfAbsent;

    if (node.Value.IsEmpty) // Empty
      return defaultIfEmpty;

    return bool.Parse(node.Value);
  }

  private static TokenRuleCollection ParseRules (XElement? rulesElement)
  {
    TokenRuleCollection result = [];

    if (rulesElement == null)
      return result;

    foreach (XElement rule in rulesElement.Elements(Ns + "Rule"))
    {
      string type = rule.Element(Ns + "Type")?.Value ?? "None";
      string assign = rule.Element(Ns + "TypeToAssign")?.Value ?? "None";
      string data = rule.Element(Ns + "Data")?.Value.Trim() ?? "";

      try
      {
        result.Add(new TokenRule(ParseRuleTypes(type), assign, data));
      }
      catch (OperationBadDefinitionException ex)
      {
        Log(MsgClass.Warning, $"Failed to parse rule {assign}: {ex.Message}", "SpecInstructionParser");
      }
    }

    return result;
  }

  private static RegexOptions ParseRegexOption (string opt) => opt switch
  {
    "IgnoreCase" => RegexOptions.IgnoreCase,
    "Multiline" => RegexOptions.Multiline,
    "Singleline" => RegexOptions.Singleline,
    "IgnorePatternWhitespace" => RegexOptions.IgnorePatternWhitespace,
    "ExplicitCapture" => RegexOptions.ExplicitCapture,
    _ => throw Err.ThrowBadDef($"Invalid regex option {opt}")
  };

  private static RegexOptions ParseRegex (string value) => value.
    Split(' ', SSORT).
    Aggregate(
      RegexOptions.None,
      (current, token) => current | ParseRegexOption(token));

  private static RT ParseRuleTypes (string value) => value.
    Split(' ', SSORT).
    Aggregate(
      RT.None,
      (current, token) => current | (Enum.TryParse(token, true, out RT result) ? result : throw Err.ThrowBadDef($"Invalid value {value}")));
}
