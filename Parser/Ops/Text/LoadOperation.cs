//using Parser.Text.Tokens;

namespace Parser.Ops.Text;

/// <summary>Loads a path as text.</summary>
public class LoadOperation : Operation
{
  public bool IgnoreMissing { get; init; }
  public bool LoadBinary { get; init; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  private dynamic DoLoad (string file)
  {
    if (File.Exists(file))
    {
      return LoadBinary ? new Memory<byte>(File.ReadAllBytes(file)) : File.ReadAllText(file);
    }
    else if (IgnoreMissing)
    {
      return LoadBinary ? Memory<byte>.Empty : SE;
    }

    throw Err.ThrowBadResult($"File {file} was not found.");
  }

  protected override void Execute ()
  {
    if (Data[InputKey] is string str)
    {
      Data[OutputKey] = DoLoad(str);
      Status = OpStatus.Pass;
    }
    else if (Data[InputKey] is IEnumerable<string> list)
    {
      Data[OutputKey] = (Collection<string>) [.. list.Select(DoLoad)];
      Status = OpStatus.Pass;
    }
    else
    {
      throw Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);
    }
  }
}
