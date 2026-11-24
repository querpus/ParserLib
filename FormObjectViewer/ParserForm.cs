#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1812 // Remove unused classes

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Common;
using Common.Extensions;

using Parser.Tokens;

using static Common.Names;

using XMLMaker = Specification.XML.XMLString;

namespace FormObjectViewer;

internal sealed partial class ParserForm : Form
{
  private Library Lib { get; }
  private bool ItemsChanged { get; set; }
  public BindingList<string> TokenRules { get; } = [
      "TokenMatch",
      "TokenExact",
      "SplitMatch",
      "SplitExact",
      "ErrorMatch",
      "TokenExtract",
      "StoreExtra",
      "StoreOther",
    ];
  private Spec LoadedSpec => SpecComboBox.SelectedIndex >= 0 ? Lib.Values.At(SpecComboBox.SelectedIndex) : Lib["unknown"];
  private readonly BindingList<TokenRule> _workingRules = [];
  private readonly BindingList<TokenRule> _workingGroupRules = [];
  private readonly BindingList<IToken> _currentTokens = [];
  [AllowNull]
  private SectionCollection _sections;
  private string _parseFile = "";
  private TokenFactory? _factory;

  public ParserForm ()
  {
    InitializeComponent();
    Lib = Library.InitializeLibrary(AppDomain.CurrentDomain);
  }

  private void UpdateCounts ()
  {
    TokenRuleCountLabel.Text = $"{_workingRules.Count}";
    GroupTokenRuleCountLabel.Text = $"{_workingGroupRules.Count}";
    TokenCountLabel.Text = $"{_currentTokens.Count}";
    SaveRuleButton.Enabled = _workingRules.Count > 0;
  }
  private void ParserForm_Load (object sender, EventArgs e)
  {
    TokenDataGrid.DataSource = _currentTokens;
    TokenRuleDataGrid.DataSource = _workingRules;
    TokenGroupRuleDataGrid.DataSource = _workingGroupRules;
    TokenRuleBindingSource.DataSource = _workingRules;
    LoadSpecList(this, e);
  }

  private void OpenParseFileDialog_FileOk (object sender, CancelEventArgs e)
  {
    ParsePathTextBox.Text = OpenParseFileDialog.FileName;
    ParsePathTextBox.SelectAll();
  }

  private void LoadSpecList (object sender, EventArgs e)
  {
    SpecComboBox.DataSource = (Collection<string>) [.. Lib.Values.Select(spec => spec.Name)];
    LoadParseFileButton.Enabled = true;
    LoadRulesButton.Enabled = true;
    LoadRulesFileButton.Enabled = true;
    ClearRulesButton.Enabled = true;
  }

  private void Exit (object sender, EventArgs e)
  {
    if (ItemsChanged)
    {
      DialogResult ask = MessageBox.Show(this, "Rules have been modified.", "Save changes?", MessageBoxButtons.YesNoCancel);
      if (ask == DialogResult.Yes)
      {
        SaveRuleDialog(ExitMenuItem, e);
      }
      else if (ask == DialogResult.No)
      {
        Close();
      }
      else if (ask == DialogResult.Cancel)
      {
        // Do Nothing.
      }
    }
  }

  private void LoadRules (object sender, EventArgs e)
  {

    foreach (TokenRule rule in LoadedSpec.TokenRules)
    {
      _workingRules.Add(rule);
    }
    TokenRuleDataGrid.Refresh();
    UpdateCounts();
  }
  private void LoadGroupRules (object sender, EventArgs e)
  {

    foreach (TokenRule rule in LoadedSpec.GroupTokenRules)
    {
      _workingGroupRules.Add(rule);
    }
    TokenGroupRuleDataGrid.Refresh();
    UpdateCounts();
  }
  private void SaveRuleDialog (object sender, EventArgs e)
  {
    DialogResult dialog = SaveRuleFileDialog.ShowDialog();
    if (dialog == DialogResult.OK)
    {
      SaveRule(SaveRuleFileDialog, new() { Path = SaveRuleFileDialog.FileName });
    }
  }

  private void OpenParseFile (object sender, EventArgs e)
  {
    DialogResult dialog = OpenParseFileDialog.ShowDialog();
    if (dialog == DialogResult.OK)
    {
      //TODO: Maybe need something here?
    }
  }

