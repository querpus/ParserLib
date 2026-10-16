//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Common.RegExp;

using SysRegex = System.Text.RegularExpressions.Regex;

namespace Common.Extensions;

/// <summary>Extensions for string objects.</summary>
public static class StringExtensions
{
  /// <summary>String Extensions</summary>
  /// <param name="text">The input string.</param>
  extension(string? text)
  {
    /// <summary>Shorthand for case sensitive ordinal comparison.</summary>
    /// <param name="other">The text to compare to.</param>
    /// <returns><see langword="true"/> if the values match, otherwise <see langword="false"/></returns>
    public bool Is (string? other) => (text.IsEmpty && other.IsEmpty) || (text?.Equals(other, SCO) ?? false);
    public bool IsAny (IEnumerable<string> other) => (text.IsEmpty && other.IsEmpty) || text.Any(other, Equals);
    public string Remove (string regex, RegexOptions options = RegexOptions.None) => text is null ? SE : SysRegex.Replace(text, regex, SE, options);
    public string Remove (int changed_pos, int changed_length) => text is null ? SE : text.Replace(changed_pos, changed_length);
    public string RemoveChars (string chars) => new(text?.Where(item => !chars.Contains(item, SCO)).ToArray());
    public string RemoveAllButChars (string chars) => new(text?.Where(item => chars.Contains(item, SCO)).ToArray());
    public string RemoveAll (string regex, RegexOptions options = RegexOptions.None)
    {
      string result = text ?? string.Empty;

      while (SysRegex.IsMatch(result, regex))
        result = result.Remove(regex, options);

      return result;
    }
    /// <summary>Removes all null characters from the string.</summary>
    /// <returns>The input string with all null characters removed.</returns>
    public string RemoveNulls () => text?.Replace("\0", "", SCO) ?? string.Empty;
    public string Replace (int changed_pos, int changed_length, string replacement = EmptyString)
    {
      if (text is null)
        return string.Empty;

      ArgumentOutOfRangeException.ThrowIfNegative(changed_pos);
      ArgumentOutOfRangeException.ThrowIfNegative(changed_length);

      return text.PreReplace(changed_pos) + replacement + text.PostReplace(changed_pos, changed_length);
    }
    public string Replace (IEnumerable<string> lfs, IEnumerable<string> rws)
    {
      Collection<string> lfsc = [.. lfs];
      Collection<string> rwsc = [.. rws];

      if (text is null)
        return string.Empty;

      if (lfsc.Count != rwsc.Count)
        throw new ArgumentException("lfs and rws must be equal in size.", nameof(rws));

      for (int i = 0; i < lfsc.Count; i++)
      {
        string lf = lfsc[i];
        string rw = rwsc[i];

        text = text.Replace(lf, rw, SCO);
      }
      return text;
    }
    public string Replace (ReplaceNode node, RegexOptions options = ROIPW) => node?.ReplaceRegex(text ?? SE, options) ?? SE;
  }

  extension(string s)
  {
    public bool IsAlphanumeric => s.All(item => item.IsAlphanumeric());
    public bool IsControl => s.All(item => item.IsCtrl);
    public bool IsWhitespace => s.All(item => item.IsWhitespace());
    public bool IsNumber => decimal.TryParse(s, out decimal _);
    public int FirstIndexOfAny (IEnumerable<string> checkFor, int startAt, StringComparison sc, out int found_len)
    {
      found_len = DNE;

      if (s.IsEmpty || checkFor?.IsEmpty != false)
        return DNE;

      int index = DNE;

      foreach (string item in checkFor)
      {
        if (item.IsEmpty)
          continue;

        int cur = s.IndexOf(item, startAt, sc);

        if (index == DNE || (cur < index && cur != DNE))
        {
          index = cur;
          found_len = item.Length;
        }
      }

      return index;
    }
    public int ContainsCount (string checkFor, StringComparison sc = SCO)
    {
      if (s is null || checkFor is null || s.Length == 0 || checkFor.Length == 0)
        return 0;

      int count = 0;
      int pos = DNE;
      do
      {
        pos = s.IndexOf(checkFor, pos == DNE ? 0 : pos, sc);
        if (pos != DNE)
        {
          count++;
          pos++;
        }
      } while (pos != DNE);
      return count;
    }
    public int ContainsCount (IList<string> checkForAny, StringComparison sc = SCO)
    {
      if (s is null || checkForAny is null || s.Length == 0 || checkForAny.Count == 0)
        return 0;

      int count = 0;
      int pos = DNE;
      do
      {
        pos = s.FirstIndexOfAny(checkForAny, pos == DNE ? 0 : pos, sc, out int found_len);
        if (pos != DNE)
        {
          count++;
          pos += found_len;
        }
      } while (pos != DNE);
      return count;
    }
    public int? ToInt () => int.TryParse(s, out int value) ? value : null;
    public decimal? ToDecimal () => decimal.TryParse(s, out decimal value) ? value : null;
    public bool? ToBool () => bool.TryParse(s, out bool value) ? value : null;
    public bool ValidateWithRegex ([SS("regex")] string validation_expression, RegexOptions options = RegexOptions.None)
    {
      MatchCollection matches = SysRegex.Matches(s, validation_expression, options);
      return matches.Count == 1 && matches[0].Length == s?.Length;
    }
    public DateTime? ToDateTime () => DateTime.TryParse(s, out DateTime value) ? value : null;
    public TimeSpan? ToTimeSpan () => TimeSpan.TryParse(s, out TimeSpan value) ? value : null;
    public Collection<string> Expand () => [.. s.Select(item => item.ToString())];
    public string Remove (Match match) => s is null ? SE : match is null ? s : s.Remove(match.Index, match.Length);

