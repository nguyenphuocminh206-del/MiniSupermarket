namespace MiniSupermarket.WinForms
{
    partial class FormCategoryManagement
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            txtKeyword = new TextBox();
            btnSearch = new Button();
            btnLoad = new Button();

            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();

            dgvCategories = new DataGridView();

            btnDelete = new Button();
            btnUpdate = new Button();
            btnAdd = new Button();

            txtDescription = new TextBox();
            txtCategoryName = new TextBox();
            txtId = new TextBox();

            label3 = new Label();
            label2 = new Label();
            label1 = new Label();

            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();

            SuspendLayout();

            txtKeyword.Location = new Point(6, 22);
            txtKeyword.Name = "txtKeyword";
            txtKeyword.Size = new Size(307, 23);
            txtKeyword.TabIndex = 1;
            txtKeyword.Text = "Nhập từ khóa...";

            btnSearch.Location = new Point(325, 22);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;

            btnLoad.Location = new Point(406, 22);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 3;
            btnLoad.Text = "Tải lại";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;

            groupBox1.Controls.Add(txtKeyword);
            groupBox1.Controls.Add(btnSearch);
            groupBox1.Controls.Add(btnLoad);
            groupBox1.Location = new Point(12, 35);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(495, 57);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Tìm kiếm";

            groupBox2.Controls.Add(dgvCategories);
            groupBox2.Location = new Point(12, 98);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(495, 286);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Danh sách nhóm hàng";

            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.AllowUserToDeleteRows = false;
            dgvCategories.AllowUserToResizeRows = false;
            dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCategories.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategories.Location = new Point(6, 22);
            dgvCategories.Name = "dgvCategories";
            dgvCategories.ReadOnly = true;
            dgvCategories.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.MultiSelect = false;
            dgvCategories.Size = new Size(483, 258);
            dgvCategories.TabIndex = 0;
            dgvCategories.CellClick += dgvCategories_CellClick;

            groupBox3.Controls.Add(btnDelete);
            groupBox3.Controls.Add(btnUpdate);
            groupBox3.Controls.Add(btnAdd);
            groupBox3.Controls.Add(txtDescription);
            groupBox3.Controls.Add(txtCategoryName);
            groupBox3.Controls.Add(txtId);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label1);

            groupBox3.Location = new Point(513, 98);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(275, 286);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Thông tin nhóm hàng";

            btnDelete.Location = new Point(181, 181);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(88, 23);
            btnDelete.TabIndex = 8;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            btnUpdate.Location = new Point(100, 181);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;

            btnAdd.Location = new Point(6, 181);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(88, 23);
            btnAdd.TabIndex = 6;
            btnAdd.Text = "Thêm mới";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;

            txtDescription.Location = new Point(6, 138);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(263, 23);
            txtDescription.TabIndex = 5;

            txtCategoryName.Location = new Point(6, 89);
            txtCategoryName.Name = "txtCategoryName";
            txtCategoryName.Size = new Size(263, 23);
            txtCategoryName.TabIndex = 4;

            txtId.Location = new Point(6, 40);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(263, 23);
            txtId.TabIndex = 3;

            label3.AutoSize = true;
            label3.Location = new Point(6, 120);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 2;
            label3.Text = "Mô tả";

            label2.AutoSize = true;
            label2.Location = new Point(6, 71);
            label2.Name = "label2";
            label2.Size = new Size(90, 15);
            label2.TabIndex = 1;
            label2.Text = "Tên nhóm hàng";

            label1.AutoSize = true;
            label1.Location = new Point(6, 22);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "Mã ID";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);

            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);

            Name = "FormCategoryManagement";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý nhóm hàng";

            Load += FormCategoryManagement_Load;

            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();

            groupBox2.ResumeLayout(false);

            ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();

            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();

            ResumeLayout(false);
        }

        private TextBox txtKeyword;
        private Button btnSearch;
        private Button btnLoad;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label1;
        private TextBox txtDescription;
        private TextBox txtCategoryName;
        private TextBox txtId;
        private Label label3;
        private Label label2;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnAdd;
        private DataGridView dgvCategories;
    }
}
