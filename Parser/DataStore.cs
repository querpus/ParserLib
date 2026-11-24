namespace Parser;

/// <summary>
/// Predefined keys
/// <list type="bullet">
/// <item><c>initial:</c> The original file data.</item>
/// <item><c>result:</c> The end result.</item>
/// <item><c>text:</c> If the loaded file was text, then this is the text contents.</item>
/// <item><c>bytes:</c> If the loaded file was binary, then this is the raw content.</item>
/// <item><c>file_size:</c> The number of bytes or characters in the file.</item>
/// </list>
/// </summary>
/// <remarks>Any string may be used as a key.</remarks>
public sealed class DataStore
{
  private readonly Dictionary<string, object> _dict = [];
  /// <summary>A reference to the parser containting this data.</summary>
  public required XParser Parser { get; init; }
  /// <summary>Gets a value indicating whether the collection contains any elements.</summary>
  public bool HasData => Count > 0;
  /// <summary>Gets the number of elements contained in the collection.</summary>
  public int Count => _dict.Count;
  public ReadOnlyCollection<string> Keys => [.. _dict.Keys];
  /// <summary>Gets or sets data to a given key.</summary>
  /// <param name="key">The key to assign to or look up.
  /// Prefixing this string with a "+" when assigning will cause it to make a list instead of overrwriting.</param>
  /// <param name="throw_on_fail">Specifies whether to throw an exception on a missing key, or to return <see langword="null"/>.</param>
  /// <returns>The data assigned to the given key, or throws an exception if it is not found.</returns>
  /// <exception cref="ArgumentException"/>
  [DisallowNull]
  public object? this[string? key, bool throw_on_fail]
  {
    get => key is null ? null : throw_on_fail ? LoadOrFail(key) : LoadIfAble(key);
    set
    {
      if (throw_on_fail && key is null)
        key.ThrowIfNull();
      else if (key is null)
        return;

      _ = DoSave<object>(key, value, DM.Overwrite);
    }
  }

  [DisallowNull]
  [NotNull]
  public object this[string? key]
  {
    get => key is null ? null! : LoadOrFail(key);
    set
    {
      key.ThrowIfNull();
      _ = DoSave<object>(key, value, DM.Overwrite);
    }
  }

  [DisallowNull]
  [NotNull]
  public object this[string? key, dynamic value_if_not_found]
  {
    get => key is null ? value_if_not_found : LoadIfAble(key) ?? value_if_not_found;
    set
    {
      if (key is null)
        return;

      _ = DoSave<object>(key, value, DM.Overwrite);
    }
  }

  /// <summary>Loads the key, or throws an exception.</summary>
  /// <param name="key">The key to load.</param>
  /// <returns>The value of the key.</returns>
  /// <exception cref="OperationNoSuchVarException">The key is not present in the <see cref="DataStore"/>.</exception>
  internal object LoadOrFail (string key) => TryLoad(key, out object? value) ? value : Err.ThrowNoVar(key);
  internal T LoadOrFail<T> (string key) => TryLoad(key, out T? value) ? value : Err.ThrowNoVar(key);
  internal object? LoadIfAble (string key) => TryLoad(key, out object? value) ? value : null;
  internal T? LoadIfAble<T> (string key) => TryLoad(key, out T? value) ? value : default;

  public override string ToString ()
  {
    string alldata = "\n";

    foreach (KeyValuePair<string, object> kvp in _dict)
    {
      alldata += $"[{kvp.Key}] = {kvp.Value}\n";
    }

    return alldata;
  }

