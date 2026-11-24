using System;

namespace Specification.XML;

/// <summary>Represents an XML attribute.</summary>
public sealed class XMLAttr () : IXMLObject, IProperty<string>, IEquatable<IProperty<string>>
{
  /// <summary>The attribute name.</summary>
  public string Key { get; set; } = SE;
  /// <summary>The attribute value.</summary>
  public string? Value { get; set; } = SE;
  /// <summary>The attribute name formatted as uppercase invariant.</summary>
  public string UKey => Key.ToUpperInvariant();

  /// <summary>The attribute name.</summary>
  string IXMLObject.Tag => Key;
  public int CompareTo (IProperty<string>? other) => other is null ? -1 : other.CompareTo(this) * -1;
  public bool Equals (IProperty<string>? other) => other is not null && other.Key == Key && other.Value == Value;
  public override bool Equals (object? obj) => obj is IProperty<string> iprop && Key == iprop.Key && Value == iprop.Value?.ToString();
  public override int GetHashCode () => HashCode.Combine(Key, Value);
  public string Serialize () => $"{Key} = \"{Value}\"";
  public override string ToString () => Serialize();

  /// <summary>Standard equality operator.</summary>
  /// <param name="left">The left property.</param>
  /// <param name="right">The right property.</param>
  /// <returns>Returns <see langword="true"/> if the 2 properties are equal.</returns>
  public static bool operator == (XMLAttr left, XMLAttr right) => left is null ? right is null : left.Equals(right);
  public static bool operator != (XMLAttr left, XMLAttr right) => !(left == right);
  public static bool operator < (XMLAttr left, XMLAttr right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (XMLAttr left, XMLAttr right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (XMLAttr left, XMLAttr right) => left?.CompareTo(right) > 0;
  public static bool operator >= (XMLAttr left, XMLAttr right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
