//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel;

namespace Common.Extensions;

public static class TupleExtensions
{
  public static KeyValuePair<string, string> ToKVP (this (string Key, string Value) t) => new(t.Key, t.Value);
  public static KeyValuePair<TKey, TValue> ToKVP<TKey, TValue> (this (TKey Key, TValue Value) t) => new(t.Key, t.Value);
}

/// <summary>Converter between a <see cref="ReplaceNode"/>s and a <see cref="KeyValuePair"/>.</summary>
public class ReplaceNodeKVPConverter : TypeConverter
{
  /// <summary>Allows conversion from a tuple to a <see cref="KeyValuePair"/>.</summary>
  /// <param name="context"></param>
  /// <param name="sourceType">The type to convert from.</param>
  /// <returns><see langword="true"/> if it can be converted, <see langword="false"/> otherwise.</returns>
  public override bool CanConvertFrom (ITypeDescriptorContext? context, Type sourceType) =>
    sourceType == typeof(ReplaceNode) || base.CanConvertFrom(context, sourceType);

  public override object ConvertFrom (ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
    value is ReplaceNode node
      ? new KeyValuePair<string, string?>(node.LookFor, node.ReplaceWith ?? SE) as object
      : throw new NotSupportedException($"Cannot convert from {value?.GetType()?.Name ?? "<null type>"} to KeyValuePair<string, string?>.");

  public override bool CanConvertTo (ITypeDescriptorContext? context, Type? destinationType) =>
    destinationType == typeof(ReplaceNode) || base.CanConvertTo(context, destinationType);
  /// <summary>Converts a value to a given type.</summary>
  /// <param name="context">The context.</param>
  /// <param name="culture">The culture info.</param>
  /// <param name="value">The value to convert.</param>
  /// <param name="destinationType">The type to convert to.</param>
  /// <returns>The converted object.</returns>
  /// <exception cref="NotSupportedException">Conversion failed.</exception>
  public override object ConvertTo (ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
    destinationType == typeof(ReplaceNode) && value is KeyValuePair<string, string?> kvp
      ? new ReplaceNode(kvp.Key, kvp.Value)
      : throw new NotSupportedException($"Cannot convert from KeyValuePair<string, string?> to {destinationType}.");
}
