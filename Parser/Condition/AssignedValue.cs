namespace Parser.Condition;

public readonly struct AssignedValue : IValueNode, IEquatable<IValueNode>, IEquatable<AssignedValue>
{
  public readonly int? IntValue => Value is int i ? i : null;
  public readonly decimal? DecValue => Value is decimal d ? d : null;
  public readonly string? StrValue => Value is string s ? s : null;
  public KeyOption Type { get; init; }
  public dynamic? Value { get; init; }
  public override readonly string ToString () => $"{Value}";

  public override bool Equals (object? obj) => obj is IValueNode av && Equals(av);

  public override int GetHashCode () => HashCode.Combine(Type, Value);
  public bool Equals (IValueNode? other) => Type == other?.Type && Value == other?.Value;
  bool IEquatable<AssignedValue>.Equals (AssignedValue other) => Equals(other);

  public static bool operator == (AssignedValue left, AssignedValue right) => left.Equals(right);
  public static bool operator != (AssignedValue left, AssignedValue right) => !(left == right);
}
