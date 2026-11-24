using System;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;

namespace SerialSenderApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  [MTAThread]
  internal Task<int> Run ([NotNull] string[] args)
  {
    MainWindow mw = new();
    mw.InitializeComponent();
    _ = mw.Activate();
    return new(_ => args.Length, this);
  }
}

