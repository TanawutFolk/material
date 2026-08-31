using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsMaterial.CustomMsg
{
    public partial class CustomMsgBoxBase : Form
    {
        protected Label lblMessage;
        protected PictureBox picAlarm;

        public CustomMsgBoxBase()
        {
            InitializeComponent();
        }

        public string Message
        {
            get => lblMessage?.Text;
            set => lblMessage.Text = value;
        }

        // Property ใหม่สำหรับกำหนดชื่อหน้าต่าง (Title Bar)
        public string Title
        {
            get => this.Text;
            set => this.Text = value;
        }

        // Property สำหรับสีพื้นหลังของ lblMessage
        public Color MessageBackColor
        {
            get => lblMessage?.BackColor ?? Color.Aqua;
            set => lblMessage.BackColor = value;
        }

        // Property สำหรับสีตัวอักษรของ lblMessage
        public Color MessageForeColor
        {
            get => lblMessage?.ForeColor ?? Color.Black;
            set => lblMessage.ForeColor = value;
        }

        // Property สำหรับฟอนต์ของ lblMessage
        public Font MessageFont
        {
            get => lblMessage?.Font;
            set => lblMessage.Font = value;
        }

        // Property สำหรับขนาดฟอนต์ของ lblMessage
        public float MessageFontSize
        {
            get => lblMessage?.Font.Size ?? 36f;
            set => lblMessage.Font = new Font(lblMessage.Font.FontFamily, value, lblMessage.Font.Style);
        }

        public enum MessageBoxIconType
        {
            Question,
            OK,
            NG,
            Pending,
            Warning
        }

        public MessageBoxIconType Icon
        {
            set
            {
                if (picAlarm != null)
                {
                    Image iconImage = LoadIconImage(value);
                    if (iconImage != null)
                        picAlarm.Image = iconImage;

                    picAlarm.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private Image LoadIconImage(MessageBoxIconType iconType)
        {
            string fileName;
            switch (iconType)
            {
                case MessageBoxIconType.Question:
                    fileName = "QUESTION.png";
                    break;
                case MessageBoxIconType.OK:
                    fileName = "OK.png";
                    break;
                case MessageBoxIconType.NG:
                    fileName = "NG.png";
                    break;
                case MessageBoxIconType.Pending:
                    fileName = "PENDING.png";
                    break;
                case MessageBoxIconType.Warning:
                    fileName = "WARNING.png";
                    break;
                default:
                    return null;
            }

            foreach (string path in GetIconSearchPaths(fileName))
            {
                try
                {
                    if (!File.Exists(path))
                        continue;

                    using (Image image = Image.FromFile(path))
                    {
                        return new Bitmap(image);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading icon '{path}': {ex.Message}");
                }
            }

            Debug.WriteLine($"Icon file not found: {fileName}");
            return null;
        }

        private IEnumerable<string> GetIconSearchPaths(string fileName)
        {
            yield return Path.Combine(Application.StartupPath, "img", fileName);
            yield return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", fileName);
            yield return Path.Combine(Environment.CurrentDirectory, "img", fileName);
            yield return Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "img", fileName));
        }

        // Enum สำหรับเลือกประเภทของ Message Box
        public enum MessageBoxDialogType
        {
            OK,
            YesNo
        }

        // เมธอดใหม่สำหรับแสดง Custom Message Box
        public static bool ShowCustomMessageBox(
            string message,
            string title,
            MessageBoxIconType icon,
            MessageBoxDialogType dialogType = MessageBoxDialogType.OK,
            Color? backColor = null,
            Color? foreColor = null,
            Font font = null,
            float? fontSize = null)
        {
            if (dialogType == MessageBoxDialogType.OK)
            {
                var msgBox = new CustomMsgBox();
                msgBox.SetMessage(message, title);
                msgBox.SetIcon(icon);

                // ตั้งค่าสีพื้นหลัง ถ้าระบุมา
                if (backColor.HasValue)
                    msgBox.MessageBackColor = backColor.Value;

                // ตั้งค่าสีตัวอักษร ถ้าระบุมา
                if (foreColor.HasValue)
                    msgBox.MessageForeColor = foreColor.Value;

                // ตั้งค่าฟอนต์ ถ้าระบุมา
                if (font != null)
                    msgBox.MessageFont = font;

                // ตั้งค่าขนาดฟอนต์ ถ้าระบุมา
                if (fontSize.HasValue)
                    msgBox.MessageFontSize = fontSize.Value;

                msgBox.ShowDialog();
                return true; // คืนค่า true สำหรับ OK (ไม่มีผลลัพธ์เฉพาะ)
            }
            else if (dialogType == MessageBoxDialogType.YesNo)
            {
                var msgBoxYesNo = new CustomMsgBoxYesNo();
                msgBoxYesNo.SetMessage(message, title);
                msgBoxYesNo.SetIcon(icon);

                // ตั้งค่าสีพื้นหลัง ถ้าระบุมา
                if (backColor.HasValue)
                    msgBoxYesNo.MessageBackColor = backColor.Value;

                // ตั้งค่าสีตัวอักษร ถ้าระบุมา
                if (foreColor.HasValue)
                    msgBoxYesNo.MessageForeColor = foreColor.Value;

                // ตั้งค่าฟอนต์ ถ้าระบุมา
                if (font != null)
                    msgBoxYesNo.MessageFont = font;

                // ตั้งค่าขนาดฟอนต์ ถ้าระบุมา
                if (fontSize.HasValue)
                    msgBoxYesNo.MessageFontSize = fontSize.Value;

                msgBoxYesNo.ShowDialog();
                return msgBoxYesNo.IsYesClicked; // คืนค่า true ถ้ากด Yes, false ถ้ากด No
            }
            return false; // ค่าเริ่มต้นถ้า dialogType ไม่ถูกต้อง
        }


    }
}