  /// <summary>The internal saving logic.</summary>
  /// <typeparam name="T">The data type to save.</typeparam>
  /// <param name="key">The key to save to.</param>
  /// <param name="data">The data to save.</param>
  /// <param name="mode">The <see cref="DM"/> to utilize.</param>
  /// <returns><see langword="true"/> if the save was successful, <see langword="false"/> otherwise.</returns>
  private bool DoSave<T> (string key, object data, DM mode)
  {
    if (data is null)
      return false;

    if (key.StartsWith('+', SCO))
    {
      key = key[1..];
      mode |= DM.MergeCollection | DM.AddToCollection | DM.MakeCollection;
    }

    if (mode.HasFlag(DM.MergeCollection) && TryLoadArray(key, out IEnumerable<T>? existing_to_merge) && data is IEnumerable<T> new_list)
    {
      _dict[key] = (Collection<T>) [.. existing_to_merge, .. new_list];
      return true;
    }
    else if (mode.HasFlag(DM.AddToCollection) && TryLoadArray(key, out IEnumerable<T>? existing_to_add) && data is T new_item)
    {
      _dict[key] = (Collection<T>) [.. existing_to_add, new_item];
      return true;
    }
    else if (mode.HasFlag(DM.MakeCollection) && TryLoad(key, out T? lonely_item) && data is T friend_item)
    {
      _dict[key] = new Collection<T>() { lonely_item, friend_item };
      return true;
    }
    else if (mode.HasFlag(DM.Overwrite) || mode == DM.None)
    {
      _dict[key] = data;
      return true;
    }
    else if (mode.HasFlag(DM.Ignore) && !CanLoad(key))
    {
      _dict[key] = data;
      return true;
    }
    else
    {
      return false;
    }
  }
  internal void Initialize<T> (T initial)
  {
    DebugIn("DataStore", "Initialize");
    initial.ThrowIfNull();
    Save("initial", initial);

    if (initial is string s)
    {
      Save("text", s);
      Save("file_size", s.Length);
    }
    else if (initial is IEnumerable<byte> bytes)
    {
      Memory<byte> list = bytes.ToArray().AsMemory();
      Save("bytes", list);
      Save("file_size", list.Length);
    }
    else if (initial is IEnumerable list)
    {
      Log(MsgClass.Warning, "Initialization of an unknown list.", this);
      Collection<object> coll = [.. list.OfType<object>()];
      Save("list", coll);
      Save("list_size", coll.Count);
    }
    DebugOut();
  }
  public bool CanLoad ([NotNullWhen(true)] string? key) =>
    key is not null && _dict.ContainsKey(key) && _dict[key] != null;
  public bool TryLoad ([NotNullWhen(true)] string key, [NotNullWhen(true)][MaybeNullWhen(false)] out object data) =>
    (CanLoad(key) ? data = _dict[key] : data = null) is not null;
  public bool TryLoadArray ([NotNullWhen(true)] string key, [NotNullWhen(true)][MaybeNullWhen(false)] out IEnumerable data)
  {
    data = CanLoad<IEnumerable>(key) ? this[key] as IEnumerable : null;
    return data is not null;
  }
  public void Save (string key, object data, DM mode = DM.Overwrite) => DoSave<object>(key, data, mode);
  public bool CanLoad<T> ([NotNullWhen(true)] string key) =>
    CanLoad(key) && _dict[key] is T;
  public bool TryLoad<T> ([NotNullWhen(true)] string key, [NotNullWhen(true)][MaybeNullWhen(false)] out T data)
  {
    data = CanLoad<T>(key) && _dict[key] is T casted ? casted : default;
    return data is not null;
  }
  public bool TryLoadArray<T> ([NotNullWhen(true)] string key, [NotNullWhen(true)][MaybeNullWhen(false)] out IEnumerable<T> data)
  {
    data = CanLoad<IEnumerable<T>>(key) ? this[key] as IEnumerable<T> : null;
    return data is not null;
  }
  public void Save<T> ([NotNull] string key, object data, DM mode = DM.Overwrite)
  {
    if (!DoSave<T>(key, data, mode))
      throw new OperationException("DATA NOT SAVED");
  }
  public int GetCountOfKey (string key) =>
    !CanLoad(key) ? 0 :
    _dict[key] is IEnumerable<object> list ? list.Count() :
    1;
  public bool TryGetCursorByKey (string key, out CursorData? cd) =>
    !CanLoad(key) ? throw Err.ThrowNoVar(key) :
    !TryLoad<CursorData>(key, out cd) ? throw Err.ThrowBadInput("", $"{_dict[key].GetType()}") : true;
  /// <summary>Gets the cursor that was created on the given key.</summary>
  /// <param name="key">The key of the cursor to retrieve.</param>
  /// <exception cref="OperationNoSuchVarException"/>
  /// <exception cref="OperationBadInputTypeException"/>
  public CursorData GetCursorByKey (string key) =>
    !CanLoad(key) ? throw Err.ThrowNoVar(key) :
    _dict[key] is CursorData cursor ? cursor :
    throw Err.ThrowBadInput(nameof(CursorData), $"{_dict[key].GetType()}");

  public void SetCursorIndex (string key, int index) => GetCursorByKey(key).Index = index;
  public void IncCursorIndex (string key, int inc) => GetCursorByKey(key).Index += inc;
  /// <summary>Checks if a cursor exists on a given key.</summary>
  /// <param name="key">The key to check.</param>
  /// <returns><see langword="true"/> if the cursor exists on <paramref name="key"/>, <see langword="false"/> otherwise.</returns>
  public bool HasCursorByKey (string key) => CanLoad<CursorData>(key);
  /// <summary>Removes the indicated key from storage.</summary>
  /// <param name="key">The key to clear.</param>
  /// <returns><see langword="true"/> if the key existed to be cleared, <see langword="false"/> otherwise.</returns>
  public bool Remove (string key) => _dict.Remove(key);
}
