namespace Common;

public interface IHasInternalKey<TKey> where TKey : notnull
{
  TKey Key { get; }
}
