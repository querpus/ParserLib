namespace Parser.Ops;

public class AddItemOperation : Operation
{
  public required string ListKey { get; init; }
  /// <summary>A dictionary of property names and data keys to pull their value from.</summary>
  public Dictionary<string, string> ParameterKeys { get; init; } = [];
  public required string Type { get; init; }

  protected override void Execute ()
  {
    Type? t = System.Type.GetType(Type);

    if (t is null)
    {
      _ = Err.ThrowBadDef("Object " + Type + "not found.");
    }

    Dictionary<string, object> values = [.. ParameterKeys.Select(kvp => new KeyValuePair<string, object>(kvp.Key, Data[kvp.Value]))];

    object? created = t.GetConstructor([typeof(Dictionary<string, object>)])?.Invoke([.. values.Values]);

    if (Data[ListKey] is IList<object> il && created is not null)
      il.Add(created);
    else
      throw Err.ThrowBadResult("ListKey was not an IList");
  }
}
