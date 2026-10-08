#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public class QString : ITextSerializer, IEquatable<string>, IComparable<string>
{
  public string Value { get; set; } = SE;
  public string Quote { get; set; } = Chars.QTs;
  public QString () { }
  public QString (string value, string quote)
  {
    Value = value;
    Quote = quote;
  }
  public QString (string? value)
  {
    if (value?.StartsWithAny(["\"", "'"]) == true && value[0] == value[^1])
    {
      Value = value[1..^1];
      Quote = $"{value[0]}";
    }
    else if (value is not null)
    {
      Value = value;
    }
  }

  public override string ToString () => $"{Quote}{Value}{Quote}";
  public string Serialize () => $"{this}";
  public int CompareTo (string? other) => Value.CompareTo(new QString(other ?? SE), SCO);
  public bool Equals (string? other) => Serialize().Equals(other, SCO) || Value.Equals(other, SCO);

  public static implicit operator QString (string? value) => new(value);

  public static implicit operator string (QString qstring) => $"{qstring}";
}
