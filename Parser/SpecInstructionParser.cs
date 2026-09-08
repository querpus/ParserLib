#pragma warning disable CA1710 // Rename Parser.Library to end in either 'Dictionary' or 'Collection'

using System.Xml.Linq;

using Common.Entity;

using Parser.Inference;

namespace Parser;

public static class SpecInstructionParser
{
  private static readonly XNamespace NS = "Parser/Spec";

  private static bool BooleanParse (string? text, bool value_on_fail) => text is null
      ? value_on_fail
      : !text.Like(["false", "0", "no"]) && (text.Like(["true", "1", "yes"])
      ? true
      : throw Err.ThrowNoSpec($"Bad Boolean Value Encountered: {text}"));

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
    Collection<Spec> specs = [];

    Log(MsgClass.Debug, $"Loading spec xml file '{path}'", "SpecInstructionParser");

    XDocument doc = XDocument.Load(path);
    XElement root = doc.Root ?? throw Err.ThrowNoSpec("Spec XML is not good.");
    DocumentEntity aRoot = (DocumentEntity) EntityFactory.FromXElement(root, null);
    ElementEntity bRoot = (ElementEntity) (aRoot.RootNode ?? throw new InvalidDataException("Root node should not be null."));
    ElementEntity prefabs = bRoot.Children.OfType<ElementEntity>().Where(i => i.Name == "Prefabs").At(0);

    if (!bRoot.Name.Is("Definition"))
    {
      _ = Err.ThrowNoSpec("Root element must be Definition.");
    }

    if (prefabs is not null)
    {
      IEnumerable<ElementEntity> prefab_tokenRules_list = prefabs.Children.OfType<ElementEntity>().Where(i => i.Name == "TokenRule");
      IEnumerable<ElementEntity> prefab_tokenLookups_list = prefabs.Children.OfType<ElementEntity>().Where(i => i.Name == "TokenLookup");
      IEnumerable<ElementEntity> prefab_tokenGroups_list = prefabs.Children.OfType<ElementEntity>().Where(i => i.Name == "GroupTokenRule");
      IEnumerable<ElementEntity> prefab_constructs_list = prefabs.Children.OfType<ElementEntity>().Where(i => i.Name == "Construct");
    }

    foreach (XElement specxml in root.Elements(NS + "Spec"))
    {
      XElement? xname = specxml.Element(NS + "Name");

      if (xname is null || string.IsNullOrEmpty(xname.Value))
        Err.ThrowNoSpec("Invalid XML - No Name in Spec.");

      XElement? tokenLookups = specxml.Element(NS + "TokenLookups");
      XElement? tokenRules = specxml.Element(NS + "TokenRules");
      string name = specxml.Element(NS + "Name")!.Value;
      string textfile = specxml.Element(NS + "TextFile")!.Value;
      bool is_textfile = BooleanParse(textfile, false);
      IEnumerable<XElement>? instructionElements = specxml.Element(NS + "Instructions")?.Elements();

      Collection<IOperation> ops = [];
      foreach (XElement element in instructionElements ?? [])
      {
        IOperation? op = OperationFactory.Produce(element);

        if (op is not null)
          ops.Add(op);
        else
          Log(MsgClass.Warning, $"{element.Name} was null, investigate.", "SpecInstructionParser");
      }

      XElement? fileInf = specxml.Element(NS + "FileInferences");
      ReadOnlyCollection<InferenceNode> inferenceNodes = [];// = ParseFileInferences(fileInf);

      Spec specobj = new()
      {
        Name = name,
        Operations = [.. ops],
        FileInferences = inferenceNodes,
        IsTextFile = is_textfile,
      };

      specs.Add(specobj);
    }

    return specs;
  }
}