    public string Replace (Match match, string replaceWith)
    {
      if (s is null)
        return SE;

      if (match is null || match.Length == 0)
        return s;

      s = s.Remove(match.Index, match.Length);
      s = s.Insert(match.Index, replaceWith);
      return s;
    }
    public string Replace (IEnumerable<string> lookFor, string replaceWith)
    {
      if (s is null)
        return SE;
      if (lookFor is null)
        return s;
      foreach (string lf in lookFor)
        s = s.Replace(lf, replaceWith, SCO);
      return s;
    }
    public string XMLEscape ()
    {
      s ??= SE;
      s = s.Replace("&", "&amp;", SCO);
      s = s.Replace("'", "&apos;", SCO);
      s = s.Replace("\"", "&quot;", SCO);
      s = s.Replace("<", "&lt;", SCO);
      s = s.Replace(">", "&gt;", SCO);
      return s;
    }
    public string ReplaceIfContainsGroup (Match match, string group, string replaceWith) =>
      match?.Groups.ContainsKey(group) == true ? s.Replace(match, replaceWith) : s;
    public string ReplaceAllIfContainsGroup (MatchCollection matches, string group, string replaceWith)
    {
      if (matches is null)
        return s;
      foreach (Match m in matches)
        s = s.ReplaceIfContainsGroup(m, group, replaceWith);
      return s;
    }
    public bool Equals (char c) => s?.Length == 1 && c == s[0];
    public bool EqualsAny (StringComparison sc = SCO, params Collection<string> list) =>
      list.Any(s2 => s.Equals(s2, sc));
    public bool EqualsAll (StringComparison sc = SCO, params Collection<string> list) =>
      list.All(s2 => s.Equals(s2, sc));
    public bool StartsWithAny (IEnumerable<string> list, StringComparison sc = SCO) =>
      list.Any(s2 => s.StartsWith(s2, sc));
    public bool StartsWithAny (StringComparison sc = SCO, params IEnumerable<string> list) =>
      list.Any(s2 => s.StartsWith(s2, sc));
    public bool EndsWithAny (IEnumerable<string> list, StringComparison sc = SCO) =>
      list.Any(s2 => s.EndsWith(s2, sc));
    public bool EndsWithAny (StringComparison sc = SCO, params IEnumerable<string> list) =>
      list.Any(s2 => s.EndsWith(s2, sc));
  }
  /// <summary>String Extensions</summary>
  /// <param name="text">The string to check.</param>
  extension(string text)
  {
    /// <summary>Checks if this string is a positive integer.</summary>
    /// <returns><see langword="true"/> if the <see langword="string"/> is a positive integer, <see langword="false"/> otherwise.</returns>
    public bool IsPosInteger => int.TryParse(text, out int i) && i >= 0;
    public bool IsNamedGroup => !text.IsPosInteger;

    public string ReplaceRange (int start, int length, char rep = '\0')
    {
      if (text is null)
        return SE;

      char[] array = text.ToCharArray();

      if (start + length > array.Length)
        throw new ArgumentOutOfRangeException(nameof(start));

      for (int i = start; i < length; i++)
        array[i] = rep;

      return new(array);
    }

