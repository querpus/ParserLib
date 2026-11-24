using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ParserDebuggerApp
{
  partial class MainForm
  {
    private IContainer components = null;

    private SplitContainer splitMain;
    private RichTextBox rtbMain;
    private Button btnRun;
    private ComboBox cmbSpec;

    private TabControl tabRight;
    private TabPage tabHierarchy;
    private TabPage tabParents;

    private TreeView treeHierarchy;
    private RichTextBox rtbParents;

    protected override void Dispose (bool disposing)
    {
      if (disposing && components != null)
        components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent ()
    {
      this.components = new Container();

      this.Text = "Parser Debugger";
      this.ClientSize = new Size(1300, 800);

      splitMain = new SplitContainer();
      splitMain.Dock = DockStyle.Fill;
      splitMain.SplitterDistance = 700;

      // LEFT PANEL
      var leftPanel = new Panel { Dock = DockStyle.Fill };

      rtbMain = new RichTextBox
      {
        Dock = DockStyle.Fill,
        Font = new Font("Consolas", 10)
      };

      var topBar = new Panel { Dock = DockStyle.Top, Height = 35 };

      cmbSpec = new ComboBox
      {
        Dock = DockStyle.Fill,
        DropDownStyle = ComboBoxStyle.DropDownList
      };

      btnRun = new Button
      {
        Text = "Run",
        Dock = DockStyle.Right,
        Width = 80
      };

      topBar.Controls.Add(cmbSpec);
      topBar.Controls.Add(btnRun);

      leftPanel.Controls.Add(rtbMain);
      leftPanel.Controls.Add(topBar);

      splitMain.Panel1.Controls.Add(leftPanel);

      // RIGHT PANEL (Tabs)
      tabRight = new TabControl { Dock = DockStyle.Fill };

      tabHierarchy = new TabPage("Hierarchy");
      tabParents = new TabPage("Parents");

      treeHierarchy = new TreeView { Dock = DockStyle.Fill };

      rtbParents = new RichTextBox
      {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        Font = new Font("Consolas", 9)
      };

      tabHierarchy.Controls.Add(treeHierarchy);
      tabParents.Controls.Add(rtbParents);

      tabRight.TabPages.Add(tabHierarchy);
      tabRight.TabPages.Add(tabParents);

      splitMain.Panel2.Controls.Add(tabRight);

      this.Controls.Add(splitMain);
    }
  }
}
