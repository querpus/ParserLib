#pragma warning disable CA1710 // Identifiers should have correct suffix

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Specification.JSON;

public sealed class JSONObject : IJSONNode, IDictionary<string, IJSONNode>, IEnumerable<IJSONNode>
{
  public JsonValueKind Type => JsonValueKind.Object;
  public Dictionary<string, IJSONNode> Properties { get; init; } = [];

  public ICollection<string> Keys => Properties.Keys;

  public ICollection<IJSONNode> Values => Properties.Values;

  public int Count => Properties.Count;

  public bool IsReadOnly => false;

  object? IJSONNode.Value => Properties;

  public IJSONNode this[string key]
  {
    get => Properties[key];
    set => Properties[key] = value;
  }

  public void Add (string key, IJSONNode value) => Properties.Add(key, value);
  public bool ContainsKey (string key) => Properties.ContainsKey(key);
  public bool Remove (string key) => Properties.Remove(key);
  public bool TryGetValue (string key, [MaybeNullWhen(false)] out IJSONNode value) => Properties.TryGetValue(key, out value);
  public void Add (KeyValuePair<string, IJSONNode> item) => Properties.Add(item);
  public void Clear () => Properties.Clear();
  public bool Contains (KeyValuePair<string, IJSONNode> item) => ((ICollection<KeyValuePair<string, IJSONNode>>) Properties).Contains(item);
  public void CopyTo (KeyValuePair<string, IJSONNode>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, IJSONNode>>) Properties).CopyTo(array, arrayIndex);
  public bool Remove (KeyValuePair<string, IJSONNode> item) => ((ICollection<KeyValuePair<string, IJSONNode>>) Properties).Remove(item);
  public IEnumerator<KeyValuePair<string, IJSONNode>> GetEnumerator () => Properties.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  IEnumerator<IJSONNode> IEnumerable<IJSONNode>.GetEnumerator () => (IEnumerator<IJSONNode>) GetEnumerator();
}
