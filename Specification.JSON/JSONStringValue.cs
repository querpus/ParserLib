using System.Text.Json;

namespace Specification.JSON;

/// <summary>A JSON text value.</summary>
/// <param name="value">The textual value to assign.</param>
public class JSONStringValue (string value) : IJSONNode
{
  public string Value { get; set; } = value;
  public JsonValueKind Type => JsonValueKind.String;
  public override string ToString () => $"\"{Value}\"";
  object? IJSONNode.Value => Value;
}
