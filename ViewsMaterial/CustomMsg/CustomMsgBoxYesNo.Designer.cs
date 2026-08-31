namespace RawMat.ViewsMaterial.CustomMsg
{
    partial class CustomMsgBoxYesNo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

            this.bt_yes = new System.Windows.Forms.Button();
            this.bt_no = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bt_yes
            // 
            this.bt_yes.BackColor = System.Drawing.Color.Lime;
            this.bt_yes.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_yes.Location = new System.Drawing.Point(169, 267);
            this.bt_yes.Name = "bt_yes";
            this.bt_yes.Size = new System.Drawing.Size(179, 58);
            this.bt_yes.TabIndex = 2;
            this.bt_yes.Text = "YES";
            this.bt_yes.UseVisualStyleBackColor = false;
            this.bt_yes.Click += new System.EventHandler(this.bt_yes_Click);
            // 
            // bt_no
            // 
            this.bt_no.BackColor = System.Drawing.Color.Red;
            this.bt_no.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_no.Location = new System.Drawing.Point(455, 267);
            this.bt_no.Name = "bt_no";
            this.bt_no.Size = new System.Drawing.Size(179, 58);
            this.bt_no.TabIndex = 3;
            this.bt_no.Text = "NO";
            this.bt_no.UseVisualStyleBackColor = false;
            this.bt_no.Click += new System.EventHandler(this.bt_no_Click);
            // 
            // CustomMsgBoxYesNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.bt_no);
            this.Controls.Add(this.bt_yes);
            this.Name = "CustomMsgBoxYesNo";
            this.Text = "Custom Yes/No Message Box";
            this.Controls.SetChildIndex(this.bt_yes, 0);
            this.Controls.SetChildIndex(this.bt_no, 0);
            this.Controls.SetChildIndex(this.lblMessage, 0);
            this.Controls.SetChildIndex(this.picAlarm, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bt_yes;
        private System.Windows.Forms.Button bt_no;
    }
}