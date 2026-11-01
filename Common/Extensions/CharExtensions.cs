//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using static Common.Chars;

namespace Common.Extensions;

/// <summary>Extension methods for the <see cref="char"/> type.</summary>
public static class CharExtensions
{
  /// <summary>Char Extensions</summary>
  /// <param name="c">The character to check.</param>
  extension(char c)
  {
    public char Display => c switch
    {
      < SP => (char) ('␀' + c),
      DEL => '␡',
      _ => c
    };
    public bool IsAlphanumeric () =>
      c.IsLC || c.IsUC || c.In('0'..'9');
    /// <summary>Checks if this character is lowercase a thru z.</summary>
    /// <returns>true is the character is between ascii 'a' and 'z', false otherwise.</returns>
    public bool IsLC => c.In('a'..'z');
    public bool IsUC => c.In('A'..'Z');
    public bool IsCtrl => c is < ' ' or DEL;
    /// <summary>Checks if this character must be represented in 16 bits (256-65535).</summary>
    /// <returns><see langword="true"/> if it is within the specified range, otherwise <see langword="false"/>.</returns>
    public bool Is16bit => c.In(256, short.MaxValue);
    public bool IsNumber => c.IsInteger || c is '.';
    public bool IsInteger => c.IsPosInteger || c is '-';
    public bool IsPosInteger => c.In('0', '9');
    /// <summary>Checks if a character is considered whitespace by Unicode standards.</summary>
    /// <returns><see langword="true"/> if the character is whitespace, otherwise <see langword="false"/>.</returns>
    public bool IsWhitespace () =>
      c.In(9..13) || c.In(8192..8202) || c.Is(32, 133, 160, 5760, 8232, 8233, 8239, 8287, 12288);
    public bool Is (params int[] values) => values.Any(val => c == val);
    public bool In (int min, int max) => c >= min && c <= max;
    public bool In (Range range) => c >= range.Start.Value && c <= range.End.Value;
    public char ToLower () => c.IsUC ? (char) (c + ' ') : c;
    /// <summary>Gets the uppercase variant of this character, or the character, if there isn't one.</summary>
    /// <returns>The uppercase variant, or the character itself.</returns>
    public char ToUpper () => c.IsLC ? (char) (c - 32) : c;
  }
}
