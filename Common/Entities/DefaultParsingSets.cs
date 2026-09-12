#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public static class DefaultParsingSets
{
  public static ParsingInfo XML { get; } = new ParsingInfo()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    TotalPasses = 2,
    EntityOptions = [
    new() {
      IndicatedItem = new() { Group = "header" },
      Type = BT.Element,
    }, new() {
      IndicatedItem = new() { Group = "close" },
      Type = BT.Element,
      DepthChange = -1,
    }, new() {
      IndicatedItem = new() { Group = "single" },
      Type = BT.Element,
    }, new() {
      IndicatedItem = new() { Group = "element" },
      Type = BT.Element,
      
      DepthChange = 1,
      SetAsNextLevelParent = true,
    }, new() {
      CreateEmptyAtStart = true,
      Type = BT.Document,
      OnlyAtTopLevel = true,
      SetAsNextLevelParent = true,
    },new() {
      IndicatedItem = new() { Group = "ws" },
      Type = BT.IgnoredWhitespace,
    }, new() {
      IndicatedItem = new() { Group = "content" },
      Type = BT.LooseContent,
      StoresData = true,
    }, new() {
      IndicatedItem = new() { Group = "ws" },
      Type = BT.IgnoredWhitespace,
    }, new() {
      IndicatedItem = new() { Group = "ws" },
      Type = BT.IgnoredWhitespace,
    }, new() {
    }],
    Comments = [
      new() {Open = "<!==", Close = "-->", Expression = ""}
    ]
  };
  public static ParsingInfo JSON { get; } = new ParsingInfo()
  {
    GeneratesSingleObject = true,
    SingleObject = new() {
      OnlyAtTopLevel = true,
      CreateEmptyAtStart = true,
      DefinesStructure = true,
      Type = BT.Document,
    },
    IgnoreCase = false,
    TotalPasses = 2,
    EntityOptions = [
    new() {
      IndicatedItem = new() { Group = "key" },
      StoresData = true,
      Type = BT.Property,
      StorePieceTypes = new() { ["Name"] = "name" },
      SetPropKey = true
    }, new() {
      IndicatedItem = new() { Group = "str_value" },
      StoresData = true,
      AddToPropKey = true,
      Type = BT.String,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      IndicatedItem = new() { Group = "bool_value" },
      StoresData = true,
      Type = BT.Boolean,
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      IndicatedItem = new() { Group = "num_value" },
      StoresData = true,
      Type = BT.Number,
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      IndicatedItem = new() { Group = "null_value" },
      AddToPropKey = true,
      Type = BT.Null,
    }, new() {
      IndicatedItem = new() { Group = "comment" },
      Type = BT.Comment,
    }, new() {
      IndicatedItem = new() { Group = "ws" },
      Type = BT.IgnoredWhitespace,
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "{" },
      DepthChange = 1,
      AddToPropKey = true,
      Type = BT.Object,
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "}" },
      DepthChange = -1,
      Type = BT.Omit,
      ConstantValue = "}",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "[" },
      DepthChange = 1,
      AddToPropKey = true,
      Type = BT.Array,
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = ":" },
      Type = BT.Omit,
      ConstantValue = ":",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "," },
      Type = BT.Omit,
      ConstantValue = ",",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "]" },
      DepthChange = -1,
      Type = BT.Omit,
      ConstantValue = "]",
    }]
  };
}
