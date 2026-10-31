using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static MiniSupermarket.WinForms.SessionManager;

namespace MiniSupermarket.WinForms
{
    public partial class FormReport : Form
    {
        private readonly Label _newCustomersTitle = new() { Text = "KHÁCH MỚI TRONG NGÀY", AutoSize = true, Font = new Font("Segoe UI", 11F) };
        private readonly Label _newCustomersValue = new() { Text = "0", AutoSize = true, Font = new Font("Segoe UI", 26F, FontStyle.Bold) };
        private readonly Panel _newCustomersPanel = new() { BorderStyle = BorderStyle.FixedSingle };
        private readonly Panel _revenueChart = new() { BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
        private readonly Chart _revenueControl = new() { Dock = DockStyle.Fill, BackColor = Color.White };
        private readonly Label _emptyChartLabel = new()
        {
            Text = "Chưa có doanh thu trong khoảng ngày này.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Gray,
            BackColor = Color.Transparent
        };
        private readonly DataGridView _topProducts = new() { ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
        private readonly Label _chartTitle = new() { Text = "DOANH THU THEO NGÀY", AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };
        private readonly Label _topProductsTitle = new() { Text = "TOP 5 SẢN PHẨM BÁN CHẠY", AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };
        private readonly DateTimePicker _dtpTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        private readonly Label _lblTo = new() { Text = "Đến ngày:", AutoSize = true };
        private readonly Label _lblPeriod = new() { Text = "Nhóm:", AutoSize = true };
        private readonly ComboBox _period = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Button _exportProducts = new() { Text = "Xuất tồn kho CSV" };
        private readonly Button _exportOrders = new() { Text = "Xuất hóa đơn CSV" };
        private List<RevenuePointDto> _revenuePoints = new();

        public FormReport()
        {
            InitializeComponent();

            dtpDate.Value = DateTime.Today;
            _dtpTo.Value = DateTime.Today;
            _period.Items.AddRange(new object[] { "Ngày", "Tuần", "Tháng" });
            _period.SelectedIndex = 0;
            lblDate.Text = "Từ ngày:";
            lblBestSellerTitle.Text = "MẶT HÀNG SẮP HẾT";
            lblBestSellerValue.Text = "0";
            _newCustomersPanel.Controls.Add(_newCustomersTitle);
            _newCustomersPanel.Controls.Add(_newCustomersValue);
            _newCustomersTitle.Location = new Point(20, 20);
            _newCustomersValue.Location = new Point(20, 55);
            ConfigureRevenueChart();
            _revenueChart.Controls.Add(_revenueControl);
            _revenueChart.Controls.Add(_emptyChartLabel);
            _emptyChartLabel.BringToFront();
            Controls.Add(_lblTo);
            Controls.Add(_dtpTo);
            Controls.Add(_lblPeriod);
            Controls.Add(_period);
            Controls.Add(_exportProducts);
            Controls.Add(_exportOrders);
            Controls.Add(_newCustomersPanel);
            Controls.Add(_chartTitle);
            Controls.Add(_revenueChart);
            Controls.Add(_topProductsTitle);
            Controls.Add(_topProducts);
            _exportProducts.Click += async (s, e) => await ExportCsvAsync("reports/export/products", "ton-kho.csv");
            _exportOrders.Click += async (s, e) => await ExportCsvAsync(
                $"reports/export/orders?from={dtpDate.Value:yyyy-MM-dd}&to={_dtpTo.Value:yyyy-MM-dd}", "hoa-don.csv");
            UiTheme.Apply(this);
            Resize += FormReport_Resize;
            LayoutReportContent();
        }

        private void FormReport_Resize(object? sender, EventArgs e)
        {
            LayoutReportContent();
        }

        private void LayoutReportContent()
        {
            if (panelOrders == null || dgvRecentOrders == null || ClientSize.Width <= 0)
                return;

            const int sideMargin = 30;
            const int gap = 16;
            int cardWidth = Math.Max(150, (ClientSize.Width - sideMargin * 2 - gap * 3) / 4);

            panelOrders.SetBounds(sideMargin, 140, cardWidth, 130);
            panelRevenue.SetBounds(sideMargin + cardWidth + gap, 140, cardWidth, 130);
            panelBestSeller.SetBounds(sideMargin + (cardWidth + gap) * 2, 140, cardWidth, 130);
            _newCustomersPanel.SetBounds(sideMargin + (cardWidth + gap) * 3, 140, cardWidth, 130);
            lblBestSellerValue.MaximumSize = new Size(Math.Max(140, cardWidth - 40), 50);

            int half = Math.Max(250, (ClientSize.Width - sideMargin * 2 - gap) / 2);
            _chartTitle.SetBounds(sideMargin, 290, half, 25);
            _topProductsTitle.SetBounds(sideMargin + half + gap, 290, half, 25);
            _revenueChart.SetBounds(sideMargin, 320, half, 180);
            _topProducts.SetBounds(sideMargin + half + gap, 320, half, 180);
            lblRecentOrders.SetBounds(sideMargin, 520, ClientSize.Width - sideMargin * 2, 25);
            dgvRecentOrders.SetBounds(sideMargin, 550, Math.Max(200, ClientSize.Width - sideMargin * 2), Math.Max(100, ClientSize.Height - 570));
            _lblTo.SetBounds(440, 85, 70, 25);
            _dtpTo.SetBounds(510, 82, 150, 27);
            _lblPeriod.SetBounds(675, 85, 50, 25);
            _period.SetBounds(725, 82, 100, 27);
            _exportProducts.SetBounds(Math.Max(670, ClientSize.Width - 310), 80, 140, 32);
            _exportOrders.SetBounds(Math.Max(820, ClientSize.Width - 160), 80, 140, 32);
        }

        private async void FormReport_Load(object sender, EventArgs e)
        {
            await LoadReportAsync();
        }

        private async void btnViewReport_Click(object sender, EventArgs e)
        {
            await LoadReportAsync();
        }

        private async Task LoadReportAsync()
        {
            try
            {
                string date = dtpDate.Value.ToString("yyyy-MM-dd");
                string from = dtpDate.Value.ToString("yyyy-MM-dd");
                string to = _dtpTo.Value.ToString("yyyy-MM-dd");
                if (_dtpTo.Value.Date < dtpDate.Value.Date)
                {
                    MessageBox.Show("Ngày kết thúc phải bằng hoặc sau ngày bắt đầu.", "Khoảng ngày không hợp lệ");
                    return;
                }

                var report = await ApiClientService.Client
                    .GetFromJsonAsync<ReportDto>($"reports/daily?date={date}");
                var summary = await ApiClientService.Client.GetFromJsonAsync<SummaryDto>($"reports/summary?date={date}");
                string period = new[] { "day", "week", "month" }[_period.SelectedIndex];
                _revenuePoints = await ApiClientService.Client.GetFromJsonAsync<List<RevenuePointDto>>(
                    $"reports/revenue-series?from={from}&to={to}&period={period}") ?? new();
                var top = await ApiClientService.Client.GetFromJsonAsync<List<TopProductDto>>(
                    $"reports/top-products?from={from}&to={to}&take=5") ?? new();
                _topProducts.DataSource = top;
                if (_topProducts.Columns["ProductId"] != null) _topProducts.Columns["ProductId"].Visible = false;
                if (_topProducts.Columns["ProductName"] != null) _topProducts.Columns["ProductName"].HeaderText = "Sản phẩm";
                if (_topProducts.Columns["Quantity"] != null) _topProducts.Columns["Quantity"].HeaderText = "Đã bán";
                if (_topProducts.Columns["Revenue"] != null)
                {
                    _topProducts.Columns["Revenue"].HeaderText = "Doanh thu";
                    _topProducts.Columns["Revenue"].DefaultCellStyle.Format = "N0";
                }
                _newCustomersValue.Text = summary?.NewCustomers.ToString() ?? "0";
                lblBestSellerValue.Text = summary?.LowStockCount.ToString() ?? "0";
                UpdateRevenueChart(period);

                if (report == null)
                {
                    MessageBox.Show(
                        "Không có dữ liệu báo cáo!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                lblTotalOrdersValue.Text = report.TotalOrders.ToString();

                lblTotalRevenueValue.Text =
                    report.TotalRevenue.ToString("N0") + " VNĐ";

                dgvRecentOrders.DataSource = null;
                dgvRecentOrders.DataSource = report.RecentOrders;

                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi lấy báo cáo: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ConfigureRevenueChart()
        {
            var area = new ChartArea("Revenue");
            area.BackColor = Color.White;
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.IsStartedFromZero = true;
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(232, 237, 243);
            _revenueControl.ChartAreas.Add(area);

            var revenueSeries = new Series("Revenue")
            {
                ChartArea = "Revenue",
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(37, 99, 235),
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F)
            };
            revenueSeries["PointWidth"] = "0.6";
            _revenueControl.Series.Add(revenueSeries);
            _revenueControl.Legends.Clear();
            _emptyChartLabel.BringToFront();
        }

        private void UpdateRevenueChart(string period)
        {
            _chartTitle.Text = period switch
            {
                "week" => "DOANH THU THEO TUẦN",
                "month" => "DOANH THU THEO THÁNG",
                _ => "DOANH THU THEO NGÀY"
            };

            var series = _revenueControl.Series["Revenue"];
            series.Points.Clear();

            foreach (var point in _revenuePoints)
            {
                var dataPoint = new DataPoint
                {
                    AxisLabel = period switch
                    {
                        "week" => $"Tuần {point.Date:dd/MM}",
                        "month" => point.Date.ToString("MM/yyyy"),
                        _ => point.Date.ToString("dd/MM")
                    },
                    ToolTip = $"{point.Revenue:N0} đ"
                };
                dataPoint.YValues = new[] { (double)point.Revenue };
                series.Points.Add(dataPoint);
            }

            series.IsValueShownAsLabel = _revenuePoints.Count <= 12;
            _emptyChartLabel.Visible = _revenuePoints.Count == 0;
        }

        private async Task ExportCsvAsync(string endpoint, string fileName)
        {
            using var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = fileName };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                var bytes = await ApiClientService.Client.GetByteArrayAsync(endpoint);
                await File.WriteAllBytesAsync(dialog.FileName, bytes);
                MessageBox.Show("Đã xuất tệp CSV.", "Xuất báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xuất báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            if (dgvRecentOrders.Columns["OrderId"] != null)
                dgvRecentOrders.Columns["OrderId"].Visible = false;

            if (dgvRecentOrders.Columns["OrderCode"] != null)
                dgvRecentOrders.Columns["OrderCode"].HeaderText = "Mã đơn";

            if (dgvRecentOrders.Columns["CreatedAt"] != null)
            {
                dgvRecentOrders.Columns["CreatedAt"].HeaderText = "Ngày tạo";
                dgvRecentOrders.Columns["CreatedAt"].DefaultCellStyle.Format =
                    "dd/MM/yyyy HH:mm";
            }

            if (dgvRecentOrders.Columns["Total"] != null)
            {
                dgvRecentOrders.Columns["Total"].HeaderText = "Tổng tiền";
                dgvRecentOrders.Columns["Total"].DefaultCellStyle.Format =
                    "N0";
            }

            if (dgvRecentOrders.Columns["Status"] != null)
                dgvRecentOrders.Columns["Status"].HeaderText = "Trạng thái";

            if (dgvRecentOrders.Columns["CustomerName"] != null)
                dgvRecentOrders.Columns["CustomerName"].HeaderText =
                    "Khách hàng";
        }
    }

    public class ReportDto
    {
        public string Date { get; set; } = string.Empty;

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public string BestSeller { get; set; } = string.Empty;

        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class SummaryDto
    {
        public decimal Revenue { get; set; }
        public int InvoiceCount { get; set; }
        public int NewCustomers { get; set; }
        public int LowStockCount { get; set; }
    }

    public class RevenuePointDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;
    }

}
