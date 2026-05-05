using System.Windows.Forms;

namespace RawMat.Views.CustomMsg
{
    partial class CustomMsgBoxBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMsgBoxBase));
            this.lblMessage = new System.Windows.Forms.Label();
            this.picAlarm = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAlarm)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.Aqua;
            this.lblMessage.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(275, 46);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(485, 177);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "ข้อความเริ่มต้น";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picAlarm
            // 
            this.picAlarm.Image = ((System.Drawing.Image)(resources.GetObject("picAlarm.Image")));
            this.picAlarm.Location = new System.Drawing.Point(41, 46);
            this.picAlarm.Name = "picAlarm";
            this.picAlarm.Size = new System.Drawing.Size(228, 177);
            this.picAlarm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAlarm.TabIndex = 1;
            this.picAlarm.TabStop = false;
            // 
            // CustomMsgBoxBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.picAlarm);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CustomMsgBoxBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Message Box Base";
            ((System.ComponentModel.ISupportInitialize)(this.picAlarm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

    }
}