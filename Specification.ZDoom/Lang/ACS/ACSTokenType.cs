#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CA1720 // Identifier contains type name

using System;

namespace Specification.ZDoom.Lang.ACS;

public struct ACSTokenData (ACSStructureType structure, string token) : IEquatable<ACSTokenData>
{
  public ACSStructureType Structure { get; set; } = structure;
  public string Token { get; set; } = token;

  public override readonly int GetHashCode () => HashCode.Combine(Structure, Token);
  public override readonly bool Equals ([NotNullWhen(true)] object? obj) => obj is ACSTokenData atd && Equals(atd);
  public readonly bool Equals (ACSTokenData other) => Structure == other.Structure && Token.Like(other.Token);
  public static bool operator == (ACSTokenData left, ACSTokenData right) => left.Equals(right);
  public static bool operator != (ACSTokenData left, ACSTokenData right) => !(left == right);
  public override readonly string ToString () => $"{Structure}, {Token}";
}

public enum ACSStructureType
{
  Unknown,

  //Single Word Tokens
  Type,
  Keyword,
  Name,
  Literal, //Int, String, Char, Fixed,
  Op, // All Ops
  Preproc,

  //Token Groups
  Value,

  //Assembled Structures
  Expression,
  ParamDef,
  Statement,
  Block,
  FunctionCall,
  PreProcFull,

  //Top Level Structures
  Script,
  Function,
  VarDecl,
  ArrDecl,
  VarAssn,
  ArrAssn,
  MultiVarDecl,
  MapVarDecl,
  FuncCall,
  FuncCallStmt,
  Stmt,
  ExprBlock,
  Label,
  Switch,
  ForBlock,
  ElseBlock,
  DoBlock

}

public enum ACSTokenType
{
  None,

  // Data Types

  /// <summary>A string. (double quoted)</summary>
  /// <remarks>Ex. <c>"MapSpot"</c></remarks>
  String,
  /// <summary>A character. (single quoted)</summary>
  /// <remarks>Ex. <c>'c'</c></remarks>
  Char,
  /// <summary>Integer Value</summary>
  /// <remarks>Ex. <c>6388334</c></remarks>
  Int,
  /// <summary>A fixed point value, stored internally as an int.</summary>
  /// <remarks>Ex. <c>320.5</c></remarks>
  Fixed,

  // Name Tokens
  /// <summary>A name or identifier of some sort.</summary>
  Name,
  FuncDefName,
  FuncName,
  ParamName,
  VarName,
  ArrVarName,
  DefineName,

  FunctionCall,
  FunctionCallStmt,
  /// <summary>Keyword: script</summary>
  Script,
  /// <summary>Keyword: function</summary>
  Function,
  /// <summary>Group: <c>world</c> or <c>global</c></summary>
  MapVar,
  ScriptType,
  /// <summary>Placeholder representing any value, expression, or function call.</summary>
  Value,
  Bo,
  Bc,
  Po,
  Pc,
  /// <summary>Represents an assembly of an operator and one or more values.</summary>
  Expression,
  /// <summary>Semicolon</summary>
  Sc,
  /// <summary>Comma ','</summary>
  Cm,
  /// <summary>Group contruct: All operators satisfy this token.</summary>
  Op,
  /// <summary>Colon ':'</summary>
  Co,
  /// <summary>Array open '['</summary>
  Ao,
  /// <summary>Array close ']'</summary>
  Ac,
  Preprocessor,
  /// <summary>Operator: =</summary>
  Eq,
  /// <summary>Keyword: if</summary>
  If,
  /// <summary>Keyword: for</summary>
  For,
  /// <summary>Operator: ++ or --</summary>
  IncDec,
  /// <summary>Operator: !</summary>
  LogNot,
  /// <summary>Operator Group: Any Assignment Operator</summary>
  Assign,
  /// <summary>Operator: -</summary>
  Minus,
  Unary,
  Binary,
  /// <summary>Keyword: net</summary>
  Net,
  Loop,
  /// <summary>Keyword: do</summary>
  Do,
  /// <summary>Keyword: switch</summary>
  Switch,
  /// <summary>Keyword: case</summary>
  Case,
  /// <summary>Keyword: default</summary>
  Default,
  /// <summary>Keyword: return</summary>
  Return,
  SimpleJump,
  /// <summary>Keyword: void</summary>
  Void,
  /// <summary>Group construct: Any type keyword, such as <c>str</c>, <c>char</c>, <c>bool</c>, or <c>int</c>.</summary>
  Type,
  /// <summary>Group construct: Any latent function name.</summary>
  Wait,

  IfBlock,
  ElseBlock,
  ElseIfBlock,
  VarDecl,
  VarDeclAssn,
  ArrayDecl,
  VarAssn,
  /// <summary>Keyword: else</summary>
  Else,
  Stmt,
  BasicCmd,
  VarInc,
  ParamDef,
  ArrayValue,
  /// <summary>Assembled: An array dimension.</summary>
  /// <remarks><c>[43]</c> or <c>[x]</c></remarks>
  ArrayDim,
  Block,
  CaseLabel,
  ExpressionStandalone,
  ExprName,
  FinalParameter,
  FuncStmt,
  FunctionFull,
  FunctionHeader,
  Literal,
  LoopBlock,
  ParameterExpression,
  ParameterValue,
  Pre,
  PreProcName,
  PrintFunction,
  PrintParameterValue,
  ReturnStmt,
  ScriptCallStmt,
  ScriptCallWaitStmt,
  ScriptFull,
  ScriptFunc,
  ScriptHeader,
  SwitchBlock,
  ValueNoName,
  WaitStmt,
  ForBlock,
  ForHeader,
}
