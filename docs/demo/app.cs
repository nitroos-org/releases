// Original demo program written for the NitroOS README lane (public domain / CC0).
using System;
using System.Drawing;
using System.Windows.Forms;

static class P
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        var f = new Form { Text = "NitroOS .NET WinForms demo", Width = 560, Height = 360, StartPosition = FormStartPosition.CenterScreen };
        var l = new Label { Left = 16, Top = 16, Width = 520, Height = 120, Font = new Font("Segoe UI", 10f) };
        l.Text = "CLR " + Environment.Version + "\r\n" +
                 "64-bit process: " + Environment.Is64BitProcess + "\r\n" +
                 "OS: " + Environment.OSVersion + "\r\n" +
                 "Processors: " + Environment.ProcessorCount;
        var t = new TextBox { Left = 16, Top = 150, Width = 300, Text = "Type here" };
        var b = new Button { Left = 330, Top = 148, Width = 120, Text = "Click me" };
        var pb = new ProgressBar { Left = 16, Top = 190, Width = 434, Height = 22, Value = 0 };
        var lv = new ListView { Left = 16, Top = 225, Width = 434, Height = 90, View = View.Details };
        lv.Columns.Add("Control", 200); lv.Columns.Add("Works", 200);
        lv.Items.Add(new ListViewItem(new[] { "ListView", "yes" }));
        lv.Items.Add(new ListViewItem(new[] { "ProgressBar", "yes" }));
        int n = 0;
        b.Click += (s, e) => { n += 10; pb.Value = Math.Min(100, n); l.Text += "\r\nclicked " + (n / 10) + "x: " + t.Text; };
        f.Controls.AddRange(new Control[] { l, t, b, pb, lv });
        Application.Run(f);
    }
}
