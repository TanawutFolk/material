namespace RawMat.Login
{
    partial class userControlProfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlProfile));
            this.lb_empProfile = new System.Windows.Forms.Label();
            this.lb_nameProfile = new System.Windows.Forms.Label();
            this.lb_position = new System.Windows.Forms.Label();
            this.bt_logout = new Bunifu.Framework.UI.BunifuThinButton2();
            this.pb_profile = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_profile)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_empProfile
            // 
            this.lb_empProfile.AutoSize = true;
            this.lb_empProfile.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_empProfile.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_empProfile.Location = new System.Drawing.Point(12, 118);
            this.lb_empProfile.Name = "lb_empProfile";
            this.lb_empProfile.Size = new System.Drawing.Size(71, 23);
            this.lb_empProfile.TabIndex = 12;
            this.lb_empProfile.Text = "S00000";
            // 
            // lb_nameProfile
            // 
            this.lb_nameProfile.AutoSize = true;
            this.lb_nameProfile.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.lb_nameProfile.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_nameProfile.Location = new System.Drawing.Point(12, 144);
            this.lb_nameProfile.Name = "lb_nameProfile";
            this.lb_nameProfile.Size = new System.Drawing.Size(135, 23);
            this.lb_nameProfile.TabIndex = 13;
            this.lb_nameProfile.Text = "NameSurname";
            // 
            // lb_position
            // 
            this.lb_position.AutoSize = true;
            this.lb_position.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.lb_position.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_position.Location = new System.Drawing.Point(12, 170);
            this.lb_position.Name = "lb_position";
            this.lb_position.Size = new System.Drawing.Size(90, 23);
            this.lb_position.TabIndex = 14;
            this.lb_position.Text = "Operator ";
            // 
            // bt_logout
            // 
            this.bt_logout.ActiveBorderThickness = 1;
            this.bt_logout.ActiveCornerRadius = 20;
            this.bt_logout.ActiveFillColor = System.Drawing.Color.SeaGreen;
            this.bt_logout.ActiveForecolor = System.Drawing.Color.White;
            this.bt_logout.ActiveLineColor = System.Drawing.Color.SeaGreen;
            this.bt_logout.BackColor = System.Drawing.Color.LightPink;
            this.bt_logout.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_logout.BackgroundImage")));
            this.bt_logout.ButtonText = "Log out";
            this.bt_logout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_logout.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_logout.ForeColor = System.Drawing.Color.SeaGreen;
            this.bt_logout.IdleBorderThickness = 1;
            this.bt_logout.IdleCornerRadius = 20;
            this.bt_logout.IdleFillColor = System.Drawing.Color.DeepPink;
            this.bt_logout.IdleForecolor = System.Drawing.Color.White;
            this.bt_logout.IdleLineColor = System.Drawing.Color.SeaGreen;
            this.bt_logout.Location = new System.Drawing.Point(23, 201);
            this.bt_logout.Margin = new System.Windows.Forms.Padding(5);
            this.bt_logout.Name = "bt_logout";
            this.bt_logout.Size = new System.Drawing.Size(101, 39);
            this.bt_logout.TabIndex = 15;
            this.bt_logout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bt_logout.Click += new System.EventHandler(this.bt_logout_Click);
            // 
            // pb_profile
            // 
            this.pb_profile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pb_profile.Location = new System.Drawing.Point(24, 3);
            this.pb_profile.Name = "pb_profile";
            this.pb_profile.Size = new System.Drawing.Size(100, 100);
            this.pb_profile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_profile.TabIndex = 10;
            this.pb_profile.TabStop = false;
            // 
            // userControlProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.bt_logout);
            this.Controls.Add(this.lb_position);
            this.Controls.Add(this.lb_nameProfile);
            this.Controls.Add(this.lb_empProfile);
            this.Controls.Add(this.pb_profile);
            this.Name = "userControlProfile";
            this.Size = new System.Drawing.Size(150, 240);
            ((System.ComponentModel.ISupportInitialize)(this.pb_profile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_profile;
        private System.Windows.Forms.Label lb_empProfile;
        private System.Windows.Forms.Label lb_nameProfile;
        private System.Windows.Forms.Label lb_position;
        private Bunifu.Framework.UI.BunifuThinButton2 bt_logout;
    }
}
