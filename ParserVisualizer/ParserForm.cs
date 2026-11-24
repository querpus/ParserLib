using System.Windows.Forms;

namespace ParserVisualizer;

internal sealed partial class ParserForm : Form
{
  public ParserForm () => InitializeComponent();

  private void Button2_Click (object sender, System.EventArgs e)
  {
    OperationBuilder opBuild = new();
    opBuild.Show(this);
    opBuild.Dispose();
  }
}
