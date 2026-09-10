#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class SectionEntity : Entity
{
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    set => DataValues["Name"] = value;
  }
  public override BT Type => BT.Section;

  public override bool Equals (IEntity? other) =>
    other is SectionEntity ce && Name.Is(ce.Name) && Properties.SequenceEqual(ce.Properties);
  public override string Serialize () => $"[{Name}]" + '\n' + Properties.TextJoin("\n");
  public Dictionary<string, IEntity> Properties => (Dictionary<string, IEntity>) PropertyCollections["Properties"];
}
