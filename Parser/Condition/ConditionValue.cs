namespace Parser.Condition;

public readonly struct ConditionValue (KeyOption type, string? data) : IValueNode, IEquatable<IValueNode>, IEquatable<ConditionValue>
{
  public string? Data { get; } = data;
  public KeyOption Type { get; } = type;
  public readonly bool IsOperator => Type > KeyOption.OpStart;
  public readonly int? IntValue => int.TryParse(Data, out int val) ? val : null;
  public readonly decimal? DecValue => decimal.TryParse(Data, out decimal val) ? val : null;
  public readonly string? StrValue => Type is KeyOption.Literal ? Data : null;
  public readonly dynamic? Value => Data;
  public override readonly string ToString () => $"{Type}: {Data ?? "<null>"}";

  public override bool Equals (object? obj) => obj is IValueNode cv && Equals(cv);
  public override int GetHashCode () => HashCode.Combine(Type, Data);
  public bool Equals (IValueNode? other) => Type == other?.Type && Value == other?.Value;
  bool IEquatable<ConditionValue>.Equals (ConditionValue other) => Equals(other);

  public static bool operator == (ConditionValue left, IValueNode right) => left.Equals(right);
  public static bool operator != (ConditionValue left, IValueNode right) => !(left == right);
}
