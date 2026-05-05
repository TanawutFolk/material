namespace RawMat.Views.FunctionCheck
{
    partial class userControlSelectFunction
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
            this.lb_process = new System.Windows.Forms.Label();
            this.dtg_reportSelect = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_reportSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_process
            // 
            this.lb_process.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_process.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_process.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_process.Location = new System.Drawing.Point(0, 0);
            this.lb_process.Name = "lb_process";
            this.lb_process.Size = new System.Drawing.Size(1115, 40);
            this.lb_process.TabIndex = 25;
            this.lb_process.Text = "Select Report for : ";
            this.lb_process.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtg_reportSelect
            // 
            this.dtg_reportSelect.AllowUserToAddRows = false;
            this.dtg_reportSelect.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_reportSelect.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_reportSelect.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_reportSelect.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_reportSelect.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_reportSelect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_reportSelect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_reportSelect.DoubleBuffered = true;
            this.dtg_reportSelect.EnableHeadersVisualStyles = false;
            this.dtg_reportSelect.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_reportSelect.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_reportSelect.Location = new System.Drawing.Point(51, 138);
            this.dtg_reportSelect.Name = "dtg_reportSelect";
            this.dtg_reportSelect.ReadOnly = true;
            this.dtg_reportSelect.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_reportSelect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_reportSelect.Size = new System.Drawing.Size(1012, 455);
            this.dtg_reportSelect.TabIndex = 26;
            this.dtg_reportSelect.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_reportSelect_CellClick);
            this.dtg_reportSelect.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dtg_reportSelect_DataBindingComplete);
            // 
            // userControlFunction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.dtg_reportSelect);
            this.Controls.Add(this.lb_process);
            this.Name = "userControlFunction";
            this.Size = new System.Drawing.Size(1115, 730);
            this.Load += new System.EventHandler(this.userControlSelectFunction_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_reportSelect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_process;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_reportSelect;
    }
}
