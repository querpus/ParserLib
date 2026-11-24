#pragma warning disable CA1720 // Identifier contains type name

namespace Specification.ZDoom.Lang.UDMF;

/// <summary>Token types for UDMF Spec.</summary>
public enum UDMFTokenType
{
  /// <summary>Undefined or unmatched token.</summary>
  None,

  /// <summary>Vertex keyword.</summary>
  Vertex,
  /// <summary>Thing keyword.</summary>
  Thing,
  /// <summary>Namespace keyword.</summary>
  Namespace,
  /// <summary>SideDef keyword.</summary>
  SideDef,
  /// <summary>LineDef keyword.</summary>
  LineDef,
  /// <summary>Sector keyword.</summary>
  Sector,

  /// <summary>Quoted text.</summary>
  String,
  /// <summary>Unquoted text.</summary>
  Name,
  /// <summary>Positive Integer.</summary>
  PInt,
  /// <summary>Any Integer.</summary>
  Int,
  /// <summary>Decimal, Float, or Fixed Point.</summary>
  Dec,
  /// <summary>Any data type.</summary>
  Value,
  /// <summary>True or False.</summary>
  Bool,

  /// <summary>Equals sign.</summary>
  Eq,
  /// <summary>Semicolon.</summary>
  Sc,
  /// <summary>Bracket open '{'.</summary>
  Bo,
  /// <summary>Bracket close '}'.</summary>
  Bc,
  /// <summary>An assembled property definition.</summary>
  Property,
  Op,
  /// <summary>An assembled map structure.</summary>
  Structure,
  Keyword,
  NamespaceDec
}

