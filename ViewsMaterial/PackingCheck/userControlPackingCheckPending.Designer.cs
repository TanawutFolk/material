namespace RawMat.ViewsMaterial.PackingCheck
{
    partial class userControlPackingCheckPending
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lb_top = new System.Windows.Forms.Label();
            this.dtg_packingCheckPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packingCheckPending)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 29;
            this.lb_top.Text = "Select Report for : Packing Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtg_packingCheckPending
            // 
            this.dtg_packingCheckPending.AllowUserToAddRows = false;
            this.dtg_packingCheckPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_packingCheckPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dtg_packingCheckPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_packingCheckPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_packingCheckPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_packingCheckPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dtg_packingCheckPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_packingCheckPending.DoubleBuffered = true;
            this.dtg_packingCheckPending.EnableHeadersVisualStyles = false;
            this.dtg_packingCheckPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_packingCheckPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_packingCheckPending.Location = new System.Drawing.Point(37, 117);
            this.dtg_packingCheckPending.Name = "dtg_packingCheckPending";
            this.dtg_packingCheckPending.ReadOnly = true;
            this.dtg_packingCheckPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_packingCheckPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_packingCheckPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_packingCheckPending.TabIndex = 30;
            this.dtg_packingCheckPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_packingCheckPending_CellClick);
            // 
            // userControlPackingCheckPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.dtg_packingCheckPending);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlPackingCheckPending";
            this.Size = new System.Drawing.Size(1115, 888);
            this.Load += new System.EventHandler(this.userControlPackingCheckPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packingCheckPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_packingCheckPending;
    }
}
