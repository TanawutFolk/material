namespace RawMat.ViewsMaterial.RegularCheck
{
    partial class userControlSelectRegularPending
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
            this.dtg_regularPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.lb_top = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_regularPending)).BeginInit();
            this.SuspendLayout();
            // 
            // dtg_regularPending
            // 
            this.dtg_regularPending.AllowUserToAddRows = false;
            this.dtg_regularPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_regularPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_regularPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_regularPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_regularPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_regularPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_regularPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_regularPending.DoubleBuffered = true;
            this.dtg_regularPending.EnableHeadersVisualStyles = false;
            this.dtg_regularPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_regularPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_regularPending.Location = new System.Drawing.Point(59, 162);
            this.dtg_regularPending.Name = "dtg_regularPending";
            this.dtg_regularPending.ReadOnly = true;
            this.dtg_regularPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_regularPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_regularPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_regularPending.TabIndex = 31;
            this.dtg_regularPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_regularPending_CellClick);
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
            this.lb_top.Text = "Select Report for : Regular Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // userControlSelectRegularPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.dtg_regularPending);
            this.Name = "userControlSelectRegularPending";
            this.Size = new System.Drawing.Size(1115, 888);
            this.Load += new System.EventHandler(this.userControlSelectRegularPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_regularPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_regularPending;
        private System.Windows.Forms.Label lb_top;
    }
}
