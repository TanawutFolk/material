namespace RawMat.ViewsMaterialNCR.Controls
{
    partial class ucNcrTable
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pn_Card = new RawMat.ViewsMaterialNCR.Controls.RoundedPanel();
            this.dtg = new RawMat.ViewsMaterialNCR.Controls.StyledDataGrid();
            this.colNcrNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSupplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPartNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProblem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pager = new RawMat.ViewsMaterialNCR.Controls.ucPager();
            ((System.ComponentModel.ISupportInitialize)(this.dtg)).BeginInit();
            this.pn_Card.SuspendLayout();
            this.SuspendLayout();
            // 
            // pn_Card
            // 
            this.pn_Card.Controls.Add(this.dtg);
            this.pn_Card.Controls.Add(this.pager);
            this.pn_Card.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pn_Card.Location = new System.Drawing.Point(0, 0);
            this.pn_Card.Name = "pn_Card";
            this.pn_Card.Padding = new System.Windows.Forms.Padding(1);
            this.pn_Card.Size = new System.Drawing.Size(1045, 814);
            this.pn_Card.TabIndex = 0;
            // 
            // dtg
            // 
            this.dtg.ActionColumnName = "colAction";
            this.dtg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNcrNo,
            this.colDate,
            this.colSupplier,
            this.colPartNo,
            this.colProblem,
            this.colOwner,
            this.colStatus,
            this.colAction});
            this.dtg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtg.Location = new System.Drawing.Point(1, 1);
            this.dtg.Name = "dtg";
            this.dtg.StatusColumnName = "colStatus";
            this.dtg.Size = new System.Drawing.Size(1043, 752);
            this.dtg.TabIndex = 0;
            // 
            // colNcrNo
            // 
            this.colNcrNo.FillWeight = 138F;
            this.colNcrNo.HeaderText = "NCR No.";
            this.colNcrNo.Name = "colNcrNo";
            this.colNcrNo.ReadOnly = true;
            this.colNcrNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colDate
            // 
            this.colDate.FillWeight = 100F;
            this.colDate.HeaderText = "Date";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colSupplier
            // 
            this.colSupplier.FillWeight = 158F;
            this.colSupplier.HeaderText = "Supplier";
            this.colSupplier.Name = "colSupplier";
            this.colSupplier.ReadOnly = true;
            this.colSupplier.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colPartNo
            // 
            this.colPartNo.FillWeight = 105F;
            this.colPartNo.HeaderText = "Part No.";
            this.colPartNo.Name = "colPartNo";
            this.colPartNo.ReadOnly = true;
            this.colPartNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colProblem
            // 
            this.colProblem.FillWeight = 202F;
            this.colProblem.HeaderText = "Problem";
            this.colProblem.Name = "colProblem";
            this.colProblem.ReadOnly = true;
            this.colProblem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colOwner
            // 
            this.colOwner.FillWeight = 122F;
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            this.colOwner.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colStatus
            // 
            this.colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colStatus.Width = 110;
            // 
            // colAction
            // 
            this.colAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colAction.HeaderText = "Action";
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            this.colAction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colAction.Width = 90;
            // 
            // pager
            // 
            this.pager.BackColor = System.Drawing.Color.White;
            this.pager.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pager.Location = new System.Drawing.Point(1, 753);
            this.pager.Name = "pager";
            this.pager.Size = new System.Drawing.Size(1043, 60);
            this.pager.TabIndex = 1;
            // 
            // ucNcrTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pn_Card);
            this.Name = "ucNcrTable";
            this.Size = new System.Drawing.Size(1045, 814);
            ((System.ComponentModel.ISupportInitialize)(this.dtg)).EndInit();
            this.pn_Card.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RawMat.ViewsMaterialNCR.Controls.RoundedPanel pn_Card;
        private RawMat.ViewsMaterialNCR.Controls.StyledDataGrid dtg;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNcrNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSupplier;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPartNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProblem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private RawMat.ViewsMaterialNCR.Controls.ucPager pager;
    }
}
