#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class SectionEntity : Entity
{
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    set => DataValues["Name"] = value;
  }

  public override bool Equals (IEntity? other) =>
    other is SectionEntity ce && Name.Is(ce.Name) && Properties.SequenceEqual(ce.Properties);
  public override string Serialize () => $"[{Name}]" + '\n' + Properties.TextJoin("\n");
}
