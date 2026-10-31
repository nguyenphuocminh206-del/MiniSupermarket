using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    /// <summary>Shared visual language for the WinForms screens.</summary>
    internal static class UiTheme
    {
        private static readonly Color Background = Color.FromArgb(246, 248, 252);
        private static readonly Color Surface = Color.White;
        private static readonly Color Text = Color.FromArgb(51, 65, 85);
        private static readonly Color MutedText = Color.FromArgb(100, 116, 139);
        private static readonly Color Border = Color.FromArgb(226, 232, 240);
        private static readonly Color Blue = Color.FromArgb(37, 99, 235);
        private static readonly Color Green = Color.FromArgb(5, 150, 105);
        private static readonly Color Red = Color.FromArgb(185, 28, 28);

        public static void Apply(Form form)
        {
            form.BackColor = Background;
            form.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            ApplyToControls(form.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case DataGridView grid:
                        StyleGrid(grid);
                        break;
                    case Button button:
                        StyleButton(button);
                        break;
                    case TextBox textBox:
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.BackColor = textBox.ReadOnly
                            ? Color.FromArgb(248, 250, 252)
                            : Surface;
                        textBox.ForeColor = Text;
                        break;
                    case ComboBox comboBox:
                        comboBox.FlatStyle = FlatStyle.Flat;
                        comboBox.BackColor = Surface;
                        comboBox.ForeColor = Text;
                        break;
                    case GroupBox groupBox:
                        groupBox.BackColor = Surface;
                        if (groupBox.ForeColor == SystemColors.ControlText)
                            groupBox.ForeColor = Text;
                        break;
                    case Label label:
                        if (label.ForeColor == SystemColors.ControlText)
                            label.ForeColor = label.Font.Size < 9F ? MutedText : Text;
                        break;
                    case Panel panel:
                        if (panel.BackColor == SystemColors.Control)
                            panel.BackColor = Surface;
                        break;
                }

                if (control is not DataGridView)
                    ApplyToControls(control.Controls);
            }
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = Math.Max(grid.ColumnHeadersHeight, 40);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(7, 0, 7, 0);
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            grid.DefaultCellStyle.Padding = new Padding(7, 0, 7, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);
            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.RowTemplate.Height = Math.Max(grid.RowTemplate.Height, 36);
        }

        private static void StyleButton(Button button)
        {
            string caption = button.Text.Trim().ToLowerInvariant();
            bool danger = caption.Contains("xóa") || caption.Contains("hủy") || caption.Contains("huy");
            bool success = caption.Contains("thanh toán") || caption.Contains("duyệt") || caption.Contains("xac nhan");
            bool primary = caption.Contains("thêm") || caption.Contains("cập nhật") || caption.Contains("lưu")
                || caption.Contains("đăng nhập") || caption.Contains("tìm kiếm") || caption.Contains("xem báo cáo");

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = danger ? 1 : 0;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            button.UseVisualStyleBackColor = false;

            if (danger)
            {
                button.BackColor = Color.FromArgb(254, 242, 242);
                button.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
                button.ForeColor = Red;
            }
            else if (success)
            {
                button.BackColor = Green;
                button.ForeColor = Color.White;
            }
            else if (primary)
            {
                button.BackColor = Blue;
                button.ForeColor = Color.White;
            }
            else
            {
                button.BackColor = Color.FromArgb(241, 245, 249);
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.BorderSize = 1;
                button.ForeColor = Text;
            }
        }
    }
}
