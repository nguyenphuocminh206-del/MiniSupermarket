using static MiniSupermarket.WinForms.Models.ProductDto;
using static MiniSupermarket.WinForms.SessionManager;

using MiniSupermarket.WinForms.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormProductManagement : Form
    {
        private Panel panelTop = null!;
        private Label lblSearchBarcode = null!;
        private TextBox txtSearchBarcode = null!;
        private Label lblFilterCategory = null!;
        private ComboBox cboFilterCategory = null!;
        private Button btnSearch = null!;

        private DataGridView dgvProducts = null!;

        private GroupBox grpDetails = null!;
        private Label lblId = null!;
        private Label lblBarcode = null!;
        private Label lblProductName = null!;
        private Label lblPrice = null!;
        private Label lblStock = null!;
        private Label lblCategory = null!;

        private TextBox txtId = null!;
        private TextBox txtBarcode = null!;
        private TextBox txtProductName = null!;
        private NumericUpDown nudPrice = null!;
        private NumericUpDown nudStock = null!;
        private ComboBox cboCategory = null!;

        private Button btnLoad = null!;
        private Button btnAdd = null!;
        private Button btnUpdate = null!;
        private Button btnDelete = null!;
        // ===== Phân trang Server-side =====
        private Button btnPrev = null!;
        private Button btnNext = null!;
        private Label lblPaging = null!;
        private int _currentPage = 1;
        private readonly int _pageSize = 5;
        private int _totalPages = 1;

        public FormProductManagement()
        {
            InitializeComponent();
            BuildUI();
            UiTheme.Apply(this);
            Resize += (_, _) => LayoutResponsive();
            LayoutResponsive();
        }

        private void LayoutResponsive()
        {
            if (panelTop == null || dgvProducts == null || grpDetails == null || ClientSize.Width <= 0)
                return;

            const int margin = 20;
            const int gap = 18;
            int pageWidth = Math.Max(400, ClientSize.Width - margin * 2);
            panelTop.SetBounds(margin, 15, pageWidth, 60);

            int detailWidth = Math.Clamp((pageWidth - gap) * 30 / 100, 300, 380);
            int detailLeft = ClientSize.Width - margin - detailWidth;
            grpDetails.SetBounds(
                detailLeft,
                90,
                detailWidth,
                Math.Max(400, ClientSize.Height - 160));

            int listWidth = Math.Max(360, detailLeft - gap - margin);
            int gridBottom = Math.Max(250, ClientSize.Height - 110);
            dgvProducts.SetBounds(margin, 90, listWidth, gridBottom - 90);

            int pagingY = ClientSize.Height - 102;
            btnPrev.SetBounds(margin, pagingY, 130, 35);
            lblPaging.SetBounds(margin + 140, pagingY, Math.Max(160, listWidth - 298), 35);
            btnNext.SetBounds(margin + listWidth - 130, pagingY, 130, 35);

            const int searchLabelWidth = 80;
            const int categoryLabelWidth = 90;
            const int searchButtonWidth = 120;
            const int controlGap = 12;
            int flexibleWidth = Math.Max(
                420,
                panelTop.ClientSize.Width - searchLabelWidth - categoryLabelWidth - searchButtonWidth - controlGap * 3);
            int keywordWidth = flexibleWidth * 58 / 100;
            int categoryWidth = flexibleWidth - keywordWidth;

            lblSearchBarcode.SetBounds(0, 10, searchLabelWidth, 25);
            txtSearchBarcode.SetBounds(searchLabelWidth, 7, keywordWidth, 30);
            int categoryLabelLeft = searchLabelWidth + keywordWidth + controlGap;
            lblFilterCategory.SetBounds(categoryLabelLeft, 10, categoryLabelWidth, 25);
            int categoryLeft = categoryLabelLeft + categoryLabelWidth;
            cboFilterCategory.SetBounds(categoryLeft, 7, categoryWidth, 30);
            btnSearch.SetBounds(categoryLeft + categoryWidth + controlGap, 5, searchButtonWidth, 35);
        }

        private void BuildUI()
        {
            this.Text = "QUẢN LÝ SẢN PHẨM & KHO HÀNG";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            panelTop = new Panel
            {
                Location = new Point(20, 15),
                Size = new Size(1040, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };

            lblSearchBarcode = new Label
            {
                Text = "Từ khóa:",
                Location = new Point(0, 10),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };

            txtSearchBarcode = new TextBox
            {
                Location = new Point(80, 7),
                Size = new Size(250, 30)
            };

            lblFilterCategory = new Label
            {
                Text = "Nhóm hàng:",
                Location = new Point(350, 10),
                Size = new Size(90, 25),
                Font = new Font("Segoe UI", 10)
            };

            cboFilterCategory = new ComboBox
            {
                Location = new Point(440, 7),
                Size = new Size(220, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(680, 5),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Tìm kiếm / đổi nhóm hàng luôn quay về trang đầu tiên
            btnSearch.Click += async (s, e) =>
            {
                _currentPage = 1;
                await LoadProductsAsync();
            };

            txtSearchBarcode.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _currentPage = 1; await LoadProductsAsync(); }
            };
            cboFilterCategory.SelectionChangeCommitted += async (s, e) => { _currentPage = 1; await LoadProductsAsync(); };

            panelTop.Controls.Add(lblSearchBarcode);
            panelTop.Controls.Add(txtSearchBarcode);
            panelTop.Controls.Add(lblFilterCategory);
            panelTop.Controls.Add(cboFilterCategory);
            panelTop.Controls.Add(btnSearch);

            dgvProducts = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(700, 450),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductId",
                HeaderText = "Mã SP",
                DataPropertyName = "ProductId",
                FillWeight = 40
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Barcode",
                HeaderText = "Mã vạch",
                DataPropertyName = "Barcode"
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "ProductName"
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Đơn giá",
                DataPropertyName = "Price",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StockQuantity",
                HeaderText = "Tồn kho",
                DataPropertyName = "StockQuantity"
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Nhóm hàng",
                DataPropertyName = "CategoryName"
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CategoryId",
                DataPropertyName = "CategoryId",
                Visible = false
            });

            dgvProducts.CellClick += dgvProducts_CellClick;
            dgvProducts.CellFormatting += dgvProducts_CellFormatting;

            btnPrev = new Button
            {
                Text = "<< Trang trước",
                Location = new Point(20, 548),
                Size = new Size(130, 35),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            lblPaging = new Label
            {
                Text = "Trang 1 / 1 (Tổng: 0 SP)",
                Location = new Point(160, 548),
                Size = new Size(400, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnNext = new Button
            {
                Text = "Trang sau >>",
                Location = new Point(570, 548),
                Size = new Size(130, 35),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;

            grpDetails = new GroupBox
            {
                Text = "THÔNG TIN SẢN PHẨM",
                Location = new Point(740, 90),
                Size = new Size(320, 490),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblId = new Label
            {
                Text = "ProductId:",
                Location = new Point(20, 35),
                Size = new Size(100, 25)
            };

            txtId = new TextBox
            {
                Location = new Point(120, 32),
                Size = new Size(175, 28),
                ReadOnly = true
            };

            lblBarcode = new Label
            {
                Text = "Mã vạch:",
                Location = new Point(20, 75),
                Size = new Size(100, 25)
            };

            txtBarcode = new TextBox
            {
                Location = new Point(120, 72),
                Size = new Size(175, 28)
            };

            lblProductName = new Label
            {
                Text = "Tên sản phẩm:",
                Location = new Point(20, 115),
                Size = new Size(100, 25)
            };

            txtProductName = new TextBox
            {
                Location = new Point(120, 112),
                Size = new Size(175, 28)
            };

            lblPrice = new Label
            {
                Text = "Đơn giá:",
                Location = new Point(20, 155),
                Size = new Size(100, 25)
            };

            nudPrice = new NumericUpDown
            {
                Location = new Point(120, 152),
                Size = new Size(175, 28),
                Minimum = 0,
                Maximum = 1000000000,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            lblStock = new Label
            {
                Text = "Tồn kho:",
                Location = new Point(20, 195),
                Size = new Size(100, 25)
            };

            nudStock = new NumericUpDown
            {
                Location = new Point(120, 192),
                Size = new Size(175, 28),
                Minimum = 0,
                Maximum = 1000000
            };

            lblCategory = new Label
            {
                Text = "Nhóm hàng:",
                Location = new Point(20, 235),
                Size = new Size(100, 25)
            };

            cboCategory = new ComboBox
            {
                Location = new Point(120, 232),
                Size = new Size(175, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnLoad = new Button
            {
                Text = "Tải lại",
                Location = new Point(20, 280),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat
            };

            btnAdd = new Button
            {
                Text = "Thêm",
                Location = new Point(165, 280),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnUpdate = new Button
            {
                Text = "Cập nhật",
                Location = new Point(20, 335),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnDelete = new Button
            {
                Text = "Xóa",
                Location = new Point(165, 335),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLoad.Click += async (s, e) =>
            {
                await LoadProductsAsync();
            };

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;

            grpDetails.Controls.Add(lblId);
            grpDetails.Controls.Add(txtId);
            grpDetails.Controls.Add(lblBarcode);
            grpDetails.Controls.Add(txtBarcode);
            grpDetails.Controls.Add(lblProductName);
            grpDetails.Controls.Add(txtProductName);
            grpDetails.Controls.Add(lblPrice);
            grpDetails.Controls.Add(nudPrice);
            grpDetails.Controls.Add(lblStock);
            grpDetails.Controls.Add(nudStock);
            grpDetails.Controls.Add(lblCategory);
            grpDetails.Controls.Add(cboCategory);
            grpDetails.Controls.Add(btnLoad);
            grpDetails.Controls.Add(btnAdd);
            grpDetails.Controls.Add(btnUpdate);
            grpDetails.Controls.Add(btnDelete);

            this.Controls.Add(panelTop);
            this.Controls.Add(dgvProducts);
            this.Controls.Add(btnPrev);
            this.Controls.Add(lblPaging);
            this.Controls.Add(btnNext);

            this.Controls.Add(grpDetails);

            this.Load += FormProductManagement_Load;
        }

        private async void FormProductManagement_Load(object? sender, EventArgs e)
        {
            await LoadCategoriesToComboAsync();
            await LoadProductsAsync();
        }

        private async Task LoadCategoriesToComboAsync()
        {
            try
            {
                var categories = await ApiClientService.Client.GetFromJsonAsync<List<CategoryDto>>("categories");

                categories ??= new List<CategoryDto>();

                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryId";
                cboCategory.DataSource = new List<CategoryDto>(categories);

                var filter = new List<CategoryDto> { new CategoryDto { CategoryId = 0, CategoryName = "-- Tất cả --" } };
                filter.AddRange(categories);
                cboFilterCategory.DisplayMember = "CategoryName";
                cboFilterCategory.ValueMember = "CategoryId";
                cboFilterCategory.DataSource = filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                string search = Uri.EscapeDataString(txtSearchBarcode.Text.Trim());
                string url = $"products/paged?keyword={search}&pageNumber={_currentPage}&pageSize={_pageSize}";
                if (cboFilterCategory.SelectedValue is int cid && cid > 0)
                    url += "&categoryId=" + cid;

                var result = await ApiClientService.Client.GetFromJsonAsync<PagedViewResult<ProductItemViewDto>>(url);

                if (result == null) return;

                // Xóa hết dòng cuối của trang cuối -> lùi về trang cuối còn dữ liệu
                if (result.Items.Count == 0 && result.TotalPages > 0 && _currentPage > result.TotalPages)
                {
                    _currentPage = result.TotalPages;
                    await LoadProductsAsync();
                    return;
                }

                _currentPage = result.CurrentPage;
                _totalPages = result.TotalPages == 0 ? 1 : result.TotalPages;

                dgvProducts.DataSource = null;
                dgvProducts.DataSource = result.Items;
                UpdatePagingControls(result.TotalItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePagingControls(int totalItems)
        {
            lblPaging.Text = $"Trang {_currentPage} / {_totalPages} (Tổng: {totalItems} SP)";
            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
        }

        private void dgvProducts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 ||
                dgvProducts.Columns[e.ColumnIndex].DataPropertyName != "StockQuantity")
                return;

            var style = e.CellStyle;
            if (style != null && int.TryParse(Convert.ToString(e.Value), out int stock) && stock <= 10)
            {
                style.BackColor = Color.MistyRose;
                style.ForeColor = Color.DarkRed;
                style.Font = new Font(dgvProducts.Font, FontStyle.Bold);
            }
        }

        private async void btnPrev_Click(object? sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadProductsAsync();
            }
        }

        private async void btnNext_Click(object? sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadProductsAsync();
            }
        }

        private void dgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvProducts.Rows[e.RowIndex];

                txtId.Text = row.Cells["ProductId"].Value?.ToString();
                txtBarcode.Text = row.Cells["Barcode"].Value?.ToString();
                txtProductName.Text = row.Cells["ProductName"].Value?.ToString();

                nudPrice.Value = Convert.ToDecimal(row.Cells["Price"].Value ?? 0);
                nudStock.Value = Convert.ToInt32(row.Cells["StockQuantity"].Value ?? 0);
                if (row.Cells["CategoryId"].Value != null)
                    cboCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value);
            }
        }

        private async void btnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            var newProd = new
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = Convert.ToInt32(cboCategory.SelectedValue)
            };

            var res = await ApiClientService.Client.PostAsJsonAsync("products", newProd);

            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadProductsAsync();
                ClearInputs();
            }
            else await ShowErrorAsync(res);
        }

        private async void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
                return;

            if (!ValidateInputs()) return;
            int id = int.Parse(txtId.Text);

            var updateProd = new
            {
                ProductId = id,
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = Convert.ToInt32(cboCategory.SelectedValue)
            };

            var res = await ApiClientService.Client.PutAsJsonAsync($"products/{id}", updateProd);

            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadProductsAsync();
            }
            else await ShowErrorAsync(res);
        }

        private async void btnDelete_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text))
                return;

            int id = int.Parse(txtId.Text);

            if (MessageBox.Show($"Xác nhận xóa sản phẩm ID = {id}?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var res = await ApiClientService.Client.DeleteAsync($"products/{id}");

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Đã xóa sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync();
                    ClearInputs();
                }
                else await ShowErrorAsync(res);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Mã vạch và tên sản phẩm không được trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private static async Task ShowErrorAsync(System.Net.Http.HttpResponseMessage res)
        {
            string msg = await res.Content.ReadAsStringAsync();
            MessageBox.Show($"Thao tác thất bại ({(int)res.StatusCode}): {msg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
        }
    }

}
