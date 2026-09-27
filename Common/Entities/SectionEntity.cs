#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class SectionEntity : Entity
{
  public string? Name
  {
    get => DataValues.TryGetValue("Name", out object? value) ? (string) value! : null;
    set => DataValues["Name"] = value;
  }
  public override bool IsValid => Name is not null;
  public override string Serialize () => $"[{Name}]" + '\n' + Properties.TextJoin("\n");
  protected override void Assign (Match match) { } //TODO: Implement
}
