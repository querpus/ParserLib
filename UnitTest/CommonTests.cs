using Common.Extensions;
using Common.Nodes;

namespace UnitTest;

public class CommonTests
{
  [Theory]
  [InlineData("\u0000\u0010\u0020")]
  public void CharDisplayTest (string data)
  {
    string result = data.ToString(true);
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

    var m_tokenizer = new MatchTokenizer() { Rules = StandardRuleSets.XMLTokenRuleSet };
    var c_tokenizer = new CaptureTokenizer() { Rules = StandardRuleSets.XMLTokenRuleSet };

    Assert.NotNull(m_tokenizer);
    Assert.NotNull(c_tokenizer);

    var m_tokens = m_tokenizer.Tokenize(content);
    var c_tokens = c_tokenizer.Tokenize(content);

    Assert.NotNull(m_tokens);
    Assert.NotNull(c_tokens);
  }
}
