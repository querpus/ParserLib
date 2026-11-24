global using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;

namespace SerialSenderApp;

public partial class MainWindow : Window
{
  private readonly string _connectionString =
      "Server=aimwrnnyssql2;Database=NYSUS_SEQUENCE_MES;Integrated Security=True;TrustServerCertificate=True;";

  public MainWindow ()
  {
    InitializeComponent();
  }

  internal static bool IsValidSerial (string input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return false;

    foreach (char c in input)
    {
      if (!char.IsLetterOrDigit(c))
        return false;
    }

    return true;
  }
  internal static bool BlockSQLReason (string input, out string reason)
  {
    reason = input switch
    {
      string s when s.Contains('%', StringComparison.Ordinal) => "No wildcards allowed.",
      string s when s.Contains('\'', StringComparison.Ordinal) => "No single quotes.",
      string s when s.Contains("--", StringComparison.Ordinal) => "No Comment starts",
      string s when s.IsWhiteSpace() => "No whitespace only.",
      null => "null",
      _ => ""
    };

    return reason.Length == 0;
  }

  private async void SendButton_Click (object sender, RoutedEventArgs e)
  {
    try
    {
      StatusText.Text = "Validating...";

      List<string> serials = [.. SerialInput.Text
          .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
          .Select(s => s.Trim())
          .Where(s => !string.IsNullOrWhiteSpace(s))];
      if (serials.Count == 0)
      {
        StatusText.Text = "Blocked: no valid serials.";
        return;
      }

      // Validation
      foreach (string s in serials)
      {
        if (s.Contains('\'', StringComparison.Ordinal))
        {
          StatusText.Text = "Blocked: SQL Injection detected!";
          return;
        }
        if (!IsValidSerial(s))
        {
          StatusText.Text = $"Blocked: invalid serial [{s}]";
          return;
        }
      }

      // Build SQL
      string inClause = string.Join(",", serials.Select(s => $"'{s}'"));

      string sql = $@"
UPDATE wip_parts
SET line_ID = 99999,
    used_by_module_ID = NULL
WHERE external_serial IN ({inClause})";

      // === PREVIEW BEFORE EXECUTION ===
      MessageBoxResult confirm = MessageBox.Show(
          sql,
          "Confirm SQL Execution",
          MessageBoxButton.YesNo,
          MessageBoxImage.Warning);

      if (confirm != MessageBoxResult.Yes)
      {
        StatusText.Text = "Cancelled by user.";
        return;
      }

      StatusText.Text = "Executing...";

      await using SqlConnection conn = new(_connectionString);
      await conn.OpenAsync();

      await using SqlCommand cmd = new(sql, conn);
      int rows = await cmd.ExecuteNonQueryAsync();

      StatusText.Text = $"Updated {rows} record(s).";
      Log($"OK | Rows={rows} | SQL={sql}");
    }
    catch (SqlException ex)
    {
      StatusText.Text = "Execution error.";
      Log($"ERROR: {ex.Message}");
    }
  }

  private async void SendButton_Click_Prev (object sender, RoutedEventArgs e)
  {
    try
    {
      StatusText.Text = "Processing...";

      List<string> serials = [.. SerialInput.Text
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())];

      if (serials.Count == 0)
      {
        StatusText.Text = "No serials entered.";
        return;
      }

      int successCount = 0;

      await using (SqlConnection conn = new(_connectionString))
      {
        await conn.OpenAsync();

        foreach (string serial in serials)
        {
          await using SqlCommand cmd = new();
          cmd.Connection = conn;

          // === CHANGE THIS TO YOUR ACTUAL SQL ===
          cmd.CommandText = @"
                                INSERT INTO YourTableName (SerialNumber, CreatedDate)
                                VALUES (@serial, GETDATE())";

          _ = cmd.Parameters.AddWithValue("@serial", serial);

          _ = await cmd.ExecuteNonQueryAsync();
          successCount++;
        }
      }

      StatusText.Text = $"Success: {successCount} serial(s) sent.";
      Log($"SUCCESS: {successCount} serials");
    }
    catch (SqlException ex)
    {
      StatusText.Text = "Error sending data.";
      Log($"ERROR: {ex.Message}");
    }
  }

  private static void Log (string message)
  {
    try
    {
      File.AppendAllText("app.log",
          $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
    }
    catch (IOException)
    {
      // Suppress logging failure
    }
  }
}
