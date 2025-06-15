namespace EFCoreWinApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            edtProductId = new TextBox();
            btnSalesOrders = new Button();
            btnProductsWithEFListParams = new Button();
            btnProductsWithFilterFunc = new Button();
            btnProductsWithSqlFilter = new Button();
            btnSingleProductById = new Button();
            edtSqlFilter = new TextBox();
            btnProductsWithService = new Button();
            btnProductsNoService = new Button();
            edtLog = new RichTextBox();
            label1 = new Label();
            edtPageNo = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(edtPageNo);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(edtProductId);
            panel1.Controls.Add(btnSalesOrders);
            panel1.Controls.Add(btnProductsWithEFListParams);
            panel1.Controls.Add(btnProductsWithFilterFunc);
            panel1.Controls.Add(btnProductsWithSqlFilter);
            panel1.Controls.Add(btnSingleProductById);
            panel1.Controls.Add(edtSqlFilter);
            panel1.Controls.Add(btnProductsWithService);
            panel1.Controls.Add(btnProductsNoService);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1143, 178);
            panel1.TabIndex = 0;
            // 
            // edtProductId
            // 
            edtProductId.Font = new Font("Courier New", 9F);
            edtProductId.Location = new Point(743, 56);
            edtProductId.Name = "edtProductId";
            edtProductId.Size = new Size(240, 21);
            edtProductId.TabIndex = 9;
            // 
            // btnSalesOrders
            // 
            btnSalesOrders.Location = new Point(217, 136);
            btnSalesOrders.Name = "btnSalesOrders";
            btnSalesOrders.Size = new Size(174, 32);
            btnSalesOrders.TabIndex = 8;
            btnSalesOrders.Text = "Sales Orders";
            btnSalesOrders.UseVisualStyleBackColor = true;
            // 
            // btnProductsWithEFListParams
            // 
            btnProductsWithEFListParams.Location = new Point(217, 88);
            btnProductsWithEFListParams.Name = "btnProductsWithEFListParams";
            btnProductsWithEFListParams.Size = new Size(174, 32);
            btnProductsWithEFListParams.TabIndex = 7;
            btnProductsWithEFListParams.Text = "Products with EFListParams";
            btnProductsWithEFListParams.UseVisualStyleBackColor = true;
            // 
            // btnProductsWithFilterFunc
            // 
            btnProductsWithFilterFunc.Location = new Point(217, 50);
            btnProductsWithFilterFunc.Name = "btnProductsWithFilterFunc";
            btnProductsWithFilterFunc.Size = new Size(174, 32);
            btnProductsWithFilterFunc.TabIndex = 6;
            btnProductsWithFilterFunc.Text = "Products with Filter Func";
            btnProductsWithFilterFunc.UseVisualStyleBackColor = true;
            // 
            // btnProductsWithSqlFilter
            // 
            btnProductsWithSqlFilter.Location = new Point(566, 12);
            btnProductsWithSqlFilter.Name = "btnProductsWithSqlFilter";
            btnProductsWithSqlFilter.Size = new Size(171, 32);
            btnProductsWithSqlFilter.TabIndex = 5;
            btnProductsWithSqlFilter.Text = "Products with Sql Filter";
            btnProductsWithSqlFilter.UseVisualStyleBackColor = true;
            // 
            // btnSingleProductById
            // 
            btnSingleProductById.Location = new Point(566, 50);
            btnSingleProductById.Name = "btnSingleProductById";
            btnSingleProductById.Size = new Size(171, 32);
            btnSingleProductById.TabIndex = 4;
            btnSingleProductById.Text = "Single Product By Id";
            btnSingleProductById.UseVisualStyleBackColor = true;
            // 
            // edtSqlFilter
            // 
            edtSqlFilter.Font = new Font("Courier New", 9F);
            edtSqlFilter.Location = new Point(743, 18);
            edtSqlFilter.Name = "edtSqlFilter";
            edtSqlFilter.Size = new Size(240, 21);
            edtSqlFilter.TabIndex = 2;
            edtSqlFilter.Text = "Name like '%al%'";
            // 
            // btnProductsWithService
            // 
            btnProductsWithService.Location = new Point(217, 12);
            btnProductsWithService.Name = "btnProductsWithService";
            btnProductsWithService.Size = new Size(174, 32);
            btnProductsWithService.TabIndex = 1;
            btnProductsWithService.Text = "Products";
            btnProductsWithService.UseVisualStyleBackColor = true;
            // 
            // btnProductsNoService
            // 
            btnProductsNoService.Location = new Point(24, 12);
            btnProductsNoService.Name = "btnProductsNoService";
            btnProductsNoService.Size = new Size(174, 32);
            btnProductsNoService.TabIndex = 0;
            btnProductsNoService.Text = "Products (no service)";
            btnProductsNoService.UseVisualStyleBackColor = true;
            // 
            // edtLog
            // 
            edtLog.BackColor = Color.Gainsboro;
            edtLog.Dock = DockStyle.Fill;
            edtLog.Font = new Font("Courier New", 9F);
            edtLog.Location = new Point(0, 178);
            edtLog.Name = "edtLog";
            edtLog.Size = new Size(1143, 564);
            edtLog.TabIndex = 1;
            edtLog.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(397, 18);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 10;
            label1.Text = "Page No";
            // 
            // edtPageNo
            // 
            edtPageNo.Location = new Point(455, 17);
            edtPageNo.Name = "edtPageNo";
            edtPageNo.Size = new Size(49, 23);
            edtPageNo.TabIndex = 11;
            edtPageNo.Text = "0";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 742);
            Controls.Add(edtLog);
            Controls.Add(panel1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Entity Framework Core test app";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private RichTextBox edtLog;
        private Button btnProductsNoService;
        private Button btnProductsWithService;
        private Label label1;
        private TextBox edtSqlFilter;
        private Button btnSingleProductById;
        private Button btnProductsWithSqlFilter;
        private Button btnProductsWithFilterFunc;
        private Button btnProductsWithEFListParams;
        private Button btnSalesOrders;
        private TextBox edtProductId;
        private TextBox edtPageNo;
    }
}
