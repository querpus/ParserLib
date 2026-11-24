//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common;

public interface IOneOrMany<out T>
{
  T? Value { get; }
  IEnumerable<T> Values { get; }
  int Count { get; }
  bool IsSingle { get; }
  bool IsEmpty { get; }
  bool IsMany { get; }
}

