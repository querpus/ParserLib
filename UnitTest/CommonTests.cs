using Common.Entities;
using Common.Extensions;

using static Common.Names;

namespace UnitTest;

public class CommonTests
{
  [Theory]
  [InlineData("\u0000\u0010\u0020")]
  public void CharDisplayTest (string data)
  {
    string result = SE;
    foreach (char c in data)
    {
      result += c.Display;
    }
    Assert.Equal("␀␐␠", result);
  }

  [Theory]
  [InlineData(new byte[] { 0, 16, 5, 0 }, 0x00100500, true)]
  [InlineData(new byte[] { 0, 16, 5, 0 }, 0x00051000, false)]
  public void SpanToInt (byte[] v, int value, bool big_endian) => Assert.Equal(value, v.ToInt32(big_endian));

  [Theory]
  [InlineData("ParserLib\\Specification.XML\\Samples\\operation.xml")]
  public void TestXML (string path)
  {
    string content = File.ReadAllText(Helper.GitDir + path);

    DocumentEntity result = EntityFactory.FromString(content, DefaultParsingSets.XML);

    Assert.NotNull(result);

    foreach (KeyValuePair<string, object?> item in result.DataValues)
    {
      if (item.Value is null)
        Assert.Fail("Null value stored in DataValues");
    }
  }

  [Fact]
  public void EntityDataTest ()
  {
    RawEntity entity = new();

    entity.AddToDataCollection("test", new object());

    Assert.NotNull(entity.DataValues["test"]);
  }
}
