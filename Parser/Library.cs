#pragma warning disable CA1710 // Rename Parser.Library to end in either 'Dictionary' or 'Collection'

using Parser.Inference;

namespace Parser;

public sealed class Library : IReadOnlyDictionary<string, Spec>, IPrintable, IReadOnlyCollection<Spec>
{
  private const string Area = "Library";
  private readonly Dictionary<string, Spec> _specs = [];
  /// <summary>The singleton instance of this object.</summary>
  private Dictionary<string, ReadOnlyCollection<InferenceNode>> SpecInferences =>
    [.. _specs.Select(
      item => new KeyValuePair<string, ReadOnlyCollection<InferenceNode>>(item.Key, item.Value.FileInferences)) ?? throw new InvalidOperationException("Library must be initialized.")];

  private Library ()
  {
    _specs.Add(DefaultSpec.SpecLoader.Name, DefaultSpec.SpecLoader);
    _specs.Add(DefaultSpec.TextByLines.Name, DefaultSpec.TextByLines);
    _specs.Add(DefaultSpec.Unknown.Name, DefaultSpec.Unknown);
  }
  public Spec this[string key] { get => _specs[key.ToLower(CIIC)]; set => _specs[key.ToLower(CIIC)] = value; }
  public Spec? Lookup (string? name) => name is not null && ContainsKey(name) ? _specs[name] : null;
  public Spec LookupOrDefault (string? name)
  {
    if (name is null)
    {
      Log(MsgClass.Error, "Tried to lookup name of null.", this);
      throw new SpecNotDefinedException("Tried to lookup name of null.");
    }
    return !TryLookup(name, out Spec? spec) ? XParser.Library["unknown"] : spec;
  }
  public bool TryLookup (string? name, [NotNullWhen(true)][MaybeNullWhen(false)] out Spec spec)
  {
    if (name is not null && ContainsKey(name))
    {
      spec = _specs[name];
      return true;
    }
    else
    {
      spec = null;
      return false;
    }
  }
  /// <summary>Initializes the library.</summary>
  /// <remarks>This must be called before the library is used.</remarks>
  /// <param name="domain">The domain that we are loading the <see cref="Spec"/> objects from.</param>
  public static Library InitializeLibrary (AppDomain domain)
  {
    DebugIn(Area, "InitializeLibrary");
    Library lib = new();
    XParser.Library = lib;
    domain.ThrowIfNull();

    List<Assembly> assemblies = [.. domain.GetAssemblies()];

    foreach (Assembly assembly in assemblies.OrderBy(static i => i.GetName().Name))
    {
      if (assembly.GetName().Name?.StartsWithAny(SCO, "System", "Microsoft", "Common") ?? false)
        continue;

      foreach (Type type in assembly.GetTypes())
      {
        if (type.GetCustomAttribute<DefinitionExportAttribute>() is null)
          continue;

        foreach (PropertyInfo? prop in type.GetProperties())
        {
          bool isSpec = prop.PropertyType.Name.Is(nameof(Spec));
          bool isMarked = prop.GetCustomAttribute<DefinitionExportAttribute>() is not null;
          if (isSpec && isMarked)
          {
            Spec? check = prop.GetValue(null) as Spec;
            check.ThrowIfNull();
            lib._specs[check.Name.ToLower(CIIC)] = check;
          }
        }
      }
    }

    foreach (string path in Directory.EnumerateFiles("Specs"))
    {
      string fpath = Path.GetFullPath(path);
      foreach (Spec spec in SpecInstructionParser.LoadSpecFile(fpath))
      {
        lib._specs[spec.Name.ToLower(CIIC)] = spec;
      }
    }

    Log(MsgClass.BlueInfo, $"{lib._specs.Count} Specs Loaded.", nameof(Library));
    DebugOut();
    return lib;
  }
  /// <summary>Provides the <see cref="Spec"/> for the provided file path.</summary>
  public string? CheckFile (string path)
  {
    foreach (KeyValuePair<string, ReadOnlyCollection<InferenceNode>> fi in SpecInferences)
    {
      foreach (InferenceNode node in fi.Value)
      {
        if (node.CheckFile(path))
        {
          Log(MsgClass.BlueInfo, $"File match found. Using {fi.Key} as Spec.", this);
          return fi.Key;
        }
      }
    }
    return null;
  }

  public void Add (string key, Spec value) => _specs.Add(key.ToLower(CIIC), value);
  public void AddRange (IEnumerable<KeyValuePair<string, Spec>> list) => _specs.AddRange(list);
  public bool ContainsKey (string key) => _specs.ContainsKey(key.ToLower(CIIC));
  public bool TryGetValue (string key, [MaybeNullWhen(false)] out Spec value) => _specs.TryGetValue(key.ToLower(CIIC), out value);
  public void Add (KeyValuePair<string, Spec> item) => _specs.Add(item.Key.ToLower(CIIC), item.Value);
  IEnumerator IEnumerable.GetEnumerator () => _specs.GetEnumerator();
  public void Print (int indent)
  {

  }

  public IEnumerator<Spec> GetEnumerator () => _specs.Values.GetEnumerator();
  IEnumerator<KeyValuePair<string, Spec>> IEnumerable<KeyValuePair<string, Spec>>.GetEnumerator () => _specs.GetEnumerator();

  IEnumerable<string> IReadOnlyDictionary<string, Spec>.Keys => _specs.Keys;
  public IEnumerable<Spec> Values => _specs.Values;
  public int Count => _specs.Count;
}
