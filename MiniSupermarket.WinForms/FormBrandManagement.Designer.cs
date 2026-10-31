using System.Drawing;

namespace MiniSupermarket.WinForms
{
    partial class FormBrandManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            rootLayout = new TableLayoutPanel();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlSearch = new Panel();
            lblSearch = new Label();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnReload = new Button();
            bodyLayout = new TableLayoutPanel();
            pnlList = new Panel();
            listLayout = new TableLayoutPanel();
            pnlListHeader = new Panel();
            lblListTitle = new Label();
            lblCount = new Label();
            gridHost = new Panel();
            dgvBrands = new DataGridView();
            pnlDetails = new Panel();
            detailLayout = new TableLayoutPanel();
            lblDetailsTitle = new Label();
            lblId = new Label();
            txtId = new TextBox();
            lblName = new Label();
            txtName = new TextBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            rootLayout.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlSearch.SuspendLayout();
            bodyLayout.SuspendLayout();
            pnlList.SuspendLayout();
            listLayout.SuspendLayout();
            pnlListHeader.SuspendLayout();
            gridHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBrands).BeginInit();
            pnlDetails.SuspendLayout();
            detailLayout.SuspendLayout();
            SuspendLayout();
            //
            // rootLayout
            //
            rootLayout.BackColor = Color.FromArgb(246, 248, 252);
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(pnlHeader, 0, 0);
            rootLayout.Controls.Add(pnlSearch, 0, 1);
            rootLayout.Controls.Add(bodyLayout, 0, 2);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Padding = new Padding(24);
            rootLayout.RowCount = 3;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            //
            // pnlHeader
            //
            pnlHeader.BackColor = Color.Transparent;
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.Margin = new Padding(0, 0, 0, 12);
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 21F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(292, 38);
            lblTitle.Text = "Quản lý thương hiệu";
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubtitle.Location = new Point(2, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(278, 17);
            lblSubtitle.Text = "Quản lý danh sách và thông tin các thương hiệu";
            //
            // pnlSearch
            //
            pnlSearch.BackColor = Color.White;
            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(btnReload);
            pnlSearch.Dock = DockStyle.Fill;
            pnlSearch.Margin = new Padding(0, 0, 0, 16);
            pnlSearch.Padding = new Padding(16, 12, 16, 12);
            //
            // lblSearch
            //
            lblSearch.AutoSize = true;
            lblSearch.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblSearch.ForeColor = Color.FromArgb(71, 85, 105);
            lblSearch.Location = new Point(18, 9);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(96, 15);
            lblSearch.Text = "TÌM THƯƠNG HIỆU";
            //
            // txtSearch
            //
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtSearch.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtSearch.Location = new Point(18, 32);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Nhập tên thương hiệu...";
            txtSearch.Size = new Size(340, 25);
            txtSearch.TabIndex = 0;
            //
            // btnSearch
            //
            btnSearch.BackColor = Color.FromArgb(37, 99, 235);
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(370, 30);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(112, 32);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            //
            // btnReload
            //
            btnReload.BackColor = Color.FromArgb(241, 245, 249);
            btnReload.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnReload.FlatStyle = FlatStyle.Flat;
            btnReload.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            btnReload.ForeColor = Color.FromArgb(51, 65, 85);
            btnReload.Location = new Point(492, 30);
            btnReload.Name = "btnReload";
            btnReload.Size = new Size(104, 32);
            btnReload.TabIndex = 2;
            btnReload.Text = "↻  Tải lại";
            btnReload.UseVisualStyleBackColor = false;
            btnReload.Click += btnReload_Click;
            //
            // bodyLayout
            //
            bodyLayout.ColumnCount = 2;
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            bodyLayout.Controls.Add(pnlList, 0, 0);
            bodyLayout.Controls.Add(pnlDetails, 1, 0);
            bodyLayout.Dock = DockStyle.Fill;
            bodyLayout.Margin = new Padding(0);
            bodyLayout.RowCount = 1;
            bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            //
            // pnlList
            //
            pnlList.BackColor = Color.White;
            pnlList.Controls.Add(listLayout);
            pnlList.Dock = DockStyle.Fill;
            pnlList.Margin = new Padding(0, 0, 16, 0);
            //
            // listLayout
            //
            listLayout.ColumnCount = 1;
            listLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            listLayout.Controls.Add(pnlListHeader, 0, 0);
            listLayout.Controls.Add(gridHost, 0, 1);
            listLayout.Dock = DockStyle.Fill;
            listLayout.Margin = new Padding(0);
            listLayout.RowCount = 2;
            listLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            listLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            //
            // pnlListHeader
            //
            pnlListHeader.Controls.Add(lblListTitle);
            pnlListHeader.Controls.Add(lblCount);
            pnlListHeader.Dock = DockStyle.Fill;
            pnlListHeader.Padding = new Padding(18, 0, 18, 0);
            //
            // lblListTitle
            //
            lblListTitle.AutoSize = true;
            lblListTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblListTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblListTitle.Location = new Point(18, 17);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(160, 21);
            lblListTitle.Text = "Danh sách thương hiệu";
            //
            // lblCount
            //
            lblCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCount.AutoSize = true;
            lblCount.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblCount.ForeColor = Color.FromArgb(100, 116, 139);
            lblCount.Location = new Point(640, 20);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(70, 15);
            lblCount.Text = "0 thương hiệu";
            //
            // gridHost
            //
            gridHost.Controls.Add(dgvBrands);
            gridHost.Dock = DockStyle.Fill;
            gridHost.Padding = new Padding(14, 0, 14, 14);
            //
            // dgvBrands
            //
            dgvBrands.AllowUserToAddRows = false;
            dgvBrands.AllowUserToDeleteRows = false;
            dgvBrands.AllowUserToResizeRows = false;
            dgvBrands.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBrands.BackgroundColor = Color.White;
            dgvBrands.BorderStyle = BorderStyle.None;
            dgvBrands.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvBrands.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvBrands.ColumnHeadersHeight = 42;
            dgvBrands.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvBrands.Dock = DockStyle.Fill;
            dgvBrands.EnableHeadersVisualStyles = false;
            dgvBrands.GridColor = Color.FromArgb(226, 232, 240);
            dgvBrands.MultiSelect = false;
            dgvBrands.Name = "dgvBrands";
            dgvBrands.ReadOnly = true;
            dgvBrands.RowHeadersVisible = false;
            dgvBrands.RowTemplate.Height = 40;
            dgvBrands.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBrands.TabIndex = 0;
            dgvBrands.CellClick += dgvBrands_CellClick;
            dgvBrands.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvBrands.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(51, 65, 85);
            dgvBrands.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            dgvBrands.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgvBrands.DefaultCellStyle.BackColor = Color.White;
            dgvBrands.DefaultCellStyle.ForeColor = Color.FromArgb(51, 65, 85);
            dgvBrands.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvBrands.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dgvBrands.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgvBrands.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);
            //
            // pnlDetails
            //
            pnlDetails.BackColor = Color.White;
            pnlDetails.Controls.Add(detailLayout);
            pnlDetails.Dock = DockStyle.Fill;
            pnlDetails.Margin = new Padding(0);
            //
            // detailLayout
            //
            detailLayout.ColumnCount = 1;
            detailLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            detailLayout.Controls.Add(lblDetailsTitle, 0, 0);
            detailLayout.Controls.Add(lblId, 0, 1);
            detailLayout.Controls.Add(txtId, 0, 2);
            detailLayout.Controls.Add(lblName, 0, 3);
            detailLayout.Controls.Add(txtName, 0, 4);
            detailLayout.Controls.Add(lblDescription, 0, 5);
            detailLayout.Controls.Add(txtDescription, 0, 6);
            detailLayout.Controls.Add(btnAdd, 0, 7);
            detailLayout.Controls.Add(btnUpdate, 0, 8);
            detailLayout.Controls.Add(btnDelete, 0, 9);
            detailLayout.Dock = DockStyle.Fill;
            detailLayout.Padding = new Padding(20);
            detailLayout.RowCount = 10;
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            //
            // lblDetailsTitle
            //
            lblDetailsTitle.Dock = DockStyle.Fill;
            lblDetailsTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblDetailsTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblDetailsTitle.Name = "lblDetailsTitle";
            lblDetailsTitle.Text = "Thông tin thương hiệu";
            lblDetailsTitle.TextAlign = ContentAlignment.MiddleLeft;
            //
            // field labels and inputs
            //
            lblId.Dock = DockStyle.Fill;
            lblId.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblId.ForeColor = Color.FromArgb(71, 85, 105);
            lblId.Text = "Mã thương hiệu";
            lblId.TextAlign = ContentAlignment.BottomLeft;
            txtId.Dock = DockStyle.Fill;
            txtId.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtId.ReadOnly = true;
            txtId.BackColor = Color.FromArgb(248, 250, 252);
            txtId.BorderStyle = BorderStyle.FixedSingle;
            txtId.Margin = new Padding(0, 3, 0, 3);
            lblName.Dock = DockStyle.Fill;
            lblName.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblName.ForeColor = Color.FromArgb(71, 85, 105);
            lblName.Text = "Tên thương hiệu";
            lblName.TextAlign = ContentAlignment.BottomLeft;
            txtName.Dock = DockStyle.Fill;
            txtName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Margin = new Padding(0, 3, 0, 3);
            lblDescription.Dock = DockStyle.Fill;
            lblDescription.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblDescription.ForeColor = Color.FromArgb(71, 85, 105);
            lblDescription.Text = "Mô tả";
            lblDescription.TextAlign = ContentAlignment.BottomLeft;
            txtDescription.Dock = DockStyle.Fill;
            txtDescription.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.Margin = new Padding(0, 3, 0, 10);
            //
            // action buttons
            //
            btnAdd.BackColor = Color.FromArgb(37, 99, 235);
            btnAdd.Dock = DockStyle.Fill;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            btnAdd.ForeColor = Color.White;
            btnAdd.Margin = new Padding(0, 3, 0, 5);
            btnAdd.Name = "btnAdd";
            btnAdd.Text = "+  Thêm thương hiệu";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            btnUpdate.BackColor = Color.FromArgb(16, 185, 129);
            btnUpdate.Dock = DockStyle.Fill;
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Margin = new Padding(0, 3, 0, 5);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Text = "Lưu thay đổi";
            btnUpdate.UseVisualStyleBackColor = false;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.BackColor = Color.FromArgb(254, 242, 242);
            btnDelete.Dock = DockStyle.Fill;
            btnDelete.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            btnDelete.ForeColor = Color.FromArgb(185, 28, 28);
            btnDelete.Margin = new Padding(0, 3, 0, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Text = "Xóa thương hiệu";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            //
            // FormBrandManagement
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 252);
            ClientSize = new Size(1180, 740);
            Controls.Add(rootLayout);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            MinimumSize = new Size(840, 540);
            Name = "FormBrandManagement";
            Text = "Quản lý thương hiệu";
            Load += FormBrandManagement_Load;
            rootLayout.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            bodyLayout.ResumeLayout(false);
            pnlList.ResumeLayout(false);
            listLayout.ResumeLayout(false);
            pnlListHeader.ResumeLayout(false);
            pnlListHeader.PerformLayout();
            gridHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBrands).EndInit();
            pnlDetails.ResumeLayout(false);
            detailLayout.ResumeLayout(false);
            detailLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel rootLayout;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlSearch;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnReload;
        private TableLayoutPanel bodyLayout;
        private Panel pnlList;
        private TableLayoutPanel listLayout;
        private Panel pnlListHeader;
        private Label lblListTitle;
        private Label lblCount;
        private Panel gridHost;
        private DataGridView dgvBrands;
        private Panel pnlDetails;
        private TableLayoutPanel detailLayout;
        private Label lblDetailsTitle;
        private Label lblId;
        private TextBox txtId;
        private Label lblName;
        private TextBox txtName;
        private Label lblDescription;
        private TextBox txtDescription;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
    }
}
