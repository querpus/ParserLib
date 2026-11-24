using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Parser;
using Parser.Exceptions;
using Parser.Tokens;

using Specification.ZDoom;

namespace ParserDebuggerApp;

public partial class MainForm : Form
{
  [AllowNull]
  private Spec _currentSpec;

  private readonly Dictionary<string, Color> _colorCache = [];

  internal IToken? SelectedToken;

  internal readonly Color SelectedBackColor = Color.Black;
  internal readonly Color SelectedForeColor = Color.White;

  public MainForm ()
  {
    InitializeComponent();
    Init();
  }

  private void Init ()
  {
    _ = cmbSpec.Items.Add("ZScript");
    _ = cmbSpec.Items.Add("ACS");
    _ = cmbSpec.Items.Add("UDMF");
    _ = cmbSpec.SelectedIndex = 0;

    btnRun.Click += (_, __) => RunEngine();
    treeHierarchy.AfterSelect += TreeHierarchy_AfterSelect;
  }

  // ---------------- SPEC ----------------

  private Spec GetSpec () => cmbSpec.SelectedItem?.ToString() switch
  {
    "ACS" => Definition.ACS,
    "UDMF" => Definition.UDMF,
    "ZScript" => Definition.ZScript,
    _ => throw new SpecNotDefinedException(),
  };

  // ---------------- ENGINE ----------------
  [AllowNull]
  private TokenCollection _lastTokens;
  [AllowNull]
  private TokenAssemblyResult _lastResult;

  private void RunEngine ()
  {
    try
    {
      _currentSpec = GetSpec();

      string text = rtbMain.Text;

      TokenFactory factory = new(_currentSpec);
      _lastTokens = factory.Produce(text);

      TokenAssembler assembler = new(_currentSpec);
      _lastResult = assembler.Execute(_lastTokens);

      DisplayTokens(_lastTokens);
      DisplayHierarchy(_lastResult);
      DisplayParents(_lastResult);
    }
    catch (Exception ex)
    {
      _ = MessageBox.Show(ex.ToString());
    }
  }

  // ---------------- TOKEN HIGHLIGHT ----------------

  private void DisplayTokens (TokenCollection tokens)
  {
    rtbMain.SelectAll();
    rtbMain.SelectionBackColor = Color.White;
    rtbMain.SelectionColor = Color.Black;

    foreach (IToken token in tokens)
    {
      rtbMain.Select(token.Index, token.Content.Length);

      Color baseColor = GetColor(token.Type);

      // Foreground color per type
      rtbMain.SelectionColor = baseColor;

      // Light background tint
      rtbMain.SelectionBackColor = Color.FromArgb(40, baseColor);
    }

    rtbMain.Select(0, 0);
  }

  private Color GetColor (string type)
  {
    if (!_colorCache.TryGetValue(type, out Color value))
    {
      Random rnd = new(type.GetHashCode());
      value = Color.FromArgb(
          255,
          50 + rnd.Next(180),
          50 + rnd.Next(180),
          50 + rnd.Next(180));
      _colorCache[type] = value;
    }

    return value;
  }

  // ---------------- TREE VIEW ----------------

  private void DisplayHierarchy (TokenAssemblyResult result)
  {
    treeHierarchy.Nodes.Clear();

    foreach (IToken token in result.Hierarchy)
    {
      _ = treeHierarchy.Nodes.Add(BuildNode(token));
    }
  }

  private static TreeNode BuildNode (IToken token)
  {
    string role = token.AssignTo?.ToString() ?? "-";

    TreeNode node = new($"{token.Type} ({role}) : {token.Content}")
    {
      Tag = token
    };

    foreach (IToken child in token.Children)
    {
      _ = node.Nodes.Add(BuildNode(child));
    }

    return node;
  }

  private void TreeHierarchy_AfterSelect (object? sender, TreeViewEventArgs e)
  {
    if (e.Node?.Tag is not IToken token)
      return;

    SelectedToken = token;

    RefreshHighlighting();
    HighlightRecursive(token);

    rtbMain.Select(token.Index, token.Content.Length);
    rtbMain.ScrollToCaret();
  }

  // ---------------- FOCUSED HIGHLIGHT ----------------

  private void HighlightRecursive (IToken token)
  {
    rtbMain.Select(token.Index, token.Content.Length);
    rtbMain.SelectionBackColor = SelectedBackColor;
    rtbMain.SelectionColor = SelectedForeColor;

    foreach (IToken child in token.Children)
    {
      HighlightRecursive(child);
    }
  }

  private void RefreshHighlighting ()
  {
    if (_lastTokens is null)
      return;

    DisplayTokens(_lastTokens);
  }

  // ---------------- PARENTS ----------------

  private void DisplayParents (TokenAssemblyResult result)
  {
    StringBuilder sb = new();

    foreach (IToken p in result.Parents)
    {
      _ = sb.AppendLine(p.Type);
    }

    rtbParents.Text = sb.ToString();
  }
}
