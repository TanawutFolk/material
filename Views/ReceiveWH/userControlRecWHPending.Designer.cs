namespace RawMat.Views.ReceiveWH
{
    partial class userControlRecWHPending
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
            this.dtg_recWHPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.lb_top = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_recWHPending)).BeginInit();
            this.SuspendLayout();
            // 
            // dtg_recWHPending
            // 
            this.dtg_recWHPending.AllowUserToAddRows = false;
            this.dtg_recWHPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_recWHPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_recWHPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_recWHPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_recWHPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_recWHPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_recWHPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_recWHPending.DoubleBuffered = true;
            this.dtg_recWHPending.EnableHeadersVisualStyles = false;
            this.dtg_recWHPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_recWHPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_recWHPending.Location = new System.Drawing.Point(53, 120);
            this.dtg_recWHPending.Name = "dtg_recWHPending";
            this.dtg_recWHPending.ReadOnly = true;
            this.dtg_recWHPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_recWHPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_recWHPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_recWHPending.TabIndex = 21;
            this.dtg_recWHPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_recWHPending_CellClick);
            this.dtg_recWHPending.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_recWHPending_CellContentClick);
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 40);
            this.lb_top.TabIndex = 23;
            this.lb_top.Text = "Receive WH Issue Check Sheet Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // userControlRecWHPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.dtg_recWHPending);
            this.Name = "userControlRecWHPending";
            this.Size = new System.Drawing.Size(1115, 600);
            this.Load += new System.EventHandler(this.userControlRecWHPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_recWHPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_recWHPending;
        private System.Windows.Forms.Label lb_top;
    }
}
