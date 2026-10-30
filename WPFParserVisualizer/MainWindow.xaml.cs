using Common.Nodes;

using static Common.Names;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace WPFParserVisualizer;

public record class CurrentFormat (NodeRuleSet NodeRuleSet, ITokenizer<CaptureToken> Tokenizer);

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  public static Dictionary<string, CurrentFormat> ParseOptions { get; } = new()
  {
    ["XML"] = new(StandardRuleSets.XMLRuleSet, new CaptureTokenizer { Rules = StandardRuleSets.XMLTokenRuleSet }),
    ["IPL"] = new(StandardRuleSets.IPLRuleSet, new CaptureTokenizer { Rules = StandardRuleSets.IPLTokenRuleSet }),
    ["JSON"] = new(StandardRuleSets.JSONRuleSet, new CaptureTokenizer { Rules = StandardRuleSets.JSONTokenRuleSet }),
    //["ini"] = new(StandardRuleSets.INIRuleSet, new CaptureTokenizer { Rules = StandardRuleSets.INITokenRuleSet }),
  };
  public static Collection<string> NiceNames => [.. ParseOptions.Keys];

  public CurrentFormat? Format { get; set; }
  public string InputPath { get; set; } = SE;
  public string InputText { get; set; } = SE;
  public NodeRuleSet? NodeRuleSet => Format?.NodeRuleSet;
  public ITokenizer<CaptureToken>? Tokenizer => Format?.Tokenizer;
  public Collection<IToken> Tokens => Tokenizer is null ? [] : [.. Tokenizer.Tokens.Cast<IToken>()];

  public MainWindow () => InitializeComponent();

  private void DataGrid_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  {

  }

  public void QuitForm (object sender, RoutedEventArgs e) => Close();

  private void LoadFormatButton_Click (object sender, RoutedEventArgs e)
  {
    string option = FormatComboBox.Text;
    Format = ParseOptions[option];
  }

  private void LoadInputButton_Click (object sender, RoutedEventArgs e)
  {
    string option = InputComboBox.Text;
    InputPath = option;
    if (File.Exists(InputPath))
    {
      InputText = File.ReadAllText(InputPath);
    }

    if (Format is not null)
    {
      var tokens = Format.Tokenizer.Tokenize(InputText).Cast<IToken>();
      var proc = new TokenProcessor(tokens, Format.NodeRuleSet);
      (ProgramStatus.Items[0] as Label)?.Content = "Processing Tokens...";
      proc.Process();
    }
  }
}
