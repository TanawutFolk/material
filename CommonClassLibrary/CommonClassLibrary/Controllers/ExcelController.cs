using ADGV;
using GemBox.Spreadsheet;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace CommonClassLibrary.Controllers
{
    public class ExcelController
    {

        //#######################################################################################################################
        //######################################################################################
        //#######################################################################################################################
        [DllImport("user32.dll")]

        //get Process id Excel for kill process
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);
        public Process GetExcelProcess(Excel.Application excelApp)
        {
            int id;
            GetWindowThreadProcessId(excelApp.Hwnd, out id);
            return Process.GetProcessById(id);
        }
        //#######################################################################################################################


        public bool ExportDatatable(AdvancedDataGridView advgListSerialInput)
        {
            bool _result = false;

            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook;
            Excel.Worksheet worksheet;

            SaveFileDialog saveFileDialog;
            string addressFile;

            Process excelOpen = this.GetExcelProcess(app);

            try
            {
                saveFileDialog = new SaveFileDialog();

                saveFileDialog.InitialDirectory = @"C:\";
                saveFileDialog.Title = "Save Excel Files";
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.DefaultExt = "xls";
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                //select path save.
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {

                    //close all message Alerts.
                    app.DisplayAlerts = false;

                    //show excel. true = Show. | false = Don't Show.
                    app.Visible = false;

                    workbook = app.Workbooks.Add(Type.Missing);
                    worksheet = workbook.Sheets[1];

                    //Head Colums.
                    for (int i = 1; i < advgListSerialInput.Columns.Count + 1; i++)
                    {
                        worksheet.Cells[1, i] = advgListSerialInput.Columns[i - 1].HeaderText;

                    }

                    //Data.
                    for (int i = 0; i <= advgListSerialInput.Rows.Count - 1; i++)
                    {
                        for (int j = 0; j < advgListSerialInput.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1] = advgListSerialInput.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                    //SET Colums.
                    for (int i = 1; i < advgListSerialInput.Columns.Count + 1; i++)
                    {
                        // Fit CELL & Color
                        worksheet.Cells[1, i].EntireColumn.AutoFit();
                        worksheet.Cells[1, i].Interior.ColorIndex = 24;

                    }

                    // SET Boarder 
                    Excel.Range range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[advgListSerialInput.Rows.Count + 1, advgListSerialInput.Columns.Count]].Cells;
                    range.Borders.LineStyle = LineStyle.Thin;
                    /////////////////

                    ////Check Head column if have "RESULT" Check  OK ,NG,PENDING  For Set Color
                    for (int i = 1; i < advgListSerialInput.Columns.Count + 1; i++)
                    {
                        if (worksheet.Cells[1, i].Value == "RESULT")
                        {
                            for (int k = 0; k < advgListSerialInput.Rows.Count; k++)
                            {
                                if (worksheet.Cells[k + 2, i].Value == "OK")
                                {
                                    worksheet.Cells[k + 2, i].Interior.ColorIndex = 4;
                                }
                                else if (worksheet.Cells[k + 2, i].Value == "PENDING")
                                {
                                    worksheet.Cells[k + 2, i].Interior.ColorIndex = 6;
                                }
                                else if (worksheet.Cells[k + 2, i].Value == "NG")
                                {
                                    worksheet.Cells[k + 2, i].Interior.ColorIndex = 3;
                                }
                            }
                        }
                    }


                    addressFile = saveFileDialog.FileName;
                    workbook.SaveAs(addressFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    //open all message Alerts.
                    app.DisplayAlerts = true;

                    _result = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                excelOpen.Kill();
                saveFileDialog = null;
                worksheet = null;
                workbook = null;
                app = null;

            }

            return _result;


        }

    }
}