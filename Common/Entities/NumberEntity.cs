#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An entity representing an <see langword="int"/> or <see langword="decimal"/>.<br/>
/// <br/>
/// Data Stored:<br/>
/// <c>* Value</c> - Decimal value.<br/>
/// <c>* Content</c> - Assigns to <c>Value</c> as a <see langword="string"/>.
/// </summary>
public class NumberEntity : Entity
{
  public bool IsInteger => Value % 1m == 0m;
  /// <summary>Gets or sets the decimal value of the entity.</summary>
  public decimal Value
  {
    get => (decimal) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  /// <summary>Gets or sets the decimal value as a <see langword="string"/>.</summary>
  public string Content
  {
    get => $"{DataValues["Value"]}";
    set => DataValues["Value"] = bool.Parse(value);
  }

  public override bool Equals (IEntity? other) => other is NumberEntity ne && ne.Value == Value;
  public override string Serialize () => $"{Value}";
  protected override void Assign (Match match)
  {
    Content = match.Groups["value"].Value;
    Origin = match.Value;
  }
}
