using System;
using System.Windows.Forms;

namespace ParserVisualizer;

internal sealed partial class OperationBuilder : Form
{
  public OperationBuilder ()
  {
    InitializeComponent();
  }

  private void OperationBuilder_Load (object sender, EventArgs e)
  {
    _ = OperationListBox.Items.Add("CombineDelimOperation");
    _ = OperationListBox.Items.Add("DictionaryOperation");
    _ = OperationListBox.Items.Add("TokenizeOperation");
    _ = OperationListBox.Items.Add("GenerateOperation");
    _ = OperationListBox.Items.Add("DebugToStringOperation");
    _ = OperationListBox.Items.Add("TokenTemplateOperation");
    _ = OperationListBox.Items.Add("IfOperation");
    _ = OperationListBox.Items.Add("JumpOperation");
    _ = OperationListBox.Items.Add("OperationLabel");
    _ = OperationListBox.Items.Add("OperationCollection");
    _ = OperationListBox.Items.Add("DebugWaitForInputOperation");
    _ = OperationListBox.Items.Add("ExternalOperation");
  }

  private void OperationListBox_SelectedIndexChanged (object sender, EventArgs e)
  {
    if (OperationListBox.SelectedIndex > -1)
    {
      tabControl1.TabPages.Clear();
      tabControl1.TabPages.Add(tabPage1);
    }
    else
    {
      tabPage1.Hide();
      tabPage2.Hide();
    }
  }
}
