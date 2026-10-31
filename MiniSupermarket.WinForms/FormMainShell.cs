namespace MiniSupermarket.WinForms;

// Khung quản trị của buổi 3; các màn hình chỉ được thêm khi đã có API tương ứng.
public class FormMainShell : Form
{
    private readonly FlowLayoutPanel menu = new() { Dock = DockStyle.Left, Width = 195, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
    private readonly Panel content = new() { Dock = DockStyle.Fill };
    private Form? active;
    public FormMainShell()
    {
        Text = "MiniSuperMarket — Buổi 3";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1100, 700);
        Controls.Add(content);
        Controls.Add(menu);
        menu.Controls.Add(new Label { Text = $"{SessionManager.CurrentUsername}\n{SessionManager.CurrentRole}", Width = 180, Height = 55 });
        bool admin = SessionManager.CurrentRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        bool warehouse = SessionManager.CurrentRole.Equals("Warehouse", StringComparison.OrdinalIgnoreCase);
        AddPage("Nhóm hàng", () => new FormCategoryManagement());
        if (admin) AddPage("Khách hàng", () => new FormCustomerManagement());
        var logout = new Button { Text = "Đăng xuất", Width = 180, Height = 42 };
        logout.Click += (_, _) => { SessionManager.Logout(); Close(); };
        menu.Controls.Add(logout);
        UiTheme.Apply(this);
        Shown += (_, _) => OpenPage(new FormCategoryManagement());
    }
    private void AddPage(string label, Func<Form> factory)
    {
        var button = new Button { Text = label, Width = 180, Height = 42 };
        button.Click += (_, _) => OpenPage(factory());
        menu.Controls.Add(button);
    }
    private void OpenPage(Form form)
    {
        active?.Close();
        active?.Dispose();
        content.Controls.Clear();
        active = form;
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        content.Controls.Add(form);
        form.Show();
    }
}
