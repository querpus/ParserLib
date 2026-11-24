using Parser.Inference;

namespace Parser;

/// <summary>This is a static class for <see cref="Spec"/> definitions.</summary>
public static class DefinitionStaticFunctions
{
  // Methods
  public static RxS NmOr (string name, Collection<string> options) => RxS.GrpNm(name, Or(options));
  public static RxS Nm (string name, [SS("Regex")] string rx) => RxS.GrpNm(name, rx);
  public static RxS Gp ([SS("Regex")] string rx) => RxS.Grp(rx);
  public static RxS Gp ([SS("Regex")] params Collection<string> rxs) => RxS.Grp(RxS.OOr(rxs));
  public static RxS Rx ([SS("Regex")] string rx) => RxS.Rx(rx);
  public static RxS Rx ([SS("Regex")] params Collection<string> values) => Or(values);
  public static RxS Or ([SS("Regex")] IEnumerable<string> list) => RxS.OOr(list);
  public static RxS Or ([SS("Regex")] string content, [SS("Regex")] params Collection<string> values) => RxS.Or(content, values);
  public static InferenceNode IfN (IT it, string value) => new(it, value);
  public static InferenceNodeGroup IfNOr (params IEnumerable<InferenceNode> nodes) => new(nodes, IT.Or);
  public static InferenceNodeGroup IfNAnd (params IEnumerable<InferenceNode> nodes) => new(nodes, IT.And);

  // Word Start RxS
  public static readonly RxS St = Rx(@"\b");

  // IT Flag Combos
  public const IT
    ExtIs = Ext | Is,
    HeadSt = FileHeader | Start,
    BodyContains = FileContent | Contains;

  // Constants
  public const IT
    Ext = IT.Ext,
    FName = IT.FName,
    FileHeader = IT.FileHeader,
    FileContent = IT.FileContent,
    FileSize = IT.FileSize,

    And = IT.And,
    ITOr = IT.Or,
    Is = IT.Is,
    End = IT.End,
    Start = IT.Start,
    Contains = IT.Contains,
    Larger = IT.Larger,
    Smaller = IT.Smaller,
    Not = IT.Not;
}
