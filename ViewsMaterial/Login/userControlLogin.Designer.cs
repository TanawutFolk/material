namespace RawMat.Login
{
    partial class userControlLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlLogin));
            this.tb_pass = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_login = new System.Windows.Forms.TextBox();
            this.bt_login = new Bunifu.Framework.UI.BunifuThinButton2();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_pass
            // 
            this.tb_pass.Location = new System.Drawing.Point(7, 165);
            this.tb_pass.Name = "tb_pass";
            this.tb_pass.PasswordChar = 'æ';
            this.tb_pass.Size = new System.Drawing.Size(130, 20);
            this.tb_pass.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label5.ForeColor = System.Drawing.Color.DarkRed;
            this.label5.Location = new System.Drawing.Point(4, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.DarkRed;
            this.label3.Location = new System.Drawing.Point(4, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Employee ID";
            // 
            // tb_login
            // 
            this.tb_login.Location = new System.Drawing.Point(7, 121);
            this.tb_login.Name = "tb_login";
            this.tb_login.Size = new System.Drawing.Size(130, 20);
            this.tb_login.TabIndex = 10;
            // 
            // bt_login
            // 
            this.bt_login.ActiveBorderThickness = 1;
            this.bt_login.ActiveCornerRadius = 20;
            this.bt_login.ActiveFillColor = System.Drawing.Color.SeaGreen;
            this.bt_login.ActiveForecolor = System.Drawing.Color.White;
            this.bt_login.ActiveLineColor = System.Drawing.Color.SeaGreen;
            this.bt_login.BackColor = System.Drawing.Color.LightPink;
            this.bt_login.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_login.BackgroundImage")));
            this.bt_login.ButtonText = "Log in";
            this.bt_login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_login.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_login.ForeColor = System.Drawing.Color.SeaGreen;
            this.bt_login.IdleBorderThickness = 1;
            this.bt_login.IdleCornerRadius = 20;
            this.bt_login.IdleFillColor = System.Drawing.Color.DeepPink;
            this.bt_login.IdleForecolor = System.Drawing.Color.White;
            this.bt_login.IdleLineColor = System.Drawing.Color.SeaGreen;
            this.bt_login.Location = new System.Drawing.Point(23, 193);
            this.bt_login.Margin = new System.Windows.Forms.Padding(5);
            this.bt_login.Name = "bt_login";
            this.bt_login.Size = new System.Drawing.Size(101, 39);
            this.bt_login.TabIndex = 16;
            this.bt_login.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bt_login.Click += new System.EventHandler(this.bt_login_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RawMat.Properties.Resources.login1;
            this.pictureBox2.Location = new System.Drawing.Point(23, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(101, 94);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // userControlLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.bt_login);
            this.Controls.Add(this.tb_pass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_login);
            this.Controls.Add(this.pictureBox2);
            this.Name = "userControlLogin";
            this.Size = new System.Drawing.Size(150, 240);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_pass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_login;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Bunifu.Framework.UI.BunifuThinButton2 bt_login;
    }
}
