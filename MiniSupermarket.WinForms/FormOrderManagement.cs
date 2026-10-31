using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormOrderManagement : Form
    {
        private readonly HttpClient _http = SessionManager.ApiClientService.Client;
        private const string API_URL = "https://localhost:7158/api/orders";

        private List<OrderDto> _orders = new();
        private TextBox txtSearch = null!;
        private ComboBox cboStatus = null!;
        private Label lblOrderCount = null!;
        private Label lblSelectedOrder = null!;
        private Label lblSelectedCustomer = null!;
        private Label lblSelectedDate = null!;
        private Label lblSelectedTotal = null!;
        private Label lblSelectedStatus = null!;
        private Label lblSelectedItems = null!;
        private Label[] _metricValues = Array.Empty<Label>();

        private int _selectedOrderId;

        public FormOrderManagement()
        {
            InitializeComponent();
            BuildDashboardLayout();
            UiTheme.Apply(this);

            dgvOrders.SelectionChanged += dgvOrders_SelectionChanged;
            dgvOrders.CellFormatting += dgvOrders_CellFormatting;
            txtSearch.TextChanged += (_, _) => ApplyOrderFilters();
            cboStatus.SelectedIndexChanged += (_, _) => ApplyOrderFilters();
            btnApprove.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void BuildDashboardLayout()
        {
            Controls.Clear();
            Text = "Quản lý đơn hàng";
            BackColor = Color.FromArgb(246, 248, 252);

            label1.Text = "Quản lý đơn hàng";
            label1.AutoSize = false;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(30, 41, 59);
            label1.TextAlign = ContentAlignment.MiddleLeft;

            var subtitle = new Label
            {
                Text = "Theo dõi trạng thái, giá trị và sản phẩm trong từng đơn",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0)
            };
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 62F));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));
            header.Controls.Add(label1, 0, 0);
            header.Controls.Add(subtitle, 0, 1);

            var toolbar = BuildToolbar();
            var metrics = BuildMetricCards();
            var body = BuildOrdersBody();

            var root = new TableLayoutPanel
            {
                Name = "layoutOrderDashboard",
                Dock = DockStyle.Fill,
                BackColor = BackColor,
                Padding = new Padding(22),
                ColumnCount = 1,
                RowCount = 4
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.Controls.Add(header, 0, 0);
            root.Controls.Add(toolbar, 0, 1);
            root.Controls.Add(metrics, 0, 2);
            root.Controls.Add(body, 0, 3);

            Controls.Add(root);
        }

        private Control BuildToolbar()
        {
            var toolbar = new TableLayoutPanel
            {
                Name = "panelOrderFilters",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(8, 5, 8, 5),
                ColumnCount = 5,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 8)
            };
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76F));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126F));
            toolbar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var searchLabel = CreateCaption("Tìm đơn:");
            txtSearch = new TextBox
            {
                Name = "txtOrderSearch",
                PlaceholderText = "Mã đơn hoặc mã khách hàng",
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 8, 10, 8)
            };

            var statusLabel = CreateCaption("Trạng thái:");
            cboStatus = new ComboBox
            {
                Name = "cboOrderStatus",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 7, 10, 7)
            };
            cboStatus.Items.AddRange(new object[]
            {
                "Tất cả trạng thái",
                "Chờ xử lý",
                "Đã duyệt / thanh toán",
                "Đã hủy"
            });
            cboStatus.SelectedIndex = 0;

            var btnReload = new Button
            {
                Name = "btnReloadOrders",
                Text = "Tải lại",
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 5, 0, 5)
            };
            btnReload.Click += async (_, _) => await LoadOrders();

            toolbar.Controls.Add(searchLabel, 0, 0);
            toolbar.Controls.Add(txtSearch, 1, 0);
            toolbar.Controls.Add(statusLabel, 2, 0);
            toolbar.Controls.Add(cboStatus, 3, 0);
            toolbar.Controls.Add(btnReload, 4, 0);
            return toolbar;
        }

        private Control BuildMetricCards()
        {
            string[] captions =
            {
                "TỔNG SỐ ĐƠN",
                "CHỜ XỬ LÝ",
                "ĐÃ DUYỆT / THANH TOÁN",
                "ĐÃ HỦY"
            };

            var cards = new TableLayoutPanel
            {
                Name = "layoutOrderMetrics",
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 2, 0, 10)
            };
            for (int i = 0; i < 4; i++)
                cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _metricValues = new Label[4];
            for (int i = 0; i < captions.Length; i++)
            {
                var card = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(i == 0 ? 0 : 6, 0, i == captions.Length - 1 ? 0 : 6, 0),
                    Padding = new Padding(14, 8, 12, 7)
                };
                var cardLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    Margin = new Padding(0)
                };
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                var caption = new Label
                {
                    Text = captions[i],
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoEllipsis = true
                };
                _metricValues[i] = new Label
                {
                    Text = "0",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                    ForeColor = i == 3 ? Color.FromArgb(185, 28, 28) : Color.FromArgb(30, 64, 120),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                cardLayout.Controls.Add(caption, 0, 0);
                cardLayout.Controls.Add(_metricValues[i], 0, 1);
                card.Controls.Add(cardLayout);
                cards.Controls.Add(card, i, 0);
            }

            return cards;
        }

        private Control BuildOrdersBody()
        {
            var body = new TableLayoutPanel
            {
                Name = "layoutOrderContent",
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var orderList = new Panel
            {
                Name = "panelOrderList",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 7, 0)
            };
            var listLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12, 8, 12, 10)
            };
            listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            listLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var listHeader = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            listHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            listHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            listHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            label2.Text = "Danh sách đơn hàng";
            label2.AutoSize = false;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(30, 41, 59);
            label2.TextAlign = ContentAlignment.MiddleLeft;
            lblOrderCount = new Label
            {
                Text = "0 đơn hàng",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleRight
            };
            listHeader.Controls.Add(label2, 0, 0);
            listHeader.Controls.Add(lblOrderCount, 1, 0);

            ConfigureGrid(dgvOrders);
            dgvOrders.Name = "dgvOrders";
            dgvOrders.AutoGenerateColumns = true;
            listLayout.Controls.Add(listHeader, 0, 0);
            listLayout.Controls.Add(dgvOrders, 0, 1);
            orderList.Controls.Add(listLayout);

            var detailPanel = new Panel
            {
                Name = "panelOrderDetails",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(7, 0, 0, 0)
            };
            var detailLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12, 8, 12, 10)
            };
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));

            var detailHeader = new Label
            {
                Text = "Chi tiết đơn hàng",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var summary = BuildOrderSummary();
            ConfigureGrid(dgvOrderDetails);
            dgvOrderDetails.Name = "dgvOrderDetails";
            dgvOrderDetails.AutoGenerateColumns = false;
            dgvOrderDetails.Columns.Clear();
            dgvOrderDetails.Columns.Add(CreateTextColumn("ProductId", "Mã sản phẩm", 24F));
            dgvOrderDetails.Columns.Add(CreateTextColumn("Quantity", "Số lượng", 18F));
            dgvOrderDetails.Columns.Add(CreateMoneyColumn("UnitPrice", "Đơn giá", 25F));
            dgvOrderDetails.Columns.Add(CreateMoneyColumn("LineTotal", "Thành tiền", 33F));

            var actions = new TableLayoutPanel
            {
                Name = "panelOrderActions",
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0, 8, 0, 8),
                ColumnCount = 2,
                RowCount = 1
            };
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            actions.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            btnCancel.Text = "Hủy đơn";
            btnCancel.Anchor = AnchorStyles.None;
            btnCancel.Size = new Size(92, 30);
            btnCancel.Margin = new Padding(0);
            btnApprove.Text = "Duyệt đơn";
            btnApprove.Anchor = AnchorStyles.None;
            btnApprove.Size = new Size(92, 30);
            btnApprove.Margin = new Padding(0);
            actions.Controls.Add(btnApprove, 0, 0);
            actions.Controls.Add(btnCancel, 1, 0);

            detailLayout.Controls.Add(detailHeader, 0, 0);
            detailLayout.Controls.Add(summary, 0, 1);
            detailLayout.Controls.Add(dgvOrderDetails, 0, 2);
            detailLayout.Controls.Add(actions, 0, 3);
            detailPanel.Controls.Add(detailLayout);

            body.Controls.Add(orderList, 0, 0);
            body.Controls.Add(detailPanel, 1, 0);
            return body;
        }

        private Control BuildOrderSummary()
        {
            var summary = new TableLayoutPanel
            {
                Name = "layoutSelectedOrderSummary",
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10, 5, 10, 5),
                Margin = new Padding(0, 0, 0, 8)
            };
            summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int i = 0; i < 3; i++)
                summary.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));

            summary.Controls.Add(CreateSummaryCell("MÃ ĐƠN HÀNG", out lblSelectedOrder), 0, 0);
            summary.Controls.Add(CreateSummaryCell("KHÁCH HÀNG", out lblSelectedCustomer), 1, 0);
            summary.Controls.Add(CreateSummaryCell("NGÀY TẠO", out lblSelectedDate), 0, 1);
            summary.Controls.Add(CreateSummaryCell("TỔNG TIỀN", out lblSelectedTotal), 1, 1);
            summary.Controls.Add(CreateSummaryCell("TRẠNG THÁI", out lblSelectedStatus), 0, 2);
            summary.Controls.Add(CreateSummaryCell("MẶT HÀNG", out lblSelectedItems), 1, 2);
            lblSelectedItems.Name = "lblSelectedItems";
            lblSelectedItems.Text = "—";
            return summary;
        }

        private static Control CreateSummaryCell(string captionText, out Label value)
        {
            var cell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(3, 1, 3, 1)
            };
            cell.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            cell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            var caption = new Label
            {
                Text = captionText,
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoEllipsis = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleLeft
            };
            value = new Label
            {
                Text = "—",
                Dock = DockStyle.Fill,
                AutoSize = false,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            cell.Controls.Add(caption, 0, 0);
            cell.Controls.Add(value, 0, 1);
            return cell;
        }

        private static Label CreateCaption(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(string propertyName, string header, float fillWeight)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = propertyName,
                DataPropertyName = propertyName,
                HeaderText = header,
                FillWeight = fillWeight,
                ReadOnly = true
            };
        }

        private static DataGridViewTextBoxColumn CreateMoneyColumn(string propertyName, string header, float fillWeight)
        {
            var column = CreateTextColumn(propertyName, header, fillWeight);
            column.DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" };
            return column;
        }

        private static void ConfigureGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.AutoGenerateColumns = true;
        }

        private async void FormOrderManagement_Load(object sender, EventArgs e)
        {
            await LoadOrders();
        }

        private async System.Threading.Tasks.Task LoadOrders()
        {
            try
            {
                _orders = await _http.GetFromJsonAsync<List<OrderDto>>(API_URL) ?? new List<OrderDto>();
                UpdateMetricCards();
                ApplyOrderFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi tải đơn hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMetricCards()
        {
            if (_metricValues.Length < 4)
                return;

            _metricValues[0].Text = _orders.Count.ToString("N0");
            _metricValues[1].Text = _orders.Count(order => IsPending(order.Status)).ToString("N0");
            _metricValues[2].Text = _orders.Count(order => IsApproved(order.Status)).ToString("N0");
            _metricValues[3].Text = _orders.Count(order => IsCancelled(order.Status)).ToString("N0");
        }

        private void ApplyOrderFilters()
        {
            if (dgvOrders == null || txtSearch == null || cboStatus == null)
                return;

            IEnumerable<OrderDto> filtered = _orders;
            string keyword = txtSearch.Text.Trim();
            if (keyword.Length > 0)
            {
                filtered = filtered.Where(order =>
                    order.OrderId.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    order.CustomerId.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            filtered = cboStatus.SelectedIndex switch
            {
                1 => filtered.Where(order => IsPending(order.Status)),
                2 => filtered.Where(order => IsApproved(order.Status)),
                3 => filtered.Where(order => IsCancelled(order.Status)),
                _ => filtered
            };

            var visibleOrders = filtered.OrderByDescending(order => order.OrderDate).ToList();
            dgvOrders.DataSource = visibleOrders;
            ConfigureOrderColumns();
            lblOrderCount.Text = $"{visibleOrders.Count:N0} / {_orders.Count:N0} đơn";

            if (visibleOrders.Count == 0)
            {
                dgvOrders.ClearSelection();
                ShowSelectedOrder(null);
                return;
            }

            int selectedIndex = visibleOrders.FindIndex(order => order.OrderId == _selectedOrderId);
            if (selectedIndex < 0)
                selectedIndex = 0;

            dgvOrders.CurrentCell = dgvOrders.Rows[selectedIndex].Cells[0];
            dgvOrders.Rows[selectedIndex].Selected = true;
            ShowSelectedOrder(visibleOrders[selectedIndex]);
        }

        private void ConfigureOrderColumns()
        {
            SetOrderColumn("OrderId", "Mã đơn", 12F);
            SetOrderColumn("CustomerId", "Mã khách hàng", 16F);
            SetOrderColumn("OrderDate", "Ngày tạo", 25F, "dd/MM/yyyy HH:mm");
            SetOrderColumn("TotalAmount", "Tổng tiền", 22F, "N0");
            SetOrderColumn("Status", "Trạng thái", 25F);

            if (dgvOrders.Columns["OrderDetails"] != null)
                dgvOrders.Columns["OrderDetails"].Visible = false;
        }

        private void SetOrderColumn(string name, string header, float fillWeight, string? format = null)
        {
            if (dgvOrders.Columns[name] is not DataGridViewColumn column)
                return;

            column.HeaderText = header;
            column.FillWeight = fillWeight;
            if (!string.IsNullOrEmpty(format))
                column.DefaultCellStyle = new DataGridViewCellStyle { Format = format };
        }

        private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowCurrentOrder();
        }

        private void dgvOrders_SelectionChanged(object? sender, EventArgs e)
        {
            ShowCurrentOrder();
        }

        private void ShowCurrentOrder()
        {
            if (dgvOrders.CurrentRow?.DataBoundItem is OrderDto order)
                ShowSelectedOrder(order);
        }

        private void ShowSelectedOrder(OrderDto? order)
        {
            if (order == null)
            {
                _selectedOrderId = 0;
                lblSelectedOrder.Text = "—";
                lblSelectedCustomer.Text = "—";
                lblSelectedDate.Text = "—";
                lblSelectedTotal.Text = "—";
                lblSelectedStatus.Text = "Chọn một đơn hàng";
                lblSelectedItems.Text = "—";
                btnApprove.Enabled = false;
                btnCancel.Enabled = false;
                dgvOrderDetails.DataSource = null;
                return;
            }

            _selectedOrderId = order.OrderId;
            lblSelectedOrder.Text = $"#{order.OrderId}";
            lblSelectedCustomer.Text = order.CustomerId > 0 ? $"Khách hàng #{order.CustomerId}" : "Khách vãng lai";
            lblSelectedDate.Text = order.OrderDate.ToString("dd/MM/yyyy HH:mm");
            lblSelectedTotal.Text = $"{order.TotalAmount:N0} đ";
            lblSelectedStatus.Text = string.IsNullOrWhiteSpace(order.Status) ? "Chưa cập nhật" : order.Status;
            lblSelectedStatus.ForeColor = IsCancelled(order.Status)
                ? Color.FromArgb(185, 28, 28)
                : IsApproved(order.Status)
                    ? Color.FromArgb(5, 150, 105)
                    : Color.FromArgb(180, 83, 9);

            var details = order.OrderDetails ?? new List<OrderDetailDto>();
            lblSelectedItems.Text = $"{details.Count:N0} mặt hàng";
            dgvOrderDetails.DataSource = details.Select(detail => new OrderDetailRow
            {
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                LineTotal = detail.Quantity * detail.UnitPrice
            }).ToList();

            btnApprove.Enabled = IsPending(order.Status);
            btnCancel.Enabled = !IsCancelled(order.Status) &&
                                (IsPending(order.Status) || IsApproved(order.Status));
        }

        private void dgvOrders_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                dgvOrders.Columns[e.ColumnIndex].DataPropertyName != nameof(OrderDto.Status))
                return;

            if (e.CellStyle is not DataGridViewCellStyle style)
                return;

            string status = Convert.ToString(e.Value) ?? string.Empty;
            if (IsCancelled(status))
                style.ForeColor = Color.FromArgb(185, 28, 28);
            else if (IsApproved(status))
                style.ForeColor = Color.FromArgb(5, 150, 105);
            else
                style.ForeColor = Color.FromArgb(180, 83, 9);
        }

        private static bool IsCancelled(string? status)
        {
            string normalized = (status ?? string.Empty).Trim().ToLowerInvariant();
            return normalized.Contains("hủy") || normalized.Contains("huy") || normalized.Contains("cancel");
        }

        private static bool IsApproved(string? status)
        {
            string normalized = (status ?? string.Empty).Trim().ToLowerInvariant();
            return normalized.Contains("duyệt") || normalized.Contains("duyet") ||
                   normalized.Contains("paid") || normalized.Contains("complete") ||
                   normalized.Contains("hoàn tất") || normalized.Contains("hoan tat");
        }

        private static bool IsPending(string? status)
        {
            string normalized = (status ?? string.Empty).Trim().ToLowerInvariant();
            return normalized is "pending" or "chờ xử lý" or "cho xu ly";
        }

        private async void btnApprove_Click(object sender, EventArgs e)
        {
            await UpdateOrderStatus("Đã duyệt");
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            if (_selectedOrderId == 0)
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng trên lưới!");
                return;
            }

            var confirm = MessageBox.Show(
                "Hủy đơn hàng này? Hệ thống sẽ hoàn tồn kho và điều chỉnh điểm thưởng nếu có.",
                "Xác nhận hủy đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            await UpdateOrderStatus("Đã hủy");
        }

        private async System.Threading.Tasks.Task UpdateOrderStatus(string status)
        {
            if (_selectedOrderId == 0)
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng trên lưới!");
                return;
            }

            try
            {
                var response = await _http.PutAsJsonAsync($"{API_URL}/{_selectedOrderId}/status", status);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Đã chuyển trạng thái đơn hàng thành: {status}", "Đã cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadOrders();
                }
                else
                {
                    string detail = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(detail) ? "Cập nhật thất bại!" : $"Cập nhật thất bại: {detail}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật trạng thái đơn hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    internal class OrderDetailRow
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
