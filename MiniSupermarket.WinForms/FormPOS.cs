using MiniSupermarket.WinForms.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormPOS : Form
    {
        private readonly List<CartItemDto> _cart = new();
        private readonly Label lblRedeemPoints = new() { Text = "Điểm muốn dùng (1 điểm = 1.000đ)", AutoSize = true };
        private readonly NumericUpDown nudRedeemPoints = new() { Minimum = 0, Maximum = 100000, ThousandsSeparator = true };

        public FormPOS()
        {
            InitializeComponent();
            pnlRight.Controls.Add(lblRedeemPoints);
            pnlRight.Controls.Add(nudRedeemPoints);
            nudRedeemPoints.ValueChanged += (s, e) => UpdateCartDisplay();
            SetupCartGrid();

            KeyPreview = true;
            KeyDown += FormPOS_KeyDown;

            pnlRight.AutoScroll = true;
            Load += (s, e) => ApplyResponsiveLayout();
            Resize += (s, e) => ApplyResponsiveLayout();
            txtCashReceived.BringToFront();
            txtCashReceived.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            };
            txtCashReceived.Enter += (s, e) => txtCashReceived.SelectAll();
            Shown += (s, e) => txtBarcode.Focus();
            UiTheme.Apply(this);
        }

        private void ApplyResponsiveLayout()
        {
            pnlLeft.Width = Math.Max(500, (int)(ClientSize.Width * 0.58));
            int w = Math.Max(200, pnlRight.ClientSize.Width - 50);
            int[] ys = { 10, 55, 80, 122, 145, 185, 208, 245, 270, 315, 340, 395, 418, 475, 530 };
            Control[] cs = { lblPaymentTitle, lblCustomerPhone, txtCustomerPhone, lblCustomerNameTitle, lblCustomerName,
                lblRedeemPoints, nudRedeemPoints, lblTotalTitle, lblTotalAmount, lblCashReceived, txtCashReceived,
                lblChangeTitle, lblChange, btnCheckout, btnClearCart };
            for (int i = 0; i < cs.Length; i++)
            {
                cs[i].Anchor = AnchorStyles.Top | AnchorStyles.Left;
                cs[i].Top = ys[i];
                cs[i].Left = 25;
                if (cs[i] is TextBox || cs[i] is Button || cs[i] is NumericUpDown || cs[i] == lblTotalAmount || cs[i] == lblChange) cs[i].Width = w;
            }
            lblPaymentTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTotalAmount.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
            lblTotalAmount.Height = 50;
            lblChange.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblChange.Height = 45;
            btnCheckout.Height = 50;
            btnClearCart.Height = 42;
            pnlRight.AutoScrollMinSize = new Size(0, 600);
        }

        private bool TryGetCash(out decimal cash)
        {
            string digits = new string(txtCashReceived.Text.Where(char.IsDigit).ToArray());
            return decimal.TryParse(digits, out cash);
        }

        private void SetupCartGrid()
        {
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductId",
                HeaderText = "Mã SP",
                Width = 80
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName",
                HeaderText = "Tên Sản Phẩm",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UnitPrice",
                HeaderText = "Đơn Giá",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0"
                }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "SL",
                Width = 70
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalPrice",
                HeaderText = "Thành Tiền",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0"
                }
            });
        }

        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;

            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                return;

            string barcode = txtBarcode.Text.Trim();
            txtBarcode.Clear();

            await AddProductToCartByBarcodeAsync(barcode);
        }

        private async Task AddProductToCartByBarcodeAsync(string barcode)
        {
            try
            {
                // Đã sửa: Thêm SessionManager. vào trước ApiClientService
                var product = await SessionManager.ApiClientService.Client.GetFromJsonAsync<ProductDto>(
                    $"products/barcode/{barcode}");

                if (product == null)
                {
                    MessageBox.Show(
                        "Không tìm thấy sản phẩm có mã vạch này!",
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtBarcode.Focus();
                    return;
                }

                var existingItem = _cart.FirstOrDefault(
                    c => c.ProductId == product.ProductId);

                if (existingItem != null)
                {
                    if (existingItem.Quantity >= product.StockQuantity)
                    {
                        MessageBox.Show(
                            "Số lượng sản phẩm trong kho không đủ!",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtBarcode.Focus();
                        return;
                    }

                    existingItem.Quantity++;
                }
                else
                {
                    if (product.StockQuantity <= 0)
                    {
                        MessageBox.Show(
                            "Sản phẩm đã hết hàng!",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtBarcode.Focus();
                        return;
                    }

                    _cart.Add(new CartItemDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        UnitPrice = product.Price,
                        Quantity = 1
                    });
                }

                UpdateCartDisplay();
                txtBarcode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối máy chủ: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateCartDisplay()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = _cart;

            decimal total = Math.Max(0m, _cart.Sum(x => x.TotalPrice) - nudRedeemPoints.Value * 1000m);

            lblTotalAmount.Text = $"{total:N0} đ";

            CalculateChange();
        }

        private bool _formatting;

        private void txtCashReceived_TextChanged(object sender, EventArgs e)
        {
            if (!_formatting && TryGetCash(out decimal v))
            {
                _formatting = true;
                txtCashReceived.Text = v.ToString("N0");
                txtCashReceived.SelectionStart = txtCashReceived.Text.Length;
                _formatting = false;
            }
            CalculateChange();
        }

        private void CalculateChange()
        {
            decimal total = Math.Max(0m, _cart.Sum(x => x.TotalPrice) - nudRedeemPoints.Value * 1000m);

            if (TryGetCash(out decimal cashReceived))
            {
                decimal change = cashReceived - total;

                if (change >= 0)
                {
                    lblChange.Text = $"{change:N0} đ";
                    lblChange.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblChange.Text = "Chưa đủ tiền!";
                    lblChange.ForeColor = Color.Red;
                }
            }
            else
            {
                lblChange.Text = "0 đ";
                lblChange.ForeColor = Color.Black;
            }
        }

        private async void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show(
                    "Giỏ hàng đang trống!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtBarcode.Focus();
                return;
            }

            if (!TryGetCash(out decimal cashReceived))
            {
                MessageBox.Show(
                    "Vui lòng nhập số tiền khách đưa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCashReceived.Focus();
                return;
            }

            decimal total = _cart.Sum(x => x.TotalPrice);

            if (cashReceived < total)
            {
                MessageBox.Show(
                    "Số tiền khách đưa chưa đủ!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCashReceived.Focus();
                return;
            }

            var orderRequest = new
            {
                CashierUsername = SessionManager.CurrentUsername,
                CustomerPhone = txtCustomerPhone.Text.Trim(),
                CashReceived = cashReceived,
                PointsToRedeem = (int)nudRedeemPoints.Value,
                Items = _cart.Select(i => new
                {
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice
                }).ToList()
            };
            var receiptItems = _cart.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList();
            string receiptCustomerName = lblCustomerName.Text;
            string receiptCustomerPhone = txtCustomerPhone.Text.Trim();
            int pointsRedeemed = (int)nudRedeemPoints.Value;

            try
            {
                // Đã sửa: Thêm SessionManager. vào trước ApiClientService
                var response = await SessionManager.ApiClientService.Client.PostAsJsonAsync(
                    "orders/checkout",
                    orderRequest);

                if (response.IsSuccessStatusCode)
                {
                    CheckoutReceiptDto? receipt = null;
                    try
                    {
                        receipt = await response.Content.ReadFromJsonAsync<CheckoutReceiptDto>();
                    }
                    catch
                    {
                        // Nếu API không trả phần thân hợp lệ, vẫn cho phép xuất hóa đơn từ giỏ hàng đã lưu.
                    }

                    receipt ??= new CheckoutReceiptDto
                    {
                        Subtotal = receiptItems.Sum(item => item.TotalPrice),
                        Discount = pointsRedeemed * 1000m,
                        Total = Math.Max(0m, receiptItems.Sum(item => item.TotalPrice) - pointsRedeemed * 1000m),
                        PointsRedeemed = pointsRedeemed,
                        Change = Math.Max(0m, cashReceived - Math.Max(0m, receiptItems.Sum(item => item.TotalPrice) - pointsRedeemed * 1000m))
                    };

                    DialogResult exportInvoice = MessageBox.Show(
                        "Thanh toán thành công. Bạn có muốn lưu hóa đơn Excel để xem hoặc in không?",
                        "Xuất hóa đơn",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (exportInvoice == DialogResult.Yes)
                        ExportCheckoutExcel(receipt, receiptItems, receiptCustomerName, receiptCustomerPhone, cashReceived);

                    _cart.Clear();

                    UpdateCartDisplay();

                    txtCashReceived.Clear();
                    txtCustomerPhone.Clear();
                    nudRedeemPoints.Value = 0;

                    lblCustomerName.Text = "Khách vãng lai";

                    txtBarcode.Focus();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Thanh toán thất bại từ máy chủ!\n\n" + error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối máy chủ: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ExportCheckoutExcel(
            CheckoutReceiptDto receipt,
            List<CartItemDto> items,
            string customerName,
            string customerPhone,
            decimal cashReceived)
        {
            string orderCode = string.IsNullOrWhiteSpace(receipt.OrderCode)
                ? $"HD{receipt.OrderId:D5}"
                : receipt.OrderCode;

            using var dialog = new SaveFileDialog
            {
                Title = "Lưu hóa đơn Excel",
                Filter = "Sổ làm việc Excel (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                AddExtension = true,
                FileName = $"hoa-don-{orderCode}.xlsx"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                byte[] workbook = CreateInvoiceWorkbook(
                    receipt,
                    items,
                    orderCode,
                    SessionManager.CurrentUsername,
                    customerName,
                    customerPhone,
                    cashReceived,
                    DateTime.Now);
                File.WriteAllBytes(dialog.FileName, workbook);
                MessageBox.Show("Đã lưu hóa đơn Excel.", "Xuất hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tạo tệp Excel: " + ex.Message, "Lỗi xuất hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static byte[] CreateInvoiceWorkbook(
            CheckoutReceiptDto receipt,
            List<CartItemDto> items,
            string orderCode,
            string cashier,
            string customerName,
            string customerPhone,
            decimal cashReceived,
            DateTime paidAt)
        {
            XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            XNamespace relationship = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            XNamespace packageRelationship = "http://schemas.openxmlformats.org/package/2006/relationships";
            XNamespace contentType = "http://schemas.openxmlformats.org/package/2006/content-types";

            var rows = new List<XElement>
            {
                ExcelRow(main, 1, new[] { ExcelTextCell(main, "A1", "HÓA ĐƠN BÁN HÀNG", 1) }, 30),
                ExcelRow(main, 3, new[]
                {
                    ExcelTextCell(main, "A3", "Mã hóa đơn", 3), ExcelTextCell(main, "B3", orderCode),
                    ExcelTextCell(main, "D3", "Ngày thanh toán", 3), ExcelTextCell(main, "E3", paidAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
                }),
                ExcelRow(main, 4, new[]
                {
                    ExcelTextCell(main, "A4", "Thu ngân", 3), ExcelTextCell(main, "B4", cashier),
                    ExcelTextCell(main, "D4", "Khách hàng", 3), ExcelTextCell(main, "E4", customerName)
                }),
                ExcelRow(main, 5, new[]
                {
                    ExcelTextCell(main, "A5", "Số điện thoại", 3), ExcelTextCell(main, "B5", customerPhone),
                    ExcelTextCell(main, "D5", "Điểm đã dùng", 3), ExcelNumberCell(main, "E5", receipt.PointsRedeemed)
                }),
                ExcelRow(main, 7, new[]
                {
                    ExcelTextCell(main, "A7", "Mã sản phẩm", 2),
                    ExcelTextCell(main, "B7", "Tên sản phẩm", 2),
                    ExcelTextCell(main, "C7", "Số lượng", 2),
                    ExcelTextCell(main, "D7", "Đơn giá", 2),
                    ExcelTextCell(main, "E7", "Thành tiền", 2)
                }, 24)
            };

            int rowNumber = 8;
            foreach (CartItemDto item in items)
            {
                rows.Add(ExcelRow(main, rowNumber, new[]
                {
                    ExcelNumberCell(main, $"A{rowNumber}", item.ProductId),
                    ExcelTextCell(main, $"B{rowNumber}", item.ProductName),
                    ExcelNumberCell(main, $"C{rowNumber}", item.Quantity),
                    ExcelNumberCell(main, $"D{rowNumber}", item.UnitPrice, 4),
                    ExcelNumberCell(main, $"E{rowNumber}", item.TotalPrice, 4)
                }));
                rowNumber++;
            }

            int totalStart = rowNumber + 1;
            AddInvoiceTotalRow(rows, main, totalStart, "Tạm tính", receipt.Subtotal);
            AddInvoiceTotalRow(rows, main, totalStart + 1, "Giảm giá", receipt.Discount);
            AddInvoiceTotalRow(rows, main, totalStart + 2, "Tổng thanh toán", receipt.Total, true);
            AddInvoiceTotalRow(rows, main, totalStart + 3, "Khách đưa", cashReceived);
            AddInvoiceTotalRow(rows, main, totalStart + 4, "Tiền thừa", receipt.Change);
            int lastRow = totalStart + 4;

            var worksheet = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(main + "worksheet",
                    new XElement(main + "dimension", new XAttribute("ref", $"A1:E{lastRow}")),
                    new XElement(main + "sheetViews",
                        new XElement(main + "sheetView", new XAttribute("workbookViewId", 0),
                            new XElement(main + "pane", new XAttribute("ySplit", 7), new XAttribute("topLeftCell", "A8"),
                                new XAttribute("activePane", "bottomLeft"), new XAttribute("state", "frozen")))),
                    new XElement(main + "sheetFormatPr", new XAttribute("defaultRowHeight", 19)),
                    new XElement(main + "cols",
                        new XElement(main + "col", new XAttribute("min", 1), new XAttribute("max", 1), new XAttribute("width", 16), new XAttribute("customWidth", 1)),
                        new XElement(main + "col", new XAttribute("min", 2), new XAttribute("max", 2), new XAttribute("width", 38), new XAttribute("customWidth", 1)),
                        new XElement(main + "col", new XAttribute("min", 3), new XAttribute("max", 3), new XAttribute("width", 13), new XAttribute("customWidth", 1)),
                        new XElement(main + "col", new XAttribute("min", 4), new XAttribute("max", 5), new XAttribute("width", 20), new XAttribute("customWidth", 1))),
                    new XElement(main + "sheetData", rows),
                    new XElement(main + "autoFilter", new XAttribute("ref", $"A7:E{rowNumber - 1}")),
                    new XElement(main + "mergeCells", new XAttribute("count", 1), new XElement(main + "mergeCell", new XAttribute("ref", "A1:E1"))),
                    new XElement(main + "pageMargins", new XAttribute("left", 0.3), new XAttribute("right", 0.3), new XAttribute("top", 0.5), new XAttribute("bottom", 0.5), new XAttribute("header", 0.2), new XAttribute("footer", 0.2))));

            var workbook = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(main + "workbook",
                    new XAttribute(XNamespace.Xmlns + "r", relationship),
                    new XElement(main + "sheets", new XElement(main + "sheet",
                        new XAttribute("name", "Hóa đơn"), new XAttribute("sheetId", 1), new XAttribute(relationship + "id", "rId1")))));

            var contentTypes = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(contentType + "Types",
                    new XElement(contentType + "Default", new XAttribute("Extension", "rels"), new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
                    new XElement(contentType + "Default", new XAttribute("Extension", "xml"), new XAttribute("ContentType", "application/xml")),
                    new XElement(contentType + "Override", new XAttribute("PartName", "/xl/workbook.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml")),
                    new XElement(contentType + "Override", new XAttribute("PartName", "/xl/worksheets/sheet1.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")),
                    new XElement(contentType + "Override", new XAttribute("PartName", "/xl/styles.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"))));

            var packageRelationships = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(packageRelationship + "Relationships",
                    new XElement(packageRelationship + "Relationship", new XAttribute("Id", "rId1"),
                        new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"),
                        new XAttribute("Target", "xl/workbook.xml"))));

            var workbookRelationships = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(packageRelationship + "Relationships",
                    new XElement(packageRelationship + "Relationship", new XAttribute("Id", "rId1"),
                        new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"),
                        new XAttribute("Target", "worksheets/sheet1.xml")),
                    new XElement(packageRelationship + "Relationship", new XAttribute("Id", "rId2"),
                        new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"),
                        new XAttribute("Target", "styles.xml"))));

            var styles = CreateInvoiceStyles(main);
            using var buffer = new MemoryStream();
            using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
            {
                WriteXmlEntry(archive, "[Content_Types].xml", contentTypes);
                WriteXmlEntry(archive, "_rels/.rels", packageRelationships);
                WriteXmlEntry(archive, "xl/workbook.xml", workbook);
                WriteXmlEntry(archive, "xl/_rels/workbook.xml.rels", workbookRelationships);
                WriteXmlEntry(archive, "xl/worksheets/sheet1.xml", worksheet);
                WriteXmlEntry(archive, "xl/styles.xml", styles);
            }
            return buffer.ToArray();
        }

        private static XElement ExcelRow(XNamespace main, int rowNumber, IEnumerable<XElement> cells, double? height = null)
        {
            var row = new XElement(main + "row", new XAttribute("r", rowNumber), cells);
            if (height.HasValue)
            {
                row.SetAttributeValue("ht", height.Value);
                row.SetAttributeValue("customHeight", 1);
            }
            return row;
        }

        private static XElement ExcelTextCell(XNamespace main, string reference, string? value, int style = 0)
        {
            var cell = new XElement(main + "c",
                new XAttribute("r", reference),
                new XAttribute("t", "inlineStr"),
                new XElement(main + "is", new XElement(main + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), value ?? string.Empty)));
            if (style > 0)
                cell.SetAttributeValue("s", style);
            return cell;
        }

        private static XElement ExcelNumberCell(XNamespace main, string reference, decimal value, int style = 0)
        {
            var cell = new XElement(main + "c",
                new XAttribute("r", reference),
                new XAttribute("t", "n"),
                new XElement(main + "v", value.ToString(CultureInfo.InvariantCulture)));
            if (style > 0)
                cell.SetAttributeValue("s", style);
            return cell;
        }

        private static XElement ExcelNumberCell(XNamespace main, string reference, int value, int style = 0)
        {
            return ExcelNumberCell(main, reference, (decimal)value, style);
        }

        private static void AddInvoiceTotalRow(List<XElement> rows, XNamespace main, int rowNumber, string label, decimal value, bool emphasize = false)
        {
            int style = emphasize ? 5 : 3;
            rows.Add(ExcelRow(main, rowNumber, new[]
            {
                ExcelTextCell(main, $"D{rowNumber}", label, style),
                ExcelNumberCell(main, $"E{rowNumber}", value, emphasize ? 5 : 4)
            }));
        }

        private static XDocument CreateInvoiceStyles(XNamespace main)
        {
            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(main + "styleSheet",
                    new XElement(main + "numFmts", new XAttribute("count", 1),
                        new XElement(main + "numFmt", new XAttribute("numFmtId", 164), new XAttribute("formatCode", "#,##0"))),
                    new XElement(main + "fonts", new XAttribute("count", 4),
                        new XElement(main + "font", new XElement(main + "sz", new XAttribute("val", 11)), new XElement(main + "name", new XAttribute("val", "Calibri"))),
                        new XElement(main + "font", new XElement(main + "b"), new XElement(main + "sz", new XAttribute("val", 16)), new XElement(main + "color", new XAttribute("rgb", "FFFFFFFF")), new XElement(main + "name", new XAttribute("val", "Calibri"))),
                        new XElement(main + "font", new XElement(main + "b"), new XElement(main + "sz", new XAttribute("val", 11)), new XElement(main + "color", new XAttribute("rgb", "FFFFFFFF")), new XElement(main + "name", new XAttribute("val", "Calibri"))),
                        new XElement(main + "font", new XElement(main + "b"), new XElement(main + "sz", new XAttribute("val", 11)), new XElement(main + "name", new XAttribute("val", "Calibri")))),
                    new XElement(main + "fills", new XAttribute("count", 3),
                        new XElement(main + "fill", new XElement(main + "patternFill", new XAttribute("patternType", "none"))),
                        new XElement(main + "fill", new XElement(main + "patternFill", new XAttribute("patternType", "gray125"))),
                        new XElement(main + "fill", new XElement(main + "patternFill", new XAttribute("patternType", "solid"), new XElement(main + "fgColor", new XAttribute("rgb", "FF2563EB")), new XElement(main + "bgColor", new XAttribute("indexed", 64))))),
                    new XElement(main + "borders", new XAttribute("count", 1), new XElement(main + "border",
                        new XElement(main + "left"), new XElement(main + "right"), new XElement(main + "top"), new XElement(main + "bottom"), new XElement(main + "diagonal"))),
                    new XElement(main + "cellStyleXfs", new XAttribute("count", 1), new XElement(main + "xf", new XAttribute("numFmtId", 0), new XAttribute("fontId", 0), new XAttribute("fillId", 0), new XAttribute("borderId", 0))),
                    new XElement(main + "cellXfs", new XAttribute("count", 6),
                        new XElement(main + "xf", new XAttribute("numFmtId", 0), new XAttribute("fontId", 0), new XAttribute("fillId", 0), new XAttribute("borderId", 0), new XAttribute("xfId", 0)),
                        new XElement(main + "xf", new XAttribute("numFmtId", 0), new XAttribute("fontId", 1), new XAttribute("fillId", 2), new XAttribute("borderId", 0), new XAttribute("xfId", 0), new XAttribute("applyAlignment", 1), new XElement(main + "alignment", new XAttribute("horizontal", "center"), new XAttribute("vertical", "center"))),
                        new XElement(main + "xf", new XAttribute("numFmtId", 0), new XAttribute("fontId", 2), new XAttribute("fillId", 2), new XAttribute("borderId", 0), new XAttribute("xfId", 0), new XAttribute("applyAlignment", 1), new XElement(main + "alignment", new XAttribute("horizontal", "center"), new XAttribute("vertical", "center"))),
                        new XElement(main + "xf", new XAttribute("numFmtId", 0), new XAttribute("fontId", 3), new XAttribute("fillId", 0), new XAttribute("borderId", 0), new XAttribute("xfId", 0)),
                        new XElement(main + "xf", new XAttribute("numFmtId", 164), new XAttribute("fontId", 0), new XAttribute("fillId", 0), new XAttribute("borderId", 0), new XAttribute("xfId", 0), new XAttribute("applyNumberFormat", 1)),
                        new XElement(main + "xf", new XAttribute("numFmtId", 164), new XAttribute("fontId", 3), new XAttribute("fillId", 0), new XAttribute("borderId", 0), new XAttribute("xfId", 0), new XAttribute("applyNumberFormat", 1))),
                    new XElement(main + "cellStyles", new XAttribute("count", 1), new XElement(main + "cellStyle", new XAttribute("name", "Normal"), new XAttribute("xfId", 0), new XAttribute("builtinId", 0)))));
        }

        private static void WriteXmlEntry(ZipArchive archive, string path, XDocument document)
        {
            ZipArchiveEntry entry = archive.CreateEntry(path, CompressionLevel.Optimal);
            using Stream stream = entry.Open();
            using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            document.Save(writer, SaveOptions.DisableFormatting);
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
                return;

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn hủy toàn bộ giỏ hàng?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            _cart.Clear();

            UpdateCartDisplay();

            txtCashReceived.Clear();
            txtCustomerPhone.Clear();
            nudRedeemPoints.Value = 0;

            lblCustomerName.Text = "Khách vãng lai";

            txtBarcode.Focus();
        }

        private void FormPOS_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                btnCheckout.PerformClick();
            }
        }
    }

    // Class CartItemDto dùng riêng cho giỏ hàng
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }

    public class CheckoutReceiptDto
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int PointsRedeemed { get; set; }
        public decimal Change { get; set; }
    }

    // Đã xóa class ProductDto bị trùng lặp ở đây
}
