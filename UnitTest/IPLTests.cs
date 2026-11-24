using Specification.IPL;

namespace UnitTest;

public class IPLTests
{
  [Theory]
  [InlineData("<STX><ESC>P;<ESC>C;E3;F3;<ETX>\n<STX>H2;o200,399;c20;l2;w2;d3,string me;<ETX>")]
  public void IPL_ParseTest (string initial_string)
  {
    XParser parser = new();
    OpStatus status = parser.ParseData(Definition.Spec, initial_string);
    Assert.False(status.IsFail, "Status was a failure condition.");
    Assert.Contains("initial", parser.Data.Keys);
    Assert.Contains("text", parser.Data.Keys);
    Assert.Equal(10, (parser.Data["commands"] as IList<object>)?.Count);
    Assert.Equal("E", (parser.Data["commands"] as IList<CommandDataSet>)?[2].CmdLetter);
  }

  [Theory]
  [InlineData(@"ParserLib\Specification.IPL\Samples\6458 Batch.txt")]
  public void IPL_ParseTestFull (string label_file)
  {
    XParser parser = new();
    OpStatus status = parser.ParseFile(Definition.Spec, Helper.GitDir + label_file);
    Assert.True(status.IsPass);
    Assert.Contains("initial", parser.Data.Keys);
    Assert.Contains("text", parser.Data.Keys);
    Assert.Contains("matches", parser.Data.Keys);
    Assert.True(parser.Data.TryLoad("commands", out IEnumerable<CommandDataSet>? objects));
    Collection<CommandDataSet> objs = [.. objects];
    Assert.Equal("c", objs[0].CmdLetter);
    Assert.Equal("P", objs[1].CmdLetter);
    Assert.True(objs[0].IsEscaped);
  }

  [Theory]
  [InlineData("", true)]
  [InlineData(null, true)]
  public void IPL_ParseFailure (string? initial_string, bool pass)
  {
    XParser textParser = new();
    OpStatus status = textParser.ParseData(Definition.Spec, initial_string ?? "");
    Assert.Equal(pass, status.IsPass);
  }
}
