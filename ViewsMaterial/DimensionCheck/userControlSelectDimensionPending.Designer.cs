namespace RawMat.ViewsMaterial.DimensionCheck
{
    partial class userControlSelectDimensionPending
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dtg_dimensionPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.lb_top = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_dimensionPending)).BeginInit();
            this.SuspendLayout();
            // 
            // dtg_dimensionPending
            // 
            this.dtg_dimensionPending.AllowUserToAddRows = false;
            this.dtg_dimensionPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_dimensionPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dtg_dimensionPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_dimensionPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_dimensionPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_dimensionPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dtg_dimensionPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_dimensionPending.DoubleBuffered = true;
            this.dtg_dimensionPending.EnableHeadersVisualStyles = false;
            this.dtg_dimensionPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_dimensionPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_dimensionPending.Location = new System.Drawing.Point(59, 162);
            this.dtg_dimensionPending.Name = "dtg_dimensionPending";
            this.dtg_dimensionPending.ReadOnly = true;
            this.dtg_dimensionPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_dimensionPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_dimensionPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_dimensionPending.TabIndex = 31;
            this.dtg_dimensionPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_dimensionPending_CellClick);
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 32;
            this.lb_top.Text = "Select Report for : Dimension Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // userControlSelectDimensionPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Aquamarine;
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.dtg_dimensionPending);
            this.Name = "userControlSelectDimensionPending";
            this.Size = new System.Drawing.Size(1115, 888);
            this.Load += new System.EventHandler(this.userControlSelectDimensionPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_dimensionPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_dimensionPending;
        private System.Windows.Forms.Label lb_top;
    }
}