  private void ExecuteParse (object sender, EventArgs e)
  {
    _currentTokens.Clear();
    TokenDataGrid.Refresh();
    _parseFile = ParsePathTextBox.Text;

    if (!File.Exists(_parseFile))
    {
      _ = MessageBox.Show(this, "Parse file not found.", "Please correct any mistakes in the path name.");
      return;
    }
    string? contents;
    try
    {
      contents = File.ReadAllText(_parseFile);
    }
    catch (IOException)
    {
      _ = MessageBox.Show(this, "Parse file not loaded.", "Please investigate the access to this file.");
      return;
    }

    _factory = new(LoadedSpec, _workingRules);

    foreach (IToken token in _factory.Produce(contents))
    {
      _currentTokens.Add(token);
    }

    _sections = _factory.CannotMatch;
    ItemTabs.SelectTab(1);
    TokenDataGrid.Refresh();
    ClearTokensButton.Enabled = true;
    ShowUnparsedButton.Enabled = true;
  }

  private void TokenRuleDataGrid_RowValidated (object sender, DataGridViewCellEventArgs e) => UpdateCounts();
  private void TokenGroupRuleDataGrid_RowValidated (object sender, DataGridViewCellEventArgs e) => UpdateCounts();
  private void ClearTokens (object sender, EventArgs e)
  {
    _currentTokens.Clear();
    TokenDataGrid.Refresh();
    UpdateCounts();
  }

  private void ClearRules (object sender, EventArgs e)
  {
    _workingRules.Clear();
    TokenRuleDataGrid.Refresh();
    UpdateCounts();
  }

  private void ShowUnparsed (object sender, EventArgs e)
  {
    UnparsedViewer viewer = new()
    {
      Sections = _sections
    };
    _ = viewer.ShowDialog();
    viewer.Dispose();
  }

  private sealed class PathEventArgs : EventArgs
  {
    public required string Path { get; init; }
  }

  private void SaveRule (object sender, PathEventArgs args)
  {
    int counter = 0;
    XMLMaker maker = new();
    maker.AddElementOpen("RuleSet");
    maker.AddLineFeed();
    foreach (TokenRule rule in _workingRules)
    {
      maker.AddElementOpen("Rule", [("index", $"{counter++}")]);
      maker.AddLineFeed();
      maker.AddElementOpen("Type");
      maker.AddContent($"{rule.Type}");
      maker.CloseLastElement();
      maker.AddLineFeed();
      maker.AddElementOpen("TypeToAssign");
      maker.AddContent($"{rule.TypeToAssign}");
      maker.CloseLastElement();
      maker.AddLineFeed();
      maker.AddElementOpen("Data");
      maker.AddContent($"{rule.RuleStringData?.XMLEscape()}");
      maker.CloseLastElement();
      maker.AddLineFeed();
      maker.CloseLastElement();
      maker.AddLineFeed();
    }
    maker.CloseLastElement();

    string doc = maker.Serialize();

    try { File.WriteAllText(args.Path, doc); } catch (IOException) { return; }

    ItemsChanged = false;
    UpdateCounts();
  }

  private void TokenRuleDataGrid_DataError (object sender, DataGridViewDataErrorEventArgs e)
  {
    TokenRuleDataGrid[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Pink;
    ItemsChanged = true;
    UpdateCounts();
  }

  private void TokenRuleAddRow (object sender, EventArgs e)
  {
    _workingRules.Add(new() { TypeToAssign = "None", Type = TokenRuleType.None, RuleStringData = SE });
    TokenRuleDataGrid.Refresh();
    UpdateCounts();
  }

  private void TokenRuleDataGrid_RowEnter (object sender, DataGridViewCellEventArgs e)
  {
    if (e.RowIndex == -1)
      return;

    if (e.ColumnIndex != 0)
      return;

    RuleEditForm editForm;
    if (e.RowIndex >= _workingRules.Count)
    {
      _workingRules.Add(new() { TypeToAssign = "None", Type = TokenRuleType.None, RuleStringData = SE });
      editForm = new()
      {
        Original = _workingRules[^1],
        Spec = LoadedSpec,
      };
    }
    else
    {
      editForm = new()
      {
        Original = _workingRules[e.RowIndex],
        Spec = LoadedSpec,
      };
    }
    DialogResult result = editForm.ShowDialog();

    if (result is DialogResult.OK)
    {
      _workingRules[e.RowIndex].RuleStringData = editForm.Original.RuleStringData;
      _workingRules[e.RowIndex].Type = editForm.Original.Type;
      _workingRules[e.RowIndex].TypeToAssign = editForm.Original.TypeToAssign;
    }
    editForm.Dispose();
    TokenRuleDataGrid.Refresh();
    UpdateCounts();
  }
}
