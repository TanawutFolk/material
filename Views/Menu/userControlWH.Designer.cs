namespace RawMat.Views.Menu
{
    partial class userControlWH
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
            this.bt_rec_replace = new Bunifu.Framework.UI.BunifuTileButton();
            this.bt_rec_issue = new Bunifu.Framework.UI.BunifuTileButton();
            this.SuspendLayout();
            // 
            // bt_rec_replace
            // 
            this.bt_rec_replace.BackColor = System.Drawing.Color.LightPink;
            this.bt_rec_replace.color = System.Drawing.Color.LightPink;
            this.bt_rec_replace.colorActive = System.Drawing.Color.HotPink;
            this.bt_rec_replace.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_rec_replace.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_rec_replace.ForeColor = System.Drawing.Color.DarkRed;
            this.bt_rec_replace.Image = global::RawMat.Properties.Resources.supply_chain;
            this.bt_rec_replace.ImagePosition = 5;
            this.bt_rec_replace.ImageZoom = 20;
            this.bt_rec_replace.LabelPosition = 35;
            this.bt_rec_replace.LabelText = "Receive WH \nReplacement";
            this.bt_rec_replace.Location = new System.Drawing.Point(0, 70);
            this.bt_rec_replace.Margin = new System.Windows.Forms.Padding(5);
            this.bt_rec_replace.Name = "bt_rec_replace";
            this.bt_rec_replace.Size = new System.Drawing.Size(150, 70);
            this.bt_rec_replace.TabIndex = 26;
            this.bt_rec_replace.Click += new System.EventHandler(this.bt_rec_replace_Click);
            // 
            // bt_rec_issue
            // 
            this.bt_rec_issue.BackColor = System.Drawing.Color.LightPink;
            this.bt_rec_issue.color = System.Drawing.Color.LightPink;
            this.bt_rec_issue.colorActive = System.Drawing.Color.HotPink;
            this.bt_rec_issue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_rec_issue.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_rec_issue.ForeColor = System.Drawing.Color.DarkRed;
            this.bt_rec_issue.Image = global::RawMat.Properties.Resources.warehouse;
            this.bt_rec_issue.ImagePosition = 5;
            this.bt_rec_issue.ImageZoom = 20;
            this.bt_rec_issue.LabelPosition = 35;
            this.bt_rec_issue.LabelText = "Receive WH \nIssue Check Sheet";
            this.bt_rec_issue.Location = new System.Drawing.Point(0, 0);
            this.bt_rec_issue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bt_rec_issue.Name = "bt_rec_issue";
            this.bt_rec_issue.Size = new System.Drawing.Size(150, 70);
            this.bt_rec_issue.TabIndex = 25;
            this.bt_rec_issue.Click += new System.EventHandler(this.bt_rec_issue_Click);
            // 
            // userControlWH
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.bt_rec_replace);
            this.Controls.Add(this.bt_rec_issue);
            this.Name = "userControlWH";
            this.Size = new System.Drawing.Size(150, 140);
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuTileButton bt_rec_issue;
        private Bunifu.Framework.UI.BunifuTileButton bt_rec_replace;
    }
}
