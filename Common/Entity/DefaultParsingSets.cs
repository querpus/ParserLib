#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entity.BasicType;

namespace Common.Entity;

public static class DefaultParsingSets
{
  public static ParsingSet XML { get; } = new ParsingSet()
  {
    Global = new() {
      GeneratesSingleObject = true,
      IgnoreCase = false,
      TotalPasses = 2
    },
    EntityOptions = [
    new() {
      IndicatedItem = new() { Group = "header" },
      Type = BT.Element,
    }, new() {
      IndicatedItem = new() { Group = "header" },
      Type = BT.Element,
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
    }]
  };
  public static ParsingSet JSON { get; } = new ParsingSet()
  {
    Global = new() {
      GeneratesSingleObject = true,
      IgnoreCase = false,
      TotalPasses = 2,
      PropertyByDepth = true
    },
    EntityOptions = [
    new() {
      IndicatedItem = new() { Group = "key" },
      StoresData = true,
      Type = BT.Property,
      StorePieceTypes = new() { ["Name"] = "name" },
      SetPropKey = true
    }, new() {
      IndicatedItem = new() { Group = "strvalue" },
      StoresData = true,
      AddToPropKey = true,
      Type = BT.String,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      IndicatedItem = new() { Group = "boolvalue" },
      StoresData = true,
      Type = BT.Boolean,
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new () {
      IndicatedItem = new() { Group = "numvalue" },
      StoresData = true,
      Type = BT.Number,
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      IndicatedItem = new() { Group = "nullvalue" },
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
      Type = BT.Operator,
      ConstantValue = "}",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "[" },
      DepthChange = 1,
      AddToPropKey = true,
      Type = BT.Array,
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = ":" },
      Type = BT.Operator,
      ConstantValue = ":",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "," },
      Type = BT.Operator,
      ConstantValue = ",",
    }, new() {
      IndicatedItem = new() { Group = "Op", ExactValue = "]" },
      DepthChange = -1,
      Type = BT.Operator,
      ConstantValue = "]",
    }]
  };
}
