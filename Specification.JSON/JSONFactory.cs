#pragma warning disable CA1710 // Identifiers should have correct suffix

using System.Linq;

namespace Specification.JSON;

public sealed class JSONFactory ()
{
  public object? Input { get; set; }

  private IJSONNode? TreeLoop (object? input) => input switch
  {
    null => new JSONNullValue(),
    decimal d => new JSONNumberValue(d),
    string s => new JSONStringValue(s),
    bool b => new JSONBoolValue(b),
    IEnumerable<IProperty<object>> props => new JSONObject() { Properties = (from item in props select new KeyValuePair<string, IJSONNode>(item.Key, TreeLoop(item?.Value) ?? new JSONNullValue())).ToDictionary() },
    IEnumerable<object> list => new JSONArray() { Values = [.. (from item in list select TreeLoop(item))] },
    _ => new JSONObject() { Properties = (from prop in Input?.GetType().GetProperties(System.Reflection.BindingFlags.Instance) select new KeyValuePair<string, IJSONNode>(prop.Name, TreeLoop(prop?.GetValue(Input)) ?? new JSONNullValue())).ToDictionary() },
  };

  public IJSONNode? MakeTree (object input)
  {
    Input = input;
    return TreeLoop(Input);
  }
}
