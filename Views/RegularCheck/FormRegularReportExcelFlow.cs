using RawMat.Property;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RawMat.Views.RegularCheck
{
    public class FormRegularReportExcelFlow : Form
    {
        private const string PdfFolderName = "FM-QA-B13-A Material Regular Inspection Record Sheet";

        private readonly QAdataProperty propQA;

        public FormRegularReportExcelFlow(QAdataProperty dataItem)
        {
            propQA = dataItem;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Regular Report Excel Flow";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Size = new Size(900, 430);
            MinimumSize = new Size(760, 360);

            var topLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 46,
                Text = "Regular Report Excel Test",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 18F, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                ColumnCount = 2,
                RowCount = 7
            };

            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < content.RowCount; i++)
            {
                content.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 6 ? 58F : 40F));
            }

            AddRow(content, 0, "Report No.", propQA.Report_No);
            AddRow(content, 1, "Regular No.", propQA.Regular_No);
            AddRow(content, 2, "M-Code", propQA.M_CODE);
            AddRow(content, 3, "Wait Approved", GetWaitApprovedPath());
            AddRow(content, 4, "Save PDF", GetPdfSavePath());
            AddRow(content, 5, "Approved", GetApprovedPath());

            var note = new Label
            {
                Text = "Test only: Excel/PDF file creation is not implemented yet.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Tahoma", 10F, FontStyle.Italic),
                ForeColor = Color.DarkSlateGray
            };
            content.Controls.Add(note, 1, 6);

            var closeButton = new Button
            {
                Text = "OK",
                Width = 120,
                Height = 34,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Font = new Font("Tahoma", 11F)
            };
            closeButton.Click += (sender, e) => Close();
            content.Controls.Add(closeButton, 0, 6);

            Controls.Add(content);
            Controls.Add(topLabel);
        }

        private void AddRow(TableLayoutPanel panel, int rowIndex, string title, string value)
        {
            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                ForeColor = Color.Black,
                Padding = new Padding(0, 0, 12, 0)
            };

            var valueBox = new TextBox
            {
                Text = value ?? string.Empty,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Tahoma", 10F)
            };

            panel.Controls.Add(titleLabel, 0, rowIndex);
            panel.Controls.Add(valueBox, 1, rowIndex);
        }

        private string GetWaitApprovedPath()
        {
            return ConfigurationManager.AppSettings["RegularReportWaitAppTest"]
                ?? ConfigurationManager.AppSettings["RegularReportWaitApp"]
                ?? @"C:\192.168.2.100\12_qa\01_Material\Z2_Receipt_Inspection\04_Regular check\2026\Wait Approved";
        }

        private string GetApprovedPath()
        {
            return ConfigurationManager.AppSettings["RegularReportAppTest"]
                ?? ConfigurationManager.AppSettings["RegularReportApp"]
                ?? @"C:\192.168.2.100\12_qa\01_Material\Z2_Receipt_Inspection\04_Regular check\2026\Approved";
        }

        private string GetPdfSavePath()
        {
            string scanRoot = ConfigurationManager.AppSettings["RegularReportScanTest"]
                ?? ConfigurationManager.AppSettings["RegularReportScan"]
                ?? @"C:\192.168.2.100\15_Document_Scan\DOCUMENT QA";

            return Path.Combine(scanRoot, "2026", PdfFolderName);
        }
    }
}
