using System.Linq;

using Parser.Exceptions;

namespace Specification.JSON;

public class JSONOperation : Operation
{
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  protected int Index { get; set; }
  protected IToken? TCurrent => Index >= Tokens.Count ? null : Tokens[Index];
  protected Collection<IToken> Tokens { get; } = [];
  protected void Init (IEnumerable<IToken> tokens) => Tokens.AddRange([.. tokens]);
  protected override void Execute ()
  {
    if (Data[InputKey] is not IEnumerable<IToken> tokens)
    {
      throw Err.ThrowBadInput(nameof(TokenCollection), Data[InputKey].TypeName);
    }

    Init(tokens);

    int depth = 0;
    Collection<Collection<IJSONNode>> assembly = [[]];
    for (Index = 0; Index < Tokens.Count; Index++)
    {

      Collection<string> order = [];
      void addContainer (string open)
      {
        Action go_deeper = innerArrayHelper;
        string close = "]";
        IJSONNode container = new JSONArray();
        if (open is "{")
        {
          go_deeper = innerObject;
          close = "}";
          container = new JSONObject();
        }
        assembly[depth].Add(container);
        depth++;
        if (assembly.Count <= depth)
          assembly.Add([]);
        order.Add(close);
        Index++;
        go_deeper();
      }
      void closeContainer (string close)
      {
        depth--;
        order.Drop();
        if (close is "}" && assembly[depth].Last() is JSONObject obj)
        {
          string? key = null;
          foreach (IJSONNode item in assembly[depth + 1])
          {
            if (key is null)
            {
              key = item.Value?.ToString();
            }
            else
            {
              obj.Add(key, item);
            }
          }
        }
        else if (close is "]" && assembly[depth].Last() is JSONArray arr)
        {
          arr.AddRange(assembly[depth + 1]);
        }
        assembly.RemoveAt(depth + 1);
        Index++;
      }
      void innerArrayHelper () => innerArray(false);
      void addValueToAssembly ()
      {
        assembly[depth].Add(TCurrent.Type switch
        {
          _ when TCurrent.Content.IsEmpty => new JSONUndefValue(),
          "Str" => new JSONStringValue(TCurrent.Content),
          "Num" => new JSONNumberValue(TCurrent.Content.ToDecimal() ?? 0),
          "Bool" => new JSONBoolValue(TCurrent.Content.ToBool() ?? false),
          "Null" => new JSONNullValue(),
          _ => new JSONUndefValue(),
        });
        Index++;
      }
      void innerArray (bool initial)
      {
        Debug.Log(MsgClass.Debug, $"{Index} : Array Entered", this);
        int start_point = Index;
        int exit_depth = depth;
        int sequence = 0;
        while (depth >= exit_depth && Index < Tokens.Count)
        {
          if (TCurrent is null)
            break;
          string? tContent = TCurrent.Content;
          string tType = TCurrent.Type;
          if (tContent is "{" or "[")
          {
            ThrowIf(sequence != 0, $"Sequence was not correct, {sequence} needs to be 0.");
            Debug.Log(MsgClass.Debug, $"{Index} : Opener '{tContent}'", this);
            addContainer(tContent);
            sequence++;
          }
          else if (tContent is "}" or "]")
          {
            ThrowIf(order.Peek() != tContent, $"Expected to close a {order.Peek()}, but got a {tContent}.");
            ThrowIf(depth != exit_depth, $"Depth was not correct, {depth} needs to be {exit_depth}.");
            ThrowIf(sequence != 1, $"Sequence was not correct, {sequence} needs to be 1.");
            Debug.Log(MsgClass.Debug, $"{Index} : Closer '{tContent}'", this);
            closeContainer(tContent);
            return;
          }
          else if (tContent == ",")
          {
            ThrowIf(sequence != 1, $"Sequence was not correct, {sequence} needs to be 1.");
            ThrowIf(initial, "Only one parent object allowed.");
            Debug.Log(MsgClass.Debug, $"{Index} : Comma '{tContent}'", this);
            sequence = 0;
            Index++;
          }
          else if (tContent is "=" or ":")
          {
            // No property keys in an array.
            ThrowIf(true, $"Invalid token '='. Expected {(sequence == 0 ? "value" : $", OR {order.Peek()}")}.");
            Debug.Log(MsgClass.Debug, $"{Index} : op '{tContent}' [ERROR]", this);
            sequence = 0;
            Index++;
          }
          else if (tType is "Num" or "Null" or "Str" or "Bool")
          {
            ThrowIf(sequence != 0, $"Sequence was not correct, {sequence} needs to be 0.");
            Debug.Log(MsgClass.Debug, $"{Index} : primitive '{tType}'", this);
            sequence++;
            addValueToAssembly();
          }
          else
          {
            Debug.Log(MsgClass.Debug, $"{Index} : unknown type '{tContent}'", this);
          }
        }
      }
      void innerObject ()
      {
        Debug.Log(MsgClass.Debug, $"{Index} : Object Entered", this);
        int start_point = Index;
        int exit_depth = depth;
        int sequence = 0;
        while (depth >= exit_depth)
        {
          if (TCurrent is null)
            break;

          string? tContent = TCurrent.Content;
          string tType = TCurrent.Type;
          if (tContent is "{" or "[")
          {
            ThrowIf(sequence != 2, $"Sequence was not correct, {sequence} needs to be 2.");
            Debug.Log(MsgClass.Debug, $"{Index} : op '{tContent}'", this);
            addContainer(tContent);
            sequence++;
          }
          else if (tContent is ",")
          {
            ThrowIf(sequence != 3, $"Sequence was not correct, {sequence} needs to be 3.");
            Debug.Log(MsgClass.Debug, $"{Index} : op '{tContent}'", this);
            sequence = 0;
            Index++;
          }
          else if (tContent is "}" or "]")
          {
            ThrowIf(order.Peek() != tContent, $"Expected to close a {order.Peek()}, but got a {tContent}.");
            ThrowIf(depth != exit_depth, $"Depth was not correct, {depth} needs to be {exit_depth}.");
            ThrowIf(sequence != 3, $"Sequence was not correct, {sequence} needs to be 3.");
            Debug.Log(MsgClass.Debug, $"{Index} : op '{tContent}'", this);
            closeContainer(tContent);
            return;
          }
          else if (tContent is "=" or ":")
          {
            ThrowIf(sequence != 1, $"Sequence was not correct, {sequence} needs to be 1.");
            Debug.Log(MsgClass.Debug, $"{Index} : op '{tContent}'", this);
            sequence++;
            Index++;
          }
          else if (tType is "Num" or "Null" or "Bool")
          {
            ThrowIf(sequence != 2, $"Sequence was not correct, {sequence} needs to be 2.");
            Debug.Log(MsgClass.Debug, $"{Index} : primitive '{tType}'", this);
            sequence++;
            addValueToAssembly();
          }
          else if (tType is "Str")
          {
            ThrowIf(sequence is not 0 and not 2, $"Sequence was not correct, {sequence} needs to be 0 or 2.");
            Debug.Log(MsgClass.Debug, $"{Index} : primitive '{tType}'", this);
            sequence++;
            addValueToAssembly();
          }
          else
          {
            Debug.Log(MsgClass.Debug, $"{Index} : unknown type '{tContent}'", this);
            return;
          }
        }
      }

      try
      {
        innerArray(true);
      }
      catch (Exception e)
      {
        Debug.LogException(e);
        throw new OperationException("JSON Operation Failed", e);
      }

      Data[OutputKey] = assembly[0];
    }
  }
}
