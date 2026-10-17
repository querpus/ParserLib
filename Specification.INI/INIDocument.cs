using Parser.Tokens;

namespace Specification.INI;

/// <summary>A document, which is a collection of INISections.</summary>
public sealed class INIDocument : ISerializable, ICloneable, IEnumerable<INISection>
{
  #region Static Constructors
  /// <summary>Creates an <c>INIDocument</c> from another <c>INIDocument</c>.</summary>
  /// <param name="other">The specified document.</param>
  /// <returns>An <c>INIDocument</c> that is a deep clone of the specified object.</returns>
  public static INIDocument FromINIDocument (INIDocument other) => [.. other];
  /// <summary>Creates an <c>INIDocument</c> from an array or collection of <see cref="INISection"/> objects.</summary>
  /// <param name="sections">The array or collection of INISections.</param>
  /// <returns>The newly created document.</returns>
  public static INIDocument FromSections (IEnumerable<INISection> sections) => [.. sections];
  #endregion
  /// <summary>Creates an empty <see cref="INIDocument"/>.</summary>
  public INIDocument () { }
  /// <summary>Creates a <see cref="INIDocument"/> with the provided <see cref="INISection"/> objects in it.</summary>
  public INIDocument (IEnumerable<INISection> sections) => Sections = [.. sections];
  /// <summary>Creates a deep copy of the passed document.</summary>
  /// <param name="other">The document to copy.</param>
  public INIDocument (INIDocument other)
  {
    foreach (INISection s in other?.Sections ?? [])
    {
      INISection? ns = new(s.Name);
      Sections.Add(ns);
      foreach (IProperty<string> p in s)
      {
        ns.Add(new(p));
      }
    }
  }
  public INIDocument (IEnumerable<IToken> tokens)
  {
    foreach (IToken t in tokens)
    {
      if (t.Type.Like("Section"))
      {
        Sections.Add(new INISection(t));
        continue;
      }

      Debug.Log(MsgClass.Error, $"Incorrect Token Type {t.Type} must be Section.", this);
    }
  }

  /// <summary>The name of the document.</summary>
  public string Name { get; set; } = SE;
  /// <summary>A <see cref="Collection{T}">Collection&lt;INISection></see> object stored in this <see cref="INIDocument"/>.</summary>
  private Collection<INISection> Sections { get; init; } = [];
  /// <inheritdoc/>
  public int Count => Sections.Count;
  /// <summary>Gets the (nth) INISection.</summary>
  /// <param name="index">The index.</param>
  /// <returns>The INISection at that index.</returns>
  public INISection this[int index] => Sections[index];
  /// <summary>Gets the INISection named 'name'.</summary>
  /// <param name="name">The name to identtiry it,</param>
  /// <returns>The INISection starting with that name.</returns>
  public INISection this[string name] => Sections.First(item => item.Name.Like(name));
  /// <summary>Checks if the document contains a INISection with the provided name.</summary>
  /// <param name="name">The name to check for.</param>
  /// <returns><see langword="true"/> if the document contains a INISection with the given name, <see langword="false"/> otherwise.</returns>
  public bool Contains (string name) => Sections.Any(item => item.Name.Like(name));
  /// <summary>Adds a section if it does not exist.
  /// If it does exist, it adds or updates all the values contained in the provided <see cref="INISection"/>.</summary>
  /// <param name="section">The section to add.</param>
  public void Add (INISection section)
  {
    section.ThrowIfNull();
    if (Contains(section.Name))
    {
      foreach (IProperty<string> item in section)
      {
        if (item.Value is null)
          continue;
        this[section.Name].Set(item.Key, item.Value);
      }
    }
    else
    {
      Sections.Add(section);
    }
  }
  /// <summary>Adds a collection of child sections to the current document.</summary>
  /// <param name="children">An enumerable collection of <see cref="INISection"/> objects to add as children. Cannot be null.</param>
  public void AddRange (IEnumerable<INISection> children)
  {
    children.ThrowIfNull();
    foreach (INISection child in children)
    {
      Add(child);
    }
  }
  /// <summary>Adds a INISection with the specified name if it does not exist.</summary>
  /// <param name="iniSection">The name of the INISection to add.</param>
  public void Add (string iniSection)
  {
    iniSection.ThrowIfNullOrEmpty();
    if (!Contains(iniSection))
    {
      Sections.Add(new(iniSection));
    }
  }
  /// <summary>Merges 2 INISections, Favoring the other.</summary>
  /// <param name="other">The other document.</param>
  public void Update (INIDocument other)
  {
    other ??= [];
    foreach (INISection section in other.Sections)
    {
      Add(section);
    }
  }
  public string Serialize ()
  {
    string result = SE;
    foreach (INISection s in Sections)
    {
      result += s.Serialize() + "\n";

      foreach (IProperty<string> p in s)
      {
        PropertyObj obj = PropertyObj.From(p);
        result += obj.Serialize() + "\n";
      }
    }
    return result;
  }
  public override string? ToString () => Serialize();
  /// <summary>Clears and removes all INISections.</summary>
  public void Clear () => Sections.Clear();
  public object Clone () => FromINIDocument(this);
  #region Interface IEnumerable<INISection>
  public IEnumerator<INISection> GetEnumerator () => Sections.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  #endregion
}
