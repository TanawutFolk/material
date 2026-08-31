namespace RawMat.ViewsMaterial.FunctionCheck
{
    partial class userControlSelectFunctionPending
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
            this.dtg_functionPending = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_functionPending)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_top
            // 
            this.lb_top.BackColor = System.Drawing.Color.Wheat;
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 33;
            this.lb_top.Text = "Select Report for : Function Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtg_functionPending
            // 
            this.dtg_functionPending.AllowUserToAddRows = false;
            this.dtg_functionPending.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_functionPending.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dtg_functionPending.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_functionPending.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_functionPending.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_functionPending.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dtg_functionPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_functionPending.DoubleBuffered = true;
            this.dtg_functionPending.EnableHeadersVisualStyles = false;
            this.dtg_functionPending.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_functionPending.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_functionPending.Location = new System.Drawing.Point(51, 138);
            this.dtg_functionPending.Name = "dtg_functionPending";
            this.dtg_functionPending.ReadOnly = true;
            this.dtg_functionPending.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_functionPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_functionPending.Size = new System.Drawing.Size(1012, 455);
            this.dtg_functionPending.TabIndex = 34;
            this.dtg_functionPending.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_functionPending_CellClick);
            // 
            // userControlSelectFunctionPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.dtg_functionPending);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlSelectFunctionPending";
            this.Size = new System.Drawing.Size(1115, 730);
            this.Load += new System.EventHandler(this.userControlSelectFunctionPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_functionPending)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_functionPending;
    }
}
