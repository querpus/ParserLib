using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FormObjectViewer;

internal partial class ParserForm : Form
{
  /// <summary> Required designer variable.</summary>
  private IContainer components = null;

  /// <summary> Clean up any resources being used.</summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose (bool disposing)
  {
    if (disposing && (components != null))
    {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  #region Windows Form Designer generated code

  /// <summary>
  ///  Required method for Designer support - do not modify
  ///  the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent ()
  {
    components = new Container();
    DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
    DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
    DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
    DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
    StatusStrip = new StatusStrip();
    ParseProgressBar = new ToolStripProgressBar();
    OpenParseFileDialog = new OpenFileDialog();
    SaveRuleFileDialog = new SaveFileDialog();
    TheMenuStrip = new MenuStrip();
    FileMenu = new ToolStripMenuItem();
    OpenFileMenuItem = new ToolStripMenuItem();
    ToolStripSeparator1 = new ToolStripSeparator();
    ExitMenuItem = new ToolStripMenuItem();
    SpecMenuItem = new ToolStripMenuItem();
    LoadSpecsMenuItem = new ToolStripMenuItem();
    TokensMenu = new ToolStripMenuItem();
    clearTokenTableToolStripMenuItem = new ToolStripMenuItem();
    parseTokensToolStripMenuItem = new ToolStripMenuItem();
    RuleMenuItem = new ToolStripMenuItem();
    loadFromSpecToolStripMenuItem = new ToolStripMenuItem();
    loadFromFilToolStripMenuItem = new ToolStripMenuItem();
    toolStripSeparator2 = new ToolStripSeparator();
    saveToFileToolStripMenuItem = new ToolStripMenuItem();
    clearRuleTableToolStripMenuItem = new ToolStripMenuItem();
    toolStripMenuItem1 = new ToolStripMenuItem();
    assemblyToolStripMenuItem = new ToolStripMenuItem();
    loadFromSpecToolStripMenuItem1 = new ToolStripMenuItem();
    SpecComboBox = new ComboBox();
    SpecBindingSource = new BindingSource(components);
    SpecLabel = new Label();
    TypeColumn = new DataGridViewComboBoxColumn();
    TypeToAssignColumn = new DataGridViewComboBoxColumn();
    RuleStringDataColumn = new DataGridViewTextBoxColumn();
    TokenRuleBindingSource = new BindingSource(components);
    ItemTabs = new TabControl();
    TokenRulesPage = new TabPage();
    RuleTableLabel = new Label();
    TokenRuleCountLabel = new Label();
    TokenRuleLabel = new Label();
    TokenRuleDataGrid = new DataGridView();
    EditTokenRuleButtonColumn = new DataGridViewButtonColumn();
    TypeTextColumn = new DataGridViewTextBoxColumn();
    AssignTypeColumnText = new DataGridViewTextBoxColumn();
    RuleDataColumnText = new DataGridViewTextBoxColumn();
    ClearRulesButton = new Button();
    SaveRuleButton = new Button();
    LoadRulesFileButton = new Button();
    LoadRulesButton = new Button();
    TokenListPage = new TabPage();
    ClearTokensButton = new Button();
    TokenTableLabel = new Label();
    TokenLabel = new Label();
    TokenCountLabel = new Label();
    TokenDataGrid = new DataGridView();
    TypeTokenColumn = new DataGridViewTextBoxColumn();
    ContentTokenColumn = new DataGridViewTextBoxColumn();
    ChildrenTokenColumn = new DataGridViewLinkColumn();
    TokenBindingSource = new BindingSource(components);
    ShowUnparsedButton = new Button();
    AssemblerPage = new TabPage();
    button3 = new Button();
    GroupTokenRuleCountLabel = new Label();
    label3 = new Label();
    button2 = new Button();
    button1 = new Button();
    label1 = new Label();
    TokenGroupRuleDataGrid = new DataGridView();
    dataGridViewButtonColumn1 = new DataGridViewButtonColumn();
    dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
    dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
    dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
    LoadParseFileButton = new Button();
    ParsePathLabel = new Label();
    ParsePathTextBox = new TextBox();
    ParseButton = new Button();
    StatusStrip.SuspendLayout();
    TheMenuStrip.SuspendLayout();
    ((ISupportInitialize) SpecBindingSource).BeginInit();
    ((ISupportInitialize) TokenRuleBindingSource).BeginInit();
    ItemTabs.SuspendLayout();
    TokenRulesPage.SuspendLayout();
    ((ISupportInitialize) TokenRuleDataGrid).BeginInit();
    TokenListPage.SuspendLayout();
    ((ISupportInitialize) TokenDataGrid).BeginInit();
    ((ISupportInitialize) TokenBindingSource).BeginInit();
    AssemblerPage.SuspendLayout();
    ((ISupportInitialize) TokenGroupRuleDataGrid).BeginInit();
    SuspendLayout();
    // 
    // StatusStrip
    // 
    StatusStrip.ImageScalingSize = new Size(20, 20);
    StatusStrip.Items.AddRange(new ToolStripItem[] { ParseProgressBar });
    StatusStrip.Location = new Point(0, 391);
    StatusStrip.Name = "StatusStrip";
    StatusStrip.Padding = new Padding(1, 0, 12, 0);
    StatusStrip.Size = new Size(1181, 22);
    StatusStrip.TabIndex = 0;
    StatusStrip.Text = "StatusStrip";
    // 
    // ParseProgressBar
    // 
    ParseProgressBar.Name = "ParseProgressBar";
    ParseProgressBar.Size = new Size(100, 16);
    // 
    // OpenParseFileDialog
    // 
    OpenParseFileDialog.InitialDirectory = "C:\\users\\johntay4\\source\\repos\\git";
    OpenParseFileDialog.ShowPreview = true;
    OpenParseFileDialog.ShowReadOnly = true;
    OpenParseFileDialog.SupportMultiDottedExtensions = true;
    OpenParseFileDialog.Title = "Parser File Selection";
    OpenParseFileDialog.FileOk += OpenParseFileDialog_FileOk;
    // 
    // SaveRuleFileDialog
    // 
    SaveRuleFileDialog.DefaultExt = "xml";
    SaveRuleFileDialog.FileName = "Rule.xml";
    SaveRuleFileDialog.Filter = "XML Files|*.xml";
    SaveRuleFileDialog.InitialDirectory = "%USERPROFILE%";
    SaveRuleFileDialog.Title = "Save Rule XML File";
    // 
    // TheMenuStrip
    // 
    TheMenuStrip.ImageScalingSize = new Size(20, 20);
    TheMenuStrip.Items.AddRange(new ToolStripItem[] { FileMenu, SpecMenuItem, TokensMenu, RuleMenuItem, assemblyToolStripMenuItem });
    TheMenuStrip.Location = new Point(0, 0);
    TheMenuStrip.Name = "TheMenuStrip";
    TheMenuStrip.Padding = new Padding(5, 2, 0, 2);
    TheMenuStrip.Size = new Size(1181, 24);
    TheMenuStrip.TabIndex = 2;
    TheMenuStrip.Text = "TheMenuStrip";
    // 
    // FileMenu
    // 
    FileMenu.DropDownItems.AddRange(new ToolStripItem[] { OpenFileMenuItem, ToolStripSeparator1, ExitMenuItem });
    FileMenu.Name = "FileMenu";
    FileMenu.Size = new Size(37, 20);
    FileMenu.Text = "File";
    // 
    // OpenFileMenuItem
    // 
    OpenFileMenuItem.Image = Properties.Resources.Open;
    OpenFileMenuItem.Name = "OpenFileMenuItem";
    OpenFileMenuItem.Size = new Size(184, 26);
    OpenFileMenuItem.Text = "Open File to Parse";
    OpenFileMenuItem.Click += OpenParseFile;
    // 
    // ToolStripSeparator1
    // 
    ToolStripSeparator1.Name = "ToolStripSeparator1";
    ToolStripSeparator1.Size = new Size(181, 6);
    // 
    // ExitMenuItem
    // 
    ExitMenuItem.Image = Properties.Resources.RigidRelationshipInfo;
    ExitMenuItem.Name = "ExitMenuItem";
    ExitMenuItem.Size = new Size(184, 26);
    ExitMenuItem.Text = "Exit";
    ExitMenuItem.Click += Exit;
    // 
    // SpecMenuItem
    // 
    SpecMenuItem.DropDownItems.AddRange(new ToolStripItem[] { LoadSpecsMenuItem });
    SpecMenuItem.Name = "SpecMenuItem";
    SpecMenuItem.Size = new Size(44, 20);
    SpecMenuItem.Text = "Spec";
    // 
    // LoadSpecsMenuItem
    // 
    LoadSpecsMenuItem.Image = Properties.Resources.Open;
    LoadSpecsMenuItem.Name = "LoadSpecsMenuItem";
    LoadSpecsMenuItem.Size = new Size(184, 26);
    LoadSpecsMenuItem.Text = "Load Specs";
    LoadSpecsMenuItem.Click += LoadSpecList;
    // 
    // TokensMenu
    // 
    TokensMenu.DropDownItems.AddRange(new ToolStripItem[] { clearTokenTableToolStripMenuItem, parseTokensToolStripMenuItem });
    TokensMenu.Name = "TokensMenu";
    TokensMenu.Size = new Size(56, 20);
    TokensMenu.Text = "Tokens";
    // 
    // clearTokenTableToolStripMenuItem
    // 
    clearTokenTableToolStripMenuItem.Image = Properties.Resources.CloseGroup;
    clearTokenTableToolStripMenuItem.Name = "clearTokenTableToolStripMenuItem";
    clearTokenTableToolStripMenuItem.Size = new Size(184, 26);
    clearTokenTableToolStripMenuItem.Text = "Clear Token Table";
    clearTokenTableToolStripMenuItem.Click += ClearTokens;
    // 
    // parseTokensToolStripMenuItem
    // 
    parseTokensToolStripMenuItem.Name = "parseTokensToolStripMenuItem";
    parseTokensToolStripMenuItem.Size = new Size(184, 26);
    parseTokensToolStripMenuItem.Text = "Parse Tokens";
    parseTokensToolStripMenuItem.Click += ExecuteParse;
    // 
    // RuleMenuItem
    // 
    RuleMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadFromSpecToolStripMenuItem, loadFromFilToolStripMenuItem, toolStripSeparator2, saveToFileToolStripMenuItem, clearRuleTableToolStripMenuItem, toolStripMenuItem1 });
    RuleMenuItem.Name = "RuleMenuItem";
    RuleMenuItem.Size = new Size(42, 20);
    RuleMenuItem.Text = "Rule";
    // 
    // loadFromSpecToolStripMenuItem
    // 
    loadFromSpecToolStripMenuItem.Image = Properties.Resources.Open;
    loadFromSpecToolStripMenuItem.Name = "loadFromSpecToolStripMenuItem";
    loadFromSpecToolStripMenuItem.Size = new Size(184, 26);
    loadFromSpecToolStripMenuItem.Text = "Load From Spec...";
    loadFromSpecToolStripMenuItem.Click += LoadRules;
    // 
    // loadFromFilToolStripMenuItem
    // 
    loadFromFilToolStripMenuItem.Image = Properties.Resources.Open;
    loadFromFilToolStripMenuItem.Name = "loadFromFilToolStripMenuItem";
    loadFromFilToolStripMenuItem.Size = new Size(184, 26);
    loadFromFilToolStripMenuItem.Text = "Load From File...";
    // 
    // toolStripSeparator2
    // 
    toolStripSeparator2.Name = "toolStripSeparator2";
    toolStripSeparator2.Size = new Size(181, 6);
    // 
    // saveToFileToolStripMenuItem
    // 
    saveToFileToolStripMenuItem.Image = Properties.Resources.Save;
    saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
    saveToFileToolStripMenuItem.Size = new Size(184, 26);
    saveToFileToolStripMenuItem.Text = "Save to File";
    saveToFileToolStripMenuItem.Click += SaveRuleDialog;
    // 
    // clearRuleTableToolStripMenuItem
    // 
    clearRuleTableToolStripMenuItem.Image = Properties.Resources.CloseGroup;
    clearRuleTableToolStripMenuItem.Name = "clearRuleTableToolStripMenuItem";
    clearRuleTableToolStripMenuItem.Size = new Size(184, 26);
    clearRuleTableToolStripMenuItem.Text = "Clear Rule Table";
    clearRuleTableToolStripMenuItem.Click += ClearRules;
    // 
    // toolStripMenuItem1
    // 
    toolStripMenuItem1.Image = Properties.Resources.AddMemeber;
    toolStripMenuItem1.Name = "toolStripMenuItem1";
    toolStripMenuItem1.Size = new Size(184, 26);
    toolStripMenuItem1.Text = "Add Token Rule";
    toolStripMenuItem1.Click += TokenRuleAddRow;
    // 
    // assemblyToolStripMenuItem
    // 
    assemblyToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadFromSpecToolStripMenuItem1 });
    assemblyToolStripMenuItem.Name = "assemblyToolStripMenuItem";
    assemblyToolStripMenuItem.Size = new Size(70, 20);
    assemblyToolStripMenuItem.Text = "Assembly";
    // 
    // loadFromSpecToolStripMenuItem1
    // 
    loadFromSpecToolStripMenuItem1.Image = Properties.Resources.Open;
    loadFromSpecToolStripMenuItem1.Name = "loadFromSpecToolStripMenuItem1";
    loadFromSpecToolStripMenuItem1.Size = new Size(184, 26);
    loadFromSpecToolStripMenuItem1.Text = "Load from Spec...";
    // 
    // SpecComboBox
    // 
    SpecComboBox.AccessibleRole = AccessibleRole.ComboBox;
    SpecComboBox.DataBindings.Add(new Binding("Text", SpecBindingSource, "Name", true));
    SpecComboBox.DataBindings.Add(new Binding("SelectedItem", SpecBindingSource, "Name", true));
    SpecComboBox.FormattingEnabled = true;
    SpecComboBox.Location = new Point(7, 42);
    SpecComboBox.Margin = new Padding(3, 2, 3, 2);
    SpecComboBox.Name = "SpecComboBox";
    SpecComboBox.Size = new Size(179, 23);
    SpecComboBox.TabIndex = 3;
    // 
    // SpecBindingSource
    // 
    SpecBindingSource.DataSource = typeof(Spec);
    // 
    // SpecLabel
    // 
    SpecLabel.AutoSize = true;
    SpecLabel.Location = new Point(7, 24);
    SpecLabel.Name = "SpecLabel";
    SpecLabel.Size = new Size(75, 15);
    SpecLabel.TabIndex = 4;
    SpecLabel.Text = "Specification";
    // 
    // TypeColumn
    // 
    TypeColumn.DataPropertyName = "Type";
    TypeColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
    TypeColumn.HeaderText = "Type";
    TypeColumn.MinimumWidth = 6;
    TypeColumn.Name = "TypeColumn";
    TypeColumn.Resizable = DataGridViewTriState.True;
    TypeColumn.Width = 125;
    // 
    // TypeToAssignColumn
    // 
    TypeToAssignColumn.DataPropertyName = "TypeToAssign";
    TypeToAssignColumn.HeaderText = "TypeToAssign";
    TypeToAssignColumn.MinimumWidth = 6;
    TypeToAssignColumn.Name = "TypeToAssignColumn";
    TypeToAssignColumn.Resizable = DataGridViewTriState.True;
    TypeToAssignColumn.SortMode = DataGridViewColumnSortMode.Automatic;
    TypeToAssignColumn.Width = 125;
    // 
    // RuleStringDataColumn
    // 
    RuleStringDataColumn.DataPropertyName = "RuleStringData";
    RuleStringDataColumn.HeaderText = "RuleStringData";
    RuleStringDataColumn.MinimumWidth = 6;
    RuleStringDataColumn.Name = "RuleStringDataColumn";
    RuleStringDataColumn.Width = 400;
    // 
    // TokenRuleBindingSource
    // 
    TokenRuleBindingSource.DataSource = typeof(Parser.Tokens.TokenRule);
    // 
    // ItemTabs
    // 
    ItemTabs.Controls.Add(TokenRulesPage);
    ItemTabs.Controls.Add(TokenListPage);
    ItemTabs.Controls.Add(AssemblerPage);
    ItemTabs.Location = new Point(192, 28);
    ItemTabs.Margin = new Padding(3, 2, 3, 2);
    ItemTabs.Name = "ItemTabs";
    ItemTabs.SelectedIndex = 0;
    ItemTabs.Size = new Size(964, 280);
    ItemTabs.TabIndex = 7;
    // 
    // TokenRulesPage
    // 
    TokenRulesPage.Controls.Add(RuleTableLabel);
    TokenRulesPage.Controls.Add(TokenRuleCountLabel);
    TokenRulesPage.Controls.Add(TokenRuleLabel);
    TokenRulesPage.Controls.Add(TokenRuleDataGrid);
    TokenRulesPage.Controls.Add(ClearRulesButton);
    TokenRulesPage.Controls.Add(SaveRuleButton);
    TokenRulesPage.Controls.Add(LoadRulesFileButton);
    TokenRulesPage.Controls.Add(LoadRulesButton);
    TokenRulesPage.Location = new Point(4, 24);
    TokenRulesPage.Margin = new Padding(3, 2, 3, 2);
    TokenRulesPage.Name = "TokenRulesPage";
    TokenRulesPage.Padding = new Padding(3, 2, 3, 2);
    TokenRulesPage.Size = new Size(956, 252);
    TokenRulesPage.TabIndex = 0;
    TokenRulesPage.Text = "Token Rules";
    // 
    // RuleTableLabel
    // 
    RuleTableLabel.BackColor = Color.Transparent;
    RuleTableLabel.Font = new Font("Arial Rounded MT Bold", 15.75F, FontStyle.Regular, GraphicsUnit.Point,  0);
    RuleTableLabel.Location = new Point(0, 0);
    RuleTableLabel.Margin = new Padding(2, 0, 2, 0);
    RuleTableLabel.Name = "RuleTableLabel";
    RuleTableLabel.Size = new Size(122, 24);
    RuleTableLabel.TabIndex = 12;
    RuleTableLabel.Text = "Rule Table";
    // 
    // TokenRuleCountLabel
    // 
    TokenRuleCountLabel.Location = new Point(784, 7);
    TokenRuleCountLabel.Name = "TokenRuleCountLabel";
    TokenRuleCountLabel.Size = new Size(35, 14);
    TokenRuleCountLabel.TabIndex = 9;
    TokenRuleCountLabel.Text = "0";
    // 
    // TokenRuleLabel
    // 
    TokenRuleLabel.AutoSize = true;
    TokenRuleLabel.Location = new Point(704, 7);
    TokenRuleLabel.Name = "TokenRuleLabel";
    TokenRuleLabel.Size = new Size(70, 15);
    TokenRuleLabel.TabIndex = 9;
    TokenRuleLabel.Text = "Token Rules";
    // 
    // TokenRuleDataGrid
    // 
    TokenRuleDataGrid.AllowUserToOrderColumns = true;
    TokenRuleDataGrid.AutoGenerateColumns = false;
    TokenRuleDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    TokenRuleDataGrid.Columns.AddRange(new DataGridViewColumn[] { EditTokenRuleButtonColumn, TypeTextColumn, AssignTypeColumnText, RuleDataColumnText });
    TokenRuleDataGrid.DataSource = TokenRuleBindingSource;
    TokenRuleDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
    TokenRuleDataGrid.GridColor = SystemColors.MenuText;
    TokenRuleDataGrid.Location = new Point(8, 28);
    TokenRuleDataGrid.Margin = new Padding(3, 2, 3, 2);
    TokenRuleDataGrid.MultiSelect = false;
    TokenRuleDataGrid.Name = "TokenRuleDataGrid";
    TokenRuleDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
    TokenRuleDataGrid.RowHeadersVisible = false;
    TokenRuleDataGrid.RowHeadersWidth = 51;
    dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
    dataGridViewCellStyle2.Font = new Font("Cascadia Code", 9.75F, FontStyle.Regular, GraphicsUnit.Point,  0);
    dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
    TokenRuleDataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
    TokenRuleDataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
    TokenRuleDataGrid.Size = new Size(816, 219);
    TokenRuleDataGrid.TabIndex = 7;
    TokenRuleDataGrid.CellClick += TokenRuleDataGrid_RowEnter;
    TokenRuleDataGrid.RowValidated += TokenRuleDataGrid_RowValidated;
    // 
    // EditTokenRuleButtonColumn
    // 
    EditTokenRuleButtonColumn.HeaderText = "Edit";
    EditTokenRuleButtonColumn.Name = "EditTokenRuleButtonColumn";
    EditTokenRuleButtonColumn.Text = "Edit";
    EditTokenRuleButtonColumn.Width = 50;
    // 
    // TypeTextColumn
    // 
    TypeTextColumn.DataPropertyName = "Type";
    dataGridViewCellStyle1.NullValue = "None";
    TypeTextColumn.DefaultCellStyle = dataGridViewCellStyle1;
    TypeTextColumn.HeaderText = "Type";
    TypeTextColumn.MinimumWidth = 6;
    TypeTextColumn.Name = "TypeTextColumn";
    TypeTextColumn.Resizable = DataGridViewTriState.True;
    TypeTextColumn.Width = 125;
    // 
    // AssignTypeColumnText
    // 
    AssignTypeColumnText.DataPropertyName = "TypeToAssign";
    AssignTypeColumnText.HeaderText = "TypeToAssign";
    AssignTypeColumnText.MinimumWidth = 6;
    AssignTypeColumnText.Name = "AssignTypeColumnText";
    AssignTypeColumnText.Resizable = DataGridViewTriState.True;
    AssignTypeColumnText.Width = 125;
    // 
    // RuleDataColumnText
    // 
    RuleDataColumnText.DataPropertyName = "RuleStringData";
    RuleDataColumnText.HeaderText = "RuleStringData";
    RuleDataColumnText.MinimumWidth = 6;
    RuleDataColumnText.Name = "RuleDataColumnText";
    RuleDataColumnText.Width = 500;
    // 
    // ClearRulesButton
    // 
    ClearRulesButton.Enabled = false;
    ClearRulesButton.FlatStyle = FlatStyle.System;
    ClearRulesButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    ClearRulesButton.Location = new Point(840, 219);
    ClearRulesButton.Margin = new Padding(3, 2, 3, 2);
    ClearRulesButton.Name = "ClearRulesButton";
    ClearRulesButton.Size = new Size(96, 28);
    ClearRulesButton.TabIndex = 8;
    ClearRulesButton.Text = "Clear Rule Table";
    ClearRulesButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    ClearRulesButton.UseVisualStyleBackColor = true;
    ClearRulesButton.Click += ClearRules;
    // 
    // SaveRuleButton
    // 
    SaveRuleButton.Enabled = false;
    SaveRuleButton.FlatStyle = FlatStyle.System;
    SaveRuleButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    SaveRuleButton.Location = new Point(840, 148);
    SaveRuleButton.Margin = new Padding(3, 2, 3, 2);
    SaveRuleButton.Name = "SaveRuleButton";
    SaveRuleButton.Size = new Size(96, 40);
    SaveRuleButton.TabIndex = 8;
    SaveRuleButton.Text = "Save Ruleset to File";
    SaveRuleButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    SaveRuleButton.UseVisualStyleBackColor = true;
    SaveRuleButton.Click += SaveRuleDialog;
    // 
    // LoadRulesFileButton
    // 
    LoadRulesFileButton.Enabled = false;
    LoadRulesFileButton.FlatStyle = FlatStyle.System;
    LoadRulesFileButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    LoadRulesFileButton.Location = new Point(840, 28);
    LoadRulesFileButton.Margin = new Padding(3, 2, 3, 2);
    LoadRulesFileButton.Name = "LoadRulesFileButton";
    LoadRulesFileButton.Size = new Size(96, 35);
    LoadRulesFileButton.TabIndex = 8;
    LoadRulesFileButton.Text = "Load Rules From File";
    LoadRulesFileButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    LoadRulesFileButton.UseVisualStyleBackColor = true;
    // 
    // LoadRulesButton
    // 
    LoadRulesButton.Enabled = false;
    LoadRulesButton.FlatStyle = FlatStyle.System;
    LoadRulesButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    LoadRulesButton.Location = new Point(840, 71);
    LoadRulesButton.Margin = new Padding(3, 2, 3, 2);
    LoadRulesButton.Name = "LoadRulesButton";
    LoadRulesButton.Size = new Size(96, 37);
    LoadRulesButton.TabIndex = 8;
    LoadRulesButton.Text = "Load Rules from Specification";
    LoadRulesButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    LoadRulesButton.UseVisualStyleBackColor = true;
    LoadRulesButton.Click += LoadRules;
    // 
    // TokenListPage
    // 
    TokenListPage.BackColor = SystemColors.Control;
    TokenListPage.Controls.Add(ClearTokensButton);
    TokenListPage.Controls.Add(TokenTableLabel);
    TokenListPage.Controls.Add(TokenLabel);
    TokenListPage.Controls.Add(TokenCountLabel);
    TokenListPage.Controls.Add(TokenDataGrid);
    TokenListPage.Controls.Add(ShowUnparsedButton);
    TokenListPage.Location = new Point(4, 24);
    TokenListPage.Name = "TokenListPage";
    TokenListPage.Padding = new Padding(3);
    TokenListPage.Size = new Size(956, 252);
    TokenListPage.TabIndex = 2;
    TokenListPage.Text = "Token List";
    // 
    // ClearTokensButton
    // 
    ClearTokensButton.Enabled = false;
    ClearTokensButton.FlatStyle = FlatStyle.System;
    ClearTokensButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    ClearTokensButton.Location = new Point(840, 219);
    ClearTokensButton.Margin = new Padding(3, 2, 3, 2);
    ClearTokensButton.Name = "ClearTokensButton";
    ClearTokensButton.Size = new Size(96, 28);
    ClearTokensButton.TabIndex = 22;
    ClearTokensButton.Text = "Clear Token Table";
    ClearTokensButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    ClearTokensButton.UseVisualStyleBackColor = true;
    ClearTokensButton.Click += ClearTokens;
    // 
    // TokenTableLabel
    // 
    TokenTableLabel.BackColor = Color.Transparent;
    TokenTableLabel.Font = new Font("Arial Rounded MT Bold", 15.75F, FontStyle.Regular, GraphicsUnit.Point,  0);
    TokenTableLabel.Location = new Point(0, 0);
    TokenTableLabel.Margin = new Padding(2, 0, 2, 0);
    TokenTableLabel.Name = "TokenTableLabel";
    TokenTableLabel.Size = new Size(135, 24);
    TokenTableLabel.TabIndex = 16;
    TokenTableLabel.Text = "Token Table";
    // 
    // TokenLabel
    // 
    TokenLabel.AutoSize = true;
    TokenLabel.Location = new Point(688, 7);
    TokenLabel.Name = "TokenLabel";
    TokenLabel.Size = new Size(82, 15);
    TokenLabel.TabIndex = 14;
    TokenLabel.Text = "Tokens Parsed";
    // 
    // TokenCountLabel
    // 
    TokenCountLabel.Location = new Point(784, 7);
    TokenCountLabel.Name = "TokenCountLabel";
    TokenCountLabel.Size = new Size(38, 18);
    TokenCountLabel.TabIndex = 15;
    TokenCountLabel.Text = "0";
    // 
    // TokenDataGrid
    // 
    TokenDataGrid.AllowUserToAddRows = false;
    TokenDataGrid.AllowUserToDeleteRows = false;
    TokenDataGrid.AllowUserToResizeRows = false;
    TokenDataGrid.AutoGenerateColumns = false;
    TokenDataGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
    TokenDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    TokenDataGrid.Columns.AddRange(new DataGridViewColumn[] { TypeTokenColumn, ContentTokenColumn, ChildrenTokenColumn });
    TokenDataGrid.DataSource = TokenBindingSource;
    TokenDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
    TokenDataGrid.GridColor = SystemColors.MenuText;
    TokenDataGrid.Location = new Point(8, 28);
    TokenDataGrid.Margin = new Padding(3, 2, 3, 2);
    TokenDataGrid.Name = "TokenDataGrid";
    TokenDataGrid.ReadOnly = true;
    TokenDataGrid.RowHeadersVisible = false;
    TokenDataGrid.RowHeadersWidth = 51;
    TokenDataGrid.ScrollBars = ScrollBars.Vertical;
    TokenDataGrid.Size = new Size(816, 219);
    TokenDataGrid.TabIndex = 13;
    // 
    // TypeTokenColumn
    // 
    TypeTokenColumn.DataPropertyName = "Type";
    TypeTokenColumn.HeaderText = "Type";
    TypeTokenColumn.MinimumWidth = 6;
    TypeTokenColumn.Name = "TypeTokenColumn";
    TypeTokenColumn.ReadOnly = true;
    TypeTokenColumn.Width = 125;
    // 
    // ContentTokenColumn
    // 
    ContentTokenColumn.DataPropertyName = "Content";
    ContentTokenColumn.HeaderText = "Content";
    ContentTokenColumn.MinimumWidth = 6;
    ContentTokenColumn.Name = "ContentTokenColumn";
    ContentTokenColumn.ReadOnly = true;
    ContentTokenColumn.Width = 400;
    // 
    // ChildrenTokenColumn
    // 
    ChildrenTokenColumn.DataPropertyName = "Children";
    ChildrenTokenColumn.HeaderText = "Children";
    ChildrenTokenColumn.MinimumWidth = 6;
    ChildrenTokenColumn.Name = "ChildrenTokenColumn";
    ChildrenTokenColumn.ReadOnly = true;
    ChildrenTokenColumn.Resizable = DataGridViewTriState.True;
    ChildrenTokenColumn.SortMode = DataGridViewColumnSortMode.Automatic;
    ChildrenTokenColumn.Width = 125;
    // 
    // TokenBindingSource
    // 
    TokenBindingSource.DataSource = typeof(Parser.Tokens.IToken);
    // 
    // ShowUnparsedButton
    // 
    ShowUnparsedButton.Enabled = false;
    ShowUnparsedButton.FlatStyle = FlatStyle.System;
    ShowUnparsedButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    ShowUnparsedButton.Location = new Point(840, 176);
    ShowUnparsedButton.Margin = new Padding(3, 2, 3, 2);
    ShowUnparsedButton.Name = "ShowUnparsedButton";
    ShowUnparsedButton.Size = new Size(96, 33);
    ShowUnparsedButton.TabIndex = 8;
    ShowUnparsedButton.Text = "Show Unparsed Text";
    ShowUnparsedButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    ShowUnparsedButton.UseVisualStyleBackColor = true;
    ShowUnparsedButton.Click += ShowUnparsed;
    // 
    // AssemblerPage
    // 
    AssemblerPage.Controls.Add(button3);
    AssemblerPage.Controls.Add(GroupTokenRuleCountLabel);
    AssemblerPage.Controls.Add(label3);
    AssemblerPage.Controls.Add(button2);
    AssemblerPage.Controls.Add(button1);
    AssemblerPage.Controls.Add(label1);
    AssemblerPage.Controls.Add(TokenGroupRuleDataGrid);
    AssemblerPage.Location = new Point(4, 24);
    AssemblerPage.Name = "AssemblerPage";
    AssemblerPage.Padding = new Padding(3, 2, 3, 2);
    AssemblerPage.Size = new Size(956, 252);
    AssemblerPage.TabIndex = 1;
    AssemblerPage.Text = "Assembler";
    AssemblerPage.UseVisualStyleBackColor = true;
    // 
    // button3
    // 
    button3.Enabled = false;
    button3.FlatStyle = FlatStyle.System;
    button3.Font = new Font("Bahnschrift SemiCondensed", 9F);
    button3.Location = new Point(840, 148);
    button3.Margin = new Padding(3, 2, 3, 2);
    button3.Name = "button3";
    button3.Size = new Size(96, 40);
    button3.TabIndex = 18;
    button3.Text = "Save Ruleset to File";
    button3.TextImageRelation = TextImageRelation.ImageBeforeText;
    button3.UseVisualStyleBackColor = true;
    // 
    // GroupTokenRuleCountLabel
    // 
    GroupTokenRuleCountLabel.Location = new Point(784, 7);
    GroupTokenRuleCountLabel.Name = "GroupTokenRuleCountLabel";
    GroupTokenRuleCountLabel.Size = new Size(35, 14);
    GroupTokenRuleCountLabel.TabIndex = 16;
    GroupTokenRuleCountLabel.Text = "0";
    // 
    // label3
    // 
    label3.AutoSize = true;
    label3.Location = new Point(664, 7);
    label3.Name = "label3";
    label3.Size = new Size(106, 15);
    label3.TabIndex = 17;
    label3.Text = "Token Group Rules";
    // 
    // button2
    // 
    button2.Enabled = false;
    button2.FlatStyle = FlatStyle.System;
    button2.Font = new Font("Bahnschrift SemiCondensed", 9F);
    button2.Location = new Point(840, 71);
    button2.Margin = new Padding(3, 2, 3, 2);
    button2.Name = "button2";
    button2.Size = new Size(96, 37);
    button2.TabIndex = 15;
    button2.Text = "Load Rules from Specification";
    button2.TextImageRelation = TextImageRelation.ImageBeforeText;
    button2.UseVisualStyleBackColor = true;
    // 
    // button1
    // 
    button1.Enabled = false;
    button1.FlatStyle = FlatStyle.System;
    button1.Font = new Font("Bahnschrift SemiCondensed", 9F);
    button1.Location = new Point(840, 219);
    button1.Margin = new Padding(3, 2, 3, 2);
    button1.Name = "button1";
    button1.Size = new Size(96, 28);
    button1.TabIndex = 14;
    button1.Text = "Clear Rule Table";
    button1.TextImageRelation = TextImageRelation.ImageBeforeText;
    button1.UseVisualStyleBackColor = true;
    // 
    // label1
    // 
    label1.Font = new Font("Arial Rounded MT Bold", 15.75F, FontStyle.Regular, GraphicsUnit.Point,  0);
    label1.Location = new Point(0, 0);
    label1.Margin = new Padding(2, 0, 2, 0);
    label1.Name = "label1";
    label1.Size = new Size(280, 24);
    label1.TabIndex = 13;
    label1.Text = "Group Rule Table";
    // 
    // TokenGroupRuleDataGrid
    // 
    TokenGroupRuleDataGrid.AllowUserToOrderColumns = true;
    TokenGroupRuleDataGrid.AutoGenerateColumns = false;
    TokenGroupRuleDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    TokenGroupRuleDataGrid.Columns.AddRange(new DataGridViewColumn[] { dataGridViewButtonColumn1, dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3 });
    TokenGroupRuleDataGrid.DataSource = TokenRuleBindingSource;
    TokenGroupRuleDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
    TokenGroupRuleDataGrid.GridColor = SystemColors.MenuText;
    TokenGroupRuleDataGrid.Location = new Point(8, 28);
    TokenGroupRuleDataGrid.Margin = new Padding(3, 2, 3, 2);
    TokenGroupRuleDataGrid.MultiSelect = false;
    TokenGroupRuleDataGrid.Name = "TokenGroupRuleDataGrid";
    TokenGroupRuleDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
    TokenGroupRuleDataGrid.RowHeadersVisible = false;
    TokenGroupRuleDataGrid.RowHeadersWidth = 51;
    dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
    dataGridViewCellStyle4.Font = new Font("Cascadia Code", 9.75F, FontStyle.Regular, GraphicsUnit.Point,  0);
    dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
    TokenGroupRuleDataGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
    TokenGroupRuleDataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
    TokenGroupRuleDataGrid.Size = new Size(816, 219);
    TokenGroupRuleDataGrid.TabIndex = 8;
    // 
    // dataGridViewButtonColumn1
    // 
    dataGridViewButtonColumn1.HeaderText = "Edit";
    dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
    dataGridViewButtonColumn1.Text = "Edit";
    dataGridViewButtonColumn1.Width = 50;
    // 
    // dataGridViewTextBoxColumn1
    // 
    dataGridViewTextBoxColumn1.DataPropertyName = "Type";
    dataGridViewCellStyle3.NullValue = "None";
    dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
    dataGridViewTextBoxColumn1.HeaderText = "Type";
    dataGridViewTextBoxColumn1.MinimumWidth = 6;
    dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
    dataGridViewTextBoxColumn1.Resizable = DataGridViewTriState.True;
    dataGridViewTextBoxColumn1.Width = 125;
    // 
    // dataGridViewTextBoxColumn2
    // 
    dataGridViewTextBoxColumn2.DataPropertyName = "TypeToAssign";
    dataGridViewTextBoxColumn2.HeaderText = "TypeToAssign";
    dataGridViewTextBoxColumn2.MinimumWidth = 6;
    dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
    dataGridViewTextBoxColumn2.Resizable = DataGridViewTriState.True;
    dataGridViewTextBoxColumn2.Width = 125;
    // 
    // dataGridViewTextBoxColumn3
    // 
    dataGridViewTextBoxColumn3.DataPropertyName = "RuleStringData";
    dataGridViewTextBoxColumn3.HeaderText = "RuleStringData";
    dataGridViewTextBoxColumn3.MinimumWidth = 6;
    dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
    dataGridViewTextBoxColumn3.Width = 500;
    // 
    // LoadParseFileButton
    // 
    LoadParseFileButton.Enabled = false;
    LoadParseFileButton.FlatStyle = FlatStyle.System;
    LoadParseFileButton.Font = new Font("Bahnschrift SemiCondensed", 9F);
    LoadParseFileButton.Location = new Point(928, 315);
    LoadParseFileButton.Margin = new Padding(3, 2, 3, 2);
    LoadParseFileButton.Name = "LoadParseFileButton";
    LoadParseFileButton.Size = new Size(88, 26);
    LoadParseFileButton.TabIndex = 13;
    LoadParseFileButton.Text = "Load Parse File";
    LoadParseFileButton.TextImageRelation = TextImageRelation.ImageBeforeText;
    LoadParseFileButton.UseVisualStyleBackColor = true;
    LoadParseFileButton.Click += OpenParseFile;
    // 
    // ParsePathLabel
    // 
    ParsePathLabel.AutoSize = true;
    ParsePathLabel.Location = new Point(208, 329);
    ParsePathLabel.Margin = new Padding(2, 0, 2, 0);
    ParsePathLabel.Name = "ParsePathLabel";
    ParsePathLabel.Size = new Size(109, 15);
    ParsePathLabel.TabIndex = 15;
    ParsePathLabel.Text = "Path to file to parse";
    // 
    // ParsePathTextBox
    // 
    ParsePathTextBox.Location = new Point(208, 350);
    ParsePathTextBox.Margin = new Padding(2);
    ParsePathTextBox.Name = "ParsePathTextBox";
    ParsePathTextBox.Size = new Size(712, 23);
    ParsePathTextBox.TabIndex = 14;
    // 
    // ParseButton
    // 
    ParseButton.Location = new Point(928, 350);
    ParseButton.Margin = new Padding(3, 2, 3, 2);
    ParseButton.Name = "ParseButton";
    ParseButton.Size = new Size(88, 24);
    ParseButton.TabIndex = 12;
    ParseButton.Text = "Parse";
    ParseButton.UseVisualStyleBackColor = true;
    ParseButton.Click += ExecuteParse;
    // 
    // ParserForm
    // 
    AutoScaleDimensions = new SizeF(7F, 15F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(1181, 413);
    Controls.Add(LoadParseFileButton);
    Controls.Add(ParsePathLabel);
    Controls.Add(ParsePathTextBox);
    Controls.Add(ParseButton);
    Controls.Add(ItemTabs);
    Controls.Add(SpecLabel);
    Controls.Add(SpecComboBox);
    Controls.Add(StatusStrip);
    Controls.Add(TheMenuStrip);
    MainMenuStrip = TheMenuStrip;
    Margin = new Padding(3, 2, 3, 2);
    Name = "ParserForm";
    Text = "Parser Form";
    Load += ParserForm_Load;
    StatusStrip.ResumeLayout(false);
    StatusStrip.PerformLayout();
    TheMenuStrip.ResumeLayout(false);
    TheMenuStrip.PerformLayout();
    ((ISupportInitialize) SpecBindingSource).EndInit();
    ((ISupportInitialize) TokenRuleBindingSource).EndInit();
    ItemTabs.ResumeLayout(false);
    TokenRulesPage.ResumeLayout(false);
    TokenRulesPage.PerformLayout();
    ((ISupportInitialize) TokenRuleDataGrid).EndInit();
    TokenListPage.ResumeLayout(false);
    TokenListPage.PerformLayout();
    ((ISupportInitialize) TokenDataGrid).EndInit();
    ((ISupportInitialize) TokenBindingSource).EndInit();
    AssemblerPage.ResumeLayout(false);
    AssemblerPage.PerformLayout();
    ((ISupportInitialize) TokenGroupRuleDataGrid).EndInit();
    ResumeLayout(false);
    PerformLayout();
  }

  #endregion

  private BindingSource SpecBindingSource;
  private BindingSource TokenBindingSource;
  private BindingSource TokenRuleBindingSource;
  private Button button1;
  private Button button2;
  private Button button3;
  private Button ClearRulesButton;
  private Button ClearTokensButton;
  private Button LoadParseFileButton;
  private Button LoadRulesButton;
  private Button LoadRulesFileButton;
  private Button ParseButton;
  private Button SaveRuleButton;
  private Button ShowUnparsedButton;
  private ComboBox SpecComboBox;
  private DataGridView TokenDataGrid;
  private DataGridView TokenGroupRuleDataGrid;
  private DataGridView TokenRuleDataGrid;
  private DataGridViewButtonColumn dataGridViewButtonColumn1;
  private DataGridViewButtonColumn EditTokenRuleButtonColumn;
  private DataGridViewCheckBoxColumn ExemptTokenColumn;
  private DataGridViewComboBoxColumn TypeColumn;
  private DataGridViewComboBoxColumn TypeToAssignColumn;
  private DataGridViewLinkColumn ChildrenTokenColumn;
  private DataGridViewTextBoxColumn AssignTypeColumnText;
  private DataGridViewTextBoxColumn ContentTokenColumn;
  private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
  private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
  private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
  private DataGridViewTextBoxColumn RuleDataColumnText;
  private DataGridViewTextBoxColumn RuleStringDataColumn;
  private DataGridViewTextBoxColumn TypeTextColumn;
  private DataGridViewTextBoxColumn TypeTokenColumn;
  private Label GroupTokenRuleCountLabel;
  private Label label1;
  private Label label3;
  private Label ParsePathLabel;
  private Label RuleTableLabel;
  private Label SpecLabel;
  private Label TokenCountLabel;
  private Label TokenLabel;
  private Label TokenRuleCountLabel;
  private Label TokenRuleLabel;
  private Label TokenTableLabel;
  private MenuStrip TheMenuStrip;
  private OpenFileDialog OpenParseFileDialog;
  private SaveFileDialog SaveRuleFileDialog;
  private StatusStrip StatusStrip;
  private TabControl ItemTabs;
  private TabPage AssemblerPage;
  private TabPage TokenListPage;
  private TabPage TokenRulesPage;
  private TextBox ParsePathTextBox;
  private ToolStripMenuItem assemblyToolStripMenuItem;
  private ToolStripMenuItem clearRuleTableToolStripMenuItem;
  private ToolStripMenuItem clearTokenTableToolStripMenuItem;
  private ToolStripMenuItem ExitMenuItem;
  private ToolStripMenuItem FileMenu;
  private ToolStripMenuItem loadFromFilToolStripMenuItem;
  private ToolStripMenuItem loadFromSpecToolStripMenuItem;
  private ToolStripMenuItem loadFromSpecToolStripMenuItem1;
  private ToolStripMenuItem LoadSpecsMenuItem;
  private ToolStripMenuItem OpenFileMenuItem;
  private ToolStripMenuItem parseTokensToolStripMenuItem;
  private ToolStripMenuItem RuleMenuItem;
  private ToolStripMenuItem saveToFileToolStripMenuItem;
  private ToolStripMenuItem SpecMenuItem;
  private ToolStripMenuItem TokensMenu;
  private ToolStripMenuItem toolStripMenuItem1;
  private ToolStripProgressBar ParseProgressBar;
  private ToolStripSeparator ToolStripSeparator1;
  private ToolStripSeparator toolStripSeparator2;
}
