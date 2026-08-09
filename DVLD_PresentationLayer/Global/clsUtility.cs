using System;
using System.Windows.Forms;

public static class clsUtility
{
    public static string ShowInputBox(string prompt, string title, string defaultValue = "0")
    {
        Form inputForm = new Form()
        {
            Width = 400,
            Height = 180,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = title,
            StartPosition = FormStartPosition.CenterScreen,
            MinimizeBox = false,
            MaximizeBox = false,
            BackColor = System.Drawing.Color.White
        };

        Label lblPrompt = new Label() { Left = 20, Top = 20, Text = prompt, AutoSize = true };
        TextBox txtInput = new TextBox() { Left = 20, Top = 50, Width = 340, Text = defaultValue };

        Button btnOK = new Button() { Text = "OK", Left = 260, Width = 100, Top = 90, DialogResult = DialogResult.OK };
        Button btnCancel = new Button() { Text = "Cancel", Left = 150, Width = 100, Top = 90, DialogResult = DialogResult.Cancel };

        inputForm.Controls.Add(lblPrompt);
        inputForm.Controls.Add(txtInput);
        inputForm.Controls.Add(btnOK);
        inputForm.Controls.Add(btnCancel);

        inputForm.AcceptButton = btnOK;
        inputForm.CancelButton = btnCancel;

        return inputForm.ShowDialog() == DialogResult.OK ? txtInput.Text.Trim() : "";
    }
}