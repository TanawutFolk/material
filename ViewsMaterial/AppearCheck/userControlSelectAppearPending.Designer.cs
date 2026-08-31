namespace RawMat.ViewsMaterial.AppearCheck
{
    partial class userControlSelectAppearPending
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
            this.lb_top = new System.Windows.Forms.Label();
            this.dtg_appearPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_appearPending)).BeginInit();
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
            this.lb_top.TabIndex = 33;
            this.lb_top.Text = "Select Report for : Appearance Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtg_appearPending
            // 
            this.dtg_appearPending.AllowUserToAddRows = false;
            this.dtg_appearPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_appearPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_appearPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_appearPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_appearPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_appearPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_appearPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_appearPending.DoubleBuffered = true;
            this.dtg_appearPending.EnableHeadersVisualStyles = false;
            this.dtg_appearPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_appearPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_appearPending.Location = new System.Drawing.Point(59, 162);
            this.dtg_appearPending.Name = "dtg_appearPending";
            this.dtg_appearPending.ReadOnly = true;
            this.dtg_appearPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_appearPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_appearPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_appearPending.TabIndex = 34;
            this.dtg_appearPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_appearPending_CellClick);
            // 
            // userControlSelectAppearPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.dtg_appearPending);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlSelectAppearPending";
            this.Size = new System.Drawing.Size(1115, 888);
            this.Load += new System.EventHandler(this.userControlSelectAppearPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_appearPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_appearPending;
    }
}
