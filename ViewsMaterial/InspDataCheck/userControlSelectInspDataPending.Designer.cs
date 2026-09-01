namespace RawMat.ViewsMaterial.InspDataCheck
{
    partial class userControlSelectInspDataPending
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dtg_InspDataPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.lb_process = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_InspDataPending)).BeginInit();
            this.SuspendLayout();
            // 
            // dtg_InspDataPending
            // 
            this.dtg_InspDataPending.AllowUserToAddRows = false;
            this.dtg_InspDataPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_InspDataPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_InspDataPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_InspDataPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_InspDataPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_InspDataPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_InspDataPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_InspDataPending.DoubleBuffered = true;
            this.dtg_InspDataPending.EnableHeadersVisualStyles = false;
            this.dtg_InspDataPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_InspDataPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_InspDataPending.Location = new System.Drawing.Point(51, 138);
            this.dtg_InspDataPending.Name = "dtg_InspDataPending";
            this.dtg_InspDataPending.ReadOnly = true;
            this.dtg_InspDataPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_InspDataPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_InspDataPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_InspDataPending.TabIndex = 29;
            this.dtg_InspDataPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_InspDataPending_CellClick);
            // 
            // lb_process
            // 
            this.lb_process.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_process.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_process.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_process.Location = new System.Drawing.Point(0, 0);
            this.lb_process.Name = "lb_process";
            this.lb_process.Size = new System.Drawing.Size(1115, 40);
            this.lb_process.TabIndex = 28;
            this.lb_process.Text = "Select Report for :  Inspection Data Check Pending";
            this.lb_process.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // userControlSelectInspDataPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.Controls.Add(this.dtg_InspDataPending);
            this.Controls.Add(this.lb_process);
            this.Name = "userControlSelectInspDataPending";
            this.Size = new System.Drawing.Size(1115, 730);
            this.Load += new System.EventHandler(this.userControlSelectInspDataPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_InspDataPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_InspDataPending;
        private System.Windows.Forms.Label lb_process;
    }
}