    public string PreReplace (int changed_pos) => text[..(changed_pos - 1)];
    public string PostReplace (int changed_pos, int replace_length) => text[(changed_pos + replace_length)..];
    public string RecursiveReplace (string lf, string rw, StringComparison options = SCO)
    {
      string temp = text ?? "";

      while (temp.Contains(lf, options))
        temp = temp.Replace(lf, rw, options);

      return temp;
    }
    public (int Line, int Col) Get2DPosition (int position)
    {
      if (text is null || position < 0)
        return (DNE, DNE);
      string before = text[..position];

      int lnst = before.LastIndexOfAny(['\n', '\r', '\v']);
      int lines = before.ContainsCount(Chars.NewLines, SCO);
      int col = before.Length - lnst;
      return (lines, col);
    }
    public int Get1DPosition (int line, int col)
    {
      if (string.IsNullOrEmpty(text) || line < 0 || col < 0)
        return DNE;
      string[] lines = text.Split('\n');
      string[] beforelns = lines[0..line];
      string flat = beforelns.TextJoin("\n");
      return flat.Length + col;
    }
  }
  /// <summary>string extensions</summary>
  /// <param name="text">The <see langword="string"/> to check.</param>
  extension([MaybeNullWhen(true), NotNullWhen(false)] string? text)
  {
    /// <summary>Checks if a <see langword="string"/> is <see langword="null"/> or empty.</summary>
    /// <returns><see langword="true"/> if the <see langword="string"/> is empty or is <see langword="null"/>, <see langword="false"/> otherwise.</returns>
    public bool IsEmpty => string.IsNullOrEmpty(text);
  }
  /// <summary>string extensions</summary>
  /// <param name="text">The <see langword="string"/> to check.</param>
  extension([NotNullWhen(true)] string? text)
  {
    /// <summary>Checks if a <see langword="string"/> is not <see langword="null"/> or empty.</summary>
    /// <returns><see langword="false"/> if the <see langword="string"/> is empty or <see langword="null"/>, <see langword="true"/> otherwise.</returns>
    public bool IsNotEmpty => !text.IsEmpty;

    /// <summary>Shorthand for case insensitive ordinal comparison.</summary>
    /// <param name="text">This text.</param>
    /// <param name="other">The other text.</param>
    /// <returns><see langword="true"/> if the values match, otherwise <see langword="false"/>.<br/>
    /// If either <paramref name="other"/> or <paramref name="text"/> are <see langword="null"/>, this method will return <see langword="false"/>.</returns>
    public bool Like ([NotNullWhen(true)] string? other) => text?.Equals(other, SCOIC) ?? false;
    public bool Like (IEnumerable<string> other) => (text.IsEmpty && other.IsEmpty) || (text?.Any(other, Like) ?? false);
    public bool Any (IEnumerable<string>? other, Func<string, string?, bool> func) => other?.Any(item => func(item, text)) ?? false;
  }
  /// <summary>string extensions</summary>
  /// <param name="text">The <see langword="string"/> to check.</param>
  extension([NotNullWhen(true)] string text)
  {
    public bool ContainsAny (IEnumerable<string> other, StringComparison sc = SCO) => other.Any(x => text.Contains(x, sc));
  }
  /// <summary>string extensions</summary>
  /// <param name="s">The <see langword="string"/> to check.</param>
  extension([NotNullWhen(true)] string? s)
  {
    public bool ContainsNewLine () => s is not null && (s.Contains('\n', SCO) || s.Contains('\r', SCO));
  }
  /// <summary>string extensions</summary>
  /// <param name="lines">The <see langword="string"/>[] to check.</param>
  extension(string[] lines)
  {
    public int Get1DPosition (int line, int col)
    {
      if (lines is null || lines.Length == 0 || line < 0 || col < 0)
        return DNE;
      string[] beforelns = lines[0..line];
      string flat = beforelns.TextJoin("\n");
      return flat.Length + col;
    }
  }

  extension([NotNull] string? text)
  {
    public void ThrowIfNullOrEmpty ()
    {
      if (text.IsEmpty)
        throw new ANEx(nameof(text));
    }
  }

  extension(string data)
  {
    public T ToEnum<T> () where T : notnull => (T) Enum.Parse(typeof(T), data);
    public int ToEnum (Type type) => (int) Enum.Parse(type, data);
  }

  extension([SS("regex")] string rx)
  {
    public bool TryMatchAt (RegexOptions options, int index, string input, [NotNullWhen(true)] out MatchDataSet? match)
    {
      match = null;
      Match attempt = SysRegex.Match(input, rx, options);

      if (attempt.Success && attempt.Index == index)
      {
        match = attempt.ToMatchData();
        return true;
      }

      return false;
    }
  }
}
