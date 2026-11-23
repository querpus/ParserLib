using Specification.INI;

namespace UnitTest;

public class INITests
{
  [Fact]
  public void INI_Constructor ()
  {
    INIDocument test = ["Section1"];
    test["Section1"].Set("key", "Section1_Value");
    Assert.Equal("Section1_Value", test["Section1"]["key"]);
    _ = Assert.Single(test["Section1"]);
  }

  [Theory]
  [InlineData(Helper.Default)]
  public void ParserInit (string file)
  {
    string file_text = File.ReadAllText(Helper.GitDir + file);
    XParser parser = new(Definition.Spec);
    parser.P
    Assert.True(parser.ParseData(Definition.Spec, file_text).IsPass);
    Assert.NotNull(parser.Result);
  }

  [Theory]
  [InlineData(Helper.Default)]
  public void ParserTypeInit (string file)
  {
    string file_text = File.ReadAllText(Helper.GitDir + file);
    XParser parser = new();
    Assert.True(parser.ParseData(Definition.Spec, file_text).IsPass);
    _ = Assert.IsType<INIDocument>(parser.Result);
  }

  [Theory]
  [InlineData(Helper.Default)]
  public void ParserTypeInit_Sections (string file)
  {
    string file_text = File.ReadAllText(Helper.GitDir + file);
    XParser parser = new();
    Assert.True(parser.ParseData(Definition.Spec, file_text).IsPass);
    INIDocument doc = Assert.IsType<INIDocument>(parser.Result);
    Assert.Equal(2, doc.Count);
  }
}
