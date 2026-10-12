using Common.NodeTree;

namespace WPFParserVisualizer;

public record class CurrentFormat (NodeRuleSet NodeRuleSet, TokenRuleSet TokenRuleSet, CaptureTokenizer Tokenizer, TokenProcessor Processor);

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  public CurrentFormat Format { get; set; }
  public NodeRuleSet NodeRuleSet => Format.NodeRuleSet;
  public TokenRuleSet TokenRuleSet => Format.TokenRuleSet;
  public CaptureTokenizer Tokenizer => 


  public MainWindow () => InitializeComponent();

}
