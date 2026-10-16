//using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Collections.Immutable;

using Parser.Tokens;

namespace Specification.INI;

/// <summary>Represents a INISection heading in an INI formatted file.</summary>
public sealed class INISection : IEnumerable<IProperty<string>>, ISerializable, ICloneable
{
  /// <summary>Creates an empty INISection.</summary>
  private INISection () { }
  /// <summary>Creates a <see cref="INISection"/> with the provided name.</summary>
  /// <param name="name">The name of the INISection.</param>
  [SetsRequiredMembers]
  public INISection (string name) => Name = name;
  public INISection (IToken token)
  {
    if (token is not ComplexToken ct)
    {
      throw Err.ThrowBadResult($"Token was not formed ({token.Content}).");
    }

    string? name = ct.Name?.Content;
    ImmutableList<IToken>? props = ct.Properties;

    if (name is null)
    {
      throw Err.ThrowBadResult($"Token was missing name piece \"{ct.Content}\".");
    }

    Name = name;

    if (props is null || props.Count == 0)
    {
      Debug.Log(MsgClass.Warning, $"Section {name} had no properties.", this);
      return;
    }

    foreach (IToken prop in props)
    {
      if (prop.Type.Like("Property"))
      {
        Set(new(prop));
      }
    }
  }
  /// <summary>Creates a INISection from a string.</summary>
  /// <param name="name">The name of the section.</param>
  public static implicit operator INISection (string name) => new() { Name = name };
  /// <summary>The name of the section.</summary>
  public string Name { get; set; } = SE;
  /// <summary>The properties within the INISection.</summary>
  private Dictionary<string, string> Properties { get; init; } = [];
  /// <summary>Gets the value of a property from a given key.</summary>
  /// <param name="key">The key of the property.</param>
  /// <returns>The value of the property.</returns>
  public string? this[string key]
  {
    get => Properties[key];
    set => Set(key, value ?? SE);
  }
  /// <summary>The number of properties this section contains.</summary>
  public int Count => Properties.Count;
  /// <summary>Gets the <see langword="string"/> representation of the object for serialization.</summary>
  /// <param name="k">The <see cref="KeyValuePair{TKey, TValue}"/> to serialize.</param>
  /// <returns>The <see langword="string"/> representation of the object.</returns>
  public static string SerializeProp (KeyValuePair<string, string> k) => $"  {k.Key}={k.Value}";
  public static PropertyBase<string> GenerateProp (ComplexToken input)
  {
    input.ThrowIfNull();
    IToken name = input[TokenRef.Name];
    IToken value = input[TokenRef.Value];
    return new() { Key = name.Content, Value = value.Content };
  }
  /// <summary>Sets the given property to the given value.</summary>
  /// <param name="key">The key name.</param>
  /// <param name="value">The value to set it to.</param>
  public void Set (string key, string value)
  {
    if (!Properties.TryAdd(key, value))
      Properties[key] = value;
  }
  /// <summary>Sets the property and value given, or adds the property and value if it does not exist.</summary>
  /// <param name="prop">The property to add or apply.</param>
  public void Set (PropertyObj prop)
  {
    if (prop is null || prop.Value is null)
      return;
    Properties[prop.Key] = prop.Value;
  }
  public void SetRange (IEnumerable<PropertyObj> children)
  {
    children.ThrowIfNull();
    foreach (PropertyObj child in children)
    {
      Set(child);
    }
  }
  /// <summary>Adds the property, or sets the value of the property if it already exists.</summary>
  /// <param name="child">The property to add.</param>
  public void Add (PropertyObj child) => Set(child);
  /// <summary>Adds multiple properties, or sets the values for any that already exist.</summary>
  /// <param name="children">The properties to add.</param>
  public void AddRange (IEnumerable<PropertyObj> children) => SetRange(children);
  public IEnumerator<IProperty<string>> GetEnumerator () =>
    (from prop in Properties select new PropertyBase<string>() { Key = prop.Key, Value = prop.Value }).GetEnumerator();
  IEnumerator<IProperty<string>> IEnumerable<IProperty<string>>.GetEnumerator () => GetEnumerator();
  public void Clear () => Properties.Clear();
  public bool Contains (PropertyObj item) => item?.Key is not null && Properties.ContainsKey(item.Key);
  public bool Remove (PropertyObj item) => !(item is null || item.Key is null || !Properties.Remove(item.Key));

  /// <summary>Removes the property with the provided name.</summary>
  /// <param name="name">The name of the property to remove.</param>
  /// <returns><see langword="true"/> if the property was removed, <see langword="false"/> otherwise.</returns>
  public bool Remove (string name) => Properties.Remove(name);
  public string Serialize () => $"[{Name}]";
  public object Clone ()
  {
    INISection result = new(Name);

    foreach (KeyValuePair<string, string> item in Properties)
    {
      result.Properties.Add(item.Key, item.Value ?? SE);
    }
    return result;
  }
  IEnumerator IEnumerable.GetEnumerator () => Properties.GetEnumerator();
}
