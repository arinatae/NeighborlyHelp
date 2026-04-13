using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeighborlyHelp.Models
{
    public static class DialogManager
    {
        public static void ShowDialog(Form parent, string npcName, List<string> lines)
        {
            Form dialogForm = new Form
            {
                Text = $"💬 {npcName}",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            TextBox dialogText = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(340, 100),
                Text = string.Join("\n\n", lines)
            };

            Button continueBtn = new Button
            {
                Text = "Закрыть",
                Location = new Point(150, 130),
                DialogResult = DialogResult.OK
            };

            dialogForm.Controls.Add(dialogText);
            dialogForm.Controls.Add(continueBtn);
            dialogForm.AcceptButton = continueBtn;

            dialogForm.ShowDialog(parent);
        }
    }
}