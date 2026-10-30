using System.Xml.Linq;

using Common.Nodes;

using Parser.Condition;
using Parser.Ops.Text;

namespace Parser.Ops;

public static class OperationFactory
{
  private static readonly XNamespace NS = "Parser/Spec";

  private static string? GetA (string name, XElement? parent = null) => parent?.Attribute(name)?.Value;
  private static int GetI (string name, XElement? parent = null) => GetA(name, parent) is not string s ? -1 : int.Parse(s, CIIC);
  private static string GetS (string name, XElement? parent = null) => GetA(name, parent) ?? SE;
  private static bool GetB (string name, XElement? parent = null) => bool.TryParse(GetA(name, parent), out bool result) && result;
  private static Collection<IOperation> GetOps (XElement? parent = null)
  {
    IfDepth++;
    IfDictionary[IfDepth] = null;
    IEnumerable<IOperation?> ops = [.. parent?.Elements().Select(Produce) ?? []];
    IfDepth--;
    IEnumerable<IOperation> opsnn = ops.Where(i => i is not null)!;
    return [.. opsnn];
  }
  private static SwitchCaseItem GetCase (XElement? parent = null) => new()
  {
    Value = parent?.Attribute(NS + "value")?.Value,
    Operations = GetOps(parent)
  };
  private static SwitchCaseItem? GetDefault (XElement? parent = null)
  {
    IEnumerable<SwitchCaseItem> cases = GetElems("Default", parent).Select(GetCase);

    return cases.IsEmpty ? null : cases.Single();
  }
  private static IEnumerable<SwitchCaseItem> GetSwitchCases (XElement? parent = null)
  {
    IEnumerable<SwitchCaseItem> cases = GetElems("Case", parent).Select(GetCase);
    XElement? def = GetElems("Default", parent).FirstOrDefault();

    return cases;
  }
  private static IfBlockConditional GetIfOption (XElement? parent = null) => new()
  {
    Condition = parent?.Attribute(NS + "condition")?.Value,
    Operations = GetOps(parent)
  };
  private static Dictionary<string, string> GetValueList (XElement? parent = null) => [.. parent?.Elements().Select(i => new KeyValuePair<string, string>(i.Attribute("name")?.Value ?? SE, i.Attribute("content_var")?.Value ?? SE)) ?? []];
  //private static IEnumerable<XElement> GetElems (XElement? parent = null) => parent?.Elements() ?? [];
  private static IEnumerable<XElement> GetElems (string name, XElement? parent = null) => parent?.Elements(NS + name) ?? [];
  private static readonly Dictionary<int, OperationIf?> IfDictionary = new() { [0] = null };
  private static int IfDepth;
  public static IOperation? Produce (XElement? element)
  {
    try
    {
      if (element is null)
        throw new UnknownOperationException("Element was null.");

      int target = GetI("target", element);
      int position = GetI("position", element);
      int length = GetI("length", element);
      int divisor = GetI("divisor", element);
      int dividend = GetI("dividend", element);

      string initial_var = GetS("initial_var", element);
      string target_var = GetS("target_var", element);
      string output_var = GetS("output_var", element);
      string input_var = GetS("input_var", element);
      string content_var = GetS("content_var", element);
      string cursor_var = GetS("cursor_var", element);
      string position_var = GetS("position_var", element);
      string length_var = GetS("length_var", element);
      string list_var = GetS("list_var", element);
      string user_var = GetS("user_var", element);
      string check_var = GetS("check_var", element);
      string dividend_var = GetS("dividend_var", element);
      string divisor_var = GetS("divisor_var", element);
      string extracted_var = GetS("extracted_var", element);

      string condition = GetS("condition", element);
      string message = GetS("message", element);
      string value = GetS("value", element);
      string name = GetS("name", element);
      string type = GetS("type", element);
      string key_type = GetS("key_type", element);
      string value_type = GetS("value_type", element);
      string success = GetS("success", element);
      string endian = GetS("endian", element);
      string encoding = GetS("encoding", element);
      string pattern = GetS("pattern", element);
      string tokenizer = GetS("tokenizer", element);

      bool strict = GetB("strict", element);
      bool binary = GetB("binary", element);

      //Reset block if any non-conditional block is reached.
      //This allows for multiple if blocks in a row without else if/else sections.
      string lname = element.Name.LocalName;

      if (lname is "If")
      {
        IfBlockConditional section = GetIfOption(element);
        OperationIf block = new() { Options = [section] };
        IfDictionary[IfDepth] = block;
        return block;
      }

      if (lname is "ElseIf")
      {
        OperationIf? @if = IfDictionary[IfDepth] ?? throw Err.ThrowBadDef("ElseIf block without preceding If block.");
        IfBlockConditional section = GetIfOption(element);
        @if.Options.Add(section);
        return null;
      }

      if (lname is "Else")
      {
        OperationIf? @if = IfDictionary[IfDepth] ?? throw Err.ThrowBadDef("ElseIf block without preceding If block.");
        IfBlockConditional section = GetIfOption(element);
        @if.Options.Add(section);
        _ = IfDictionary.Remove(IfDepth);
        return null;
      }

      IfDictionary[IfDepth] = null;

      return lname switch
      {
        "GotoOpIndex" when target is not DNE and 0 => new JumpOperation { TargetIndex = target },
        "GotoOpIndex" when target_var.IsNotEmpty => new JumpOperation { TargetIndexVar = target_var },
        "GotoLabel" => new JumpOperation
        {
          TargetLabel = name
        },
        "Label" => new OperationLabel
        {
          Name = name
        },
        "ReadData" when length_var.IsEmpty => new ReadDataOperation
        {
          Mode = type,
          Length = length == -1 ? type switch { "byte" => 1, "short" => 2, "int" => 4, "long" => 8, _ => -1 } : length,
          ContentKey = content_var,
          CursorKey = cursor_var,
          Position = position,
          PositionKey = position_var,
          OutputKey = output_var,
          Endianness = endian,
          Encoding = encoding,
        },
        "ReadData" when length_var.IsNotEmpty => new ReadDataOperation
        {
          Mode = type,
          LengthKey = length_var,
          OutputKey = output_var,
          ContentKey = content_var,
          CursorKey = cursor_var,
          Position = position,
          PositionKey = position_var,
          Endianness = endian,
          Encoding = encoding,
        },
        "MakeCursor" => new MakeCursorOperation
        {
          CursorKey = cursor_var,
          ListKey = list_var,
          Position = position == -1 ? 0 : position
        },
        "SetCursorPos" => new SetCursorOperation
        {
          CursorKey = cursor_var,
          Position = position,
          PositionKey = position_var
        },
        "Tokenize" => new TokenizeOperation
        {
          InputKey = input_var,
          OutputKey = output_var,
          Tokenizer = tokenizer
        },
        "TokenAssemble" => new TokenAssembleOperation
        {
          InputKey = input_var,
          OutputKey = output_var
        },
        "Terminate" => new OperationEnd(success.Like("false")),
        // These are setup during unpacking, so they can be used as placeholders for
        // break/continue targets in loops and switches. They will be replaced with the
        // correct target index during unpacking.
        "Break" => OperationBreak.Null,
        "Continue" => OperationContinue.Null,
        "Switch" => new OperationSwitch
        {
          ConditionString = condition,
          Cases = [.. GetSwitchCases(element)],
          Default = GetDefault(element)
        },
        "ForCount" => new ForCountOperation
        {
          CursorKey = cursor_var,
          Length = length,
          LengthKey = length_var,
          Operations = GetOps(element),
        },
        "While" => new WhileOperation
        {
          Condition = condition,
          CursorKey = cursor_var,
          Operations = GetOps(element),
        },
        "Prompt" => new PromptOperation
        {
          Message = message,
          UserKey = user_var,
          Validation = null, //TODO: Add validation support! regex?
        },
        "ForEach" => new ForEachOperation
        {
          CursorKey = cursor_var,
          ListKey = list_var,
          Operations = GetOps(element),
        },
        "Initialize" => new InitializeOperation
        {
          InitialKey = initial_var,
          Type = type,
          KeyType = key_type,
          ValueType = value_type,
        },
        "AddItem" => new AddItemOperation
        {
          ListKey = list_var,
          Type = type,
          ParameterKeys = GetValueList(element)
        },
        "Print" => new DebugPrintKeyOperation { InputKey = check_var },
        // TODO: Replace with expression evaluation system that can handle
        // more than just basic math operations. It is built.
        "Divide" => new DivideOperation
        {
          DividendKey = dividend_var,
          DivisorKey = divisor_var,
          Dividend = dividend == -1 ? 0 : dividend,
          Divisor = divisor == -1 ? 0 : divisor,
          OutputKey = output_var,
        },
        "Split" => new SplitOperation
        {
          InputKey = input_var,
          OutputKey = output_var,
          Procedure = type,
          Delims = []
          //TODO: Finish this...
        },
        "Extract" => new ExtractOperation
        {
          InputKey = input_var,
          OutputKey = output_var,
          ExtractedKey = extracted_var,
          Pattern = pattern,
        },
        "Load" => new LoadOperation
        {
          InputKey = input_var,
          OutputKey = output_var,
          IgnoreMissing = !strict,
          LoadBinary = binary,
        },
        _ => Err.ThrowBadDef($"Unknown Command {element.Name.LocalName}.")
      };
    }
    catch (OperationException oe)
    {
      throw Err.ThrowBadDef($"Error during Spec Parsing: {oe.Message}");
    }
  }
}
