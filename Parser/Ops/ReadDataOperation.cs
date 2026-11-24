#pragma warning disable IDE0060 // Remove unused parameter

using static Parser.OpStatus;

namespace Parser.Ops;

public sealed class ReadDataOperation : Operation
{
  private bool IsBinary => Mode.Like(["binary", "bin"]);
  private bool IsText => Mode.Like(["text", "string", "str"]);
  private bool IsValue => Mode.Like(["int", "integer", "int32", "value", "short", "byte", "long", "int16", "int64"]);

  public string? CursorKey { get; init; }
  public string? OutputKey { get; init; }
  public string? LengthKey { get; init; }

  public int Length { get; set; } = -1;
  public required string Mode { get; init; }
  public int Position { get; set; } = -1;
  public string? PositionKey { get; init; }
  public string? ContentKey { get; init; }
  public string? Endianness { get; init; }
  public string? Encoding { get; init; }

  private Memory<byte> ReadBytes (int count)
  {
    if (PositionKey.IsNotEmpty)
    {
      Position = (int) Data[PositionKey];
    }

    if (CursorKey.IsNotEmpty)
    {
      CursorData cursor = Data.GetCursorByKey(CursorKey);
      Memory<byte> mem = (Memory<byte>) Data[cursor.ListKey];
      Memory<byte> slice = mem.Slice(cursor.Index, count);
      cursor.Index += count;
      return slice;
    }

    Memory<byte> mem2 = (Memory<byte>) Data[ContentKey];

    int max = (int) Data["file_size"];

    // Throw if Position over file max size
    if (Position > max)
      Err.ThrowBufferOver(Position, max);

    // Truncate if length overshoots but initial is valid.
    if (Position + count > max)
      count = max - Position;

    return mem2.Slice(Position, count);

  }
  private string ReadChars (int count) => ReadBytes(count).Span.ByteArrToString();
  protected override void Execute ()
  {
    Length = (int) Data[LengthKey, Length];

    if (Length == 0 && IsBinary)
    {
      Log(MsgClass.BlueInfo, "Found Marker", this);
    }

    if (Length == -1 && CursorKey is not null && (IsBinary || IsText))
    {
      Length = (int) Data["file_size"] - Data.GetCursorByKey(CursorKey).Index;
    }

    object? value = Length switch
    {
      0 when IsBinary => Memory<byte>.Empty,
      1 when IsValue => ReadBytes(Length).Span[0],
      2 when IsValue => ReadBytes(Length).Span.ToInt16(),
      4 when IsValue => ReadBytes(Length).Span.ToInt32(),
      8 when IsValue => ReadBytes(Length).Span.ToInt64(),
      > 0 when IsText => ReadChars(Length),
      > 0 when IsBinary => ReadBytes(Length),
      _ => Err.ThrowBadResult("Size was not valid")
    };

    Log(MsgClass.BlueInfo, $"Read: {value}", this);

    Data[OutputKey] = value;
    Status = Pass;
  }
}
