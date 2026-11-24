using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Common.Extensions;

namespace Specification.REG;

/// <summary>A registry key.</summary>
public class RegSection : IReadOnlyDictionary<string, string?>
{
  /// <summary>Signifies that this key is to be deleted, not added.</summary>
  public bool IsDeleteKey { get; set; }
  /// <summary>The path of the key.</summary>
  public string Path { get; set; }

  private RegSection () => Path = SE;
  /// <summary>Creates a key from the path and whether or not its a delete key.</summary>
  public RegSection (string path, bool delete = false)
  {
    Path = path;
    IsDeleteKey = delete;
  }
  /// <summary>Creates a key from the parent path and a name.</summary>
  /// <param name="parentPath">The parent path.</param>
  /// <param name="name">The name.</param>
  public RegSection (string parentPath, string name) => Path = $"{parentPath}\\{name}";
  /// <summary>An empty section.</summary>
  public static RegSection Blank { get; } = new() { Name = SE };
  /// <summary>The name of the section.</summary>
  public string Name { get; set; } = SE;
  /// <summary>The properties within the section.</summary>
  protected Dictionary<string, string?> Properties { get; } = [];
  public int Count => Properties.Count;

  public IEnumerable<string> Keys => Properties.Keys;

  public IEnumerable<string?> Values => Properties.Values;

  public string? this[string key] => Properties[key];

  public void Add (RegProperty child)
  {
    if (child is null)
      return;
    Properties.Add(child);
  }
  public void AddRange (IEnumerable<RegProperty> children)
  {
    children.ThrowIfNull();
    foreach (RegProperty child in children)
    {
      Add(child);
    }
  }

  public IEnumerator<KeyValuePair<string, string?>> GetEnumerator () => Properties.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  public bool ContainsKey (string key) => Properties.ContainsKey(key);
  public bool TryGetValue (string key, [MaybeNullWhen(false)] out string value) => Properties.TryGetValue(key, out value);
}
