using static Parser.Condition.KeyOption;

namespace Parser.Condition;

public enum OoOp
{
  None,
  Paren,
  Exp,
  Mult,
  Add,
  Keyword,
  Comparison,
  And,
  Or,
}

public static class KOExtension
{
  extension(KeyOption ko)
  {
    /// <summary>This item is an operator.</summary>
    public bool IsOperator => ko > OpStart;
    /// <summary>This item takes an object as input.</summary>
    public bool UsesObjectInput => ko.IsWithin(OpIs, OpSeqEq);
    public bool UsesLogicalInput => ko.IsWithin(OpAnd, OpOr) || ko is OpXOr;
    public bool UsesNumericInput => ko.IsWithin(OpEq, OpRoot);
    public bool IsConstant => ko is Null or Literal or True or False or Integer or KeyOption.Decimal;
    public OoOp OrderOfOperationsIndex => ko switch
    {
      OpIs or OpLike or OpSeqEq => OoOp.Comparison,
      OpAnd => OoOp.And,
      OpOr or OpXOr => OoOp.Or,
      OpEq or OpGt or OpLt or OpGteq or OpLteq or OpNotEq => OoOp.Comparison,
      OpAdd or OpSub => OoOp.Add,
      OpDiv or OpMul or OpMod => OoOp.Mult,
      OpExp or OpLBs or OpRBs or OpRoot => OoOp.Exp,
      CountOfKey or CheckKeyExists or LoadKey or TypeOfKey => OoOp.Keyword,
      Undefined or Null or Literal or True or False or Integer or KeyOption.Decimal or Embedded or OpStart => throw new InvalidOperationException("These cannot exist here."),
      _ => OoOp.None,
    };
  }
}

public enum KeyOption
{
  /// <summary>Undefined. This is the initial value.</summary>
  Undefined = -1, // Initial value
  /// <summary>The value 'null'.</summary>
  Null,           // null OR NULL OR NuLl
  LoadKey,        // [key_name]
  CountOfKey,     // countof[key_name]
  CheckKeyExists, // exists[key_name]
  TypeOfKey,      // typeof[key_name]

  Literal,        // {literal text}
  /// <summary>The boolean value 'true'.</summary>
  True,           // true OR TRUE OR TrUe
  False,          // false OR FALSE OR FaLsE
  Integer,        // integer
  Decimal,        // decimal
  Embedded,       // SubExpression

  /// <summary>Represents the start operation.</summary>
  OpStart = 100,  // Operator start marker

  // Object In & Logical Out
  OpIs,           // is (ordinal compare, or type compare.)
  OpLike,         // like (case insensitive compare)
  OpSeqEq,        // matches (Sequence Equals for collections)

  // Logical In & Out
  OpAnd,          // && (logical and)
  OpOr,           // || (logical or)

  // Numeric In & Logical Out
  OpEq,           // == (equality)
  OpGt,           // > (greater than)
  OpLt,           // < (less than)
  OpGteq,         // >=
  OpLteq,         // <=
  OpNotEq,        // != (not equal to)

  // Numeric In & Out
  OpAdd,          // + (add)
  OpSub,          // - (subtract)
  OpDiv,          // / (divide)
  OpMul,          // * Multiply
  OpExp,          // Math.Pow (Exponent)
  OpMod,          // % (Modulus)
  OpLBs,          // << (Left Bitshift)
  OpRBs,          // >> (Right Bitshift)
  OpRoot,         // root (Any Root, just negative exp)
  OpXOr,          // ^ (XOR)
}
