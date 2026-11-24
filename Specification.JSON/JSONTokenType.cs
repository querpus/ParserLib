#pragma warning disable CA1720 // Identifier contains type name

namespace Specification.JSON;

public enum JSONTokenType
{
  None,
  Undef,
  Str,
  Num,
  Bool,
  Null,
  Cm,
  Ao,
  Ac,
  Co,
  Ws,
  Comment,
  Property,
  Object,
  Array,
  Value,
  Bo,
  Bc
}
