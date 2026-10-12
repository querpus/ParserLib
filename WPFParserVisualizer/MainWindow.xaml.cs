using Common.NodeTree;

namespace WPFParserVisualizer;

public record class CurrentFormat (NodeRuleSet NodeRuleSet, ITokenizer<CaptureToken> Tokenizer);

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  public CurrentFormat? Format { get; set; }
  public NodeRuleSet? NodeRuleSet => Format?.NodeRuleSet;
  public ITokenizer<CaptureToken>? Tokenizer => Format?.Tokenizer;

  public MainWindow () => InitializeComponent();

  public void LoadXMLCaptureFormat () =>
    Format = new(StandardRuleSets.XMLRuleSet, new CaptureTokenizer { Rules = StandardRuleSets.XMLTokenRuleSet });
}
