using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using Microsoft.Office.Interop.Word;
using System.Windows.Forms;

namespace OutDocChapter
{
    public partial class MyRibbon
    {

        private void myRibbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void btnOutChapter_Click(object sender, RibbonControlEventArgs e)
        {
            #region wordの章構成取得
            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            var tplInfoList = new List<(string cNum, string cTitle, string cPage)>();

            for (int i = 1; i < doc.Paragraphs.Count; i++)
            {
                var para = doc.Paragraphs[i];

                if (!(para.get_Style() is Word.Style style))
                {
                    MessageBox.Show("なんかstyleが認識できない", $"{btnOutChapter.Label}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (style.NameLocal.Contains("見出し"))
                {
                    var range = para.Range;
                    var chapterNum = range.ListFormat.ListString;
                    var chapterTitle = range.Text.Replace(Environment.NewLine, "");
                    var page = range.Information[Word.WdInformation.wdActiveEndPageNumber] as string;
                    var info = (cNum: chapterNum, cTitle: chapterTitle, cPage: page ?? string.Empty);
                    tplInfoList.Add(info);
                }
            }
            if (tplInfoList.Count <= 0)
            {
                MessageBox.Show("章を確認できませんでした。", $"{btnOutChapter.Label}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #endregion

            #region Excel出力
            var excelApp = new Excel.Application
            {
                Visible = false
            };
            try
            {
                Excel.Workbook wb = excelApp.Workbooks.Add();
                try
                {
                    Excel.Worksheet sheet = wb.Sheets[1];
                    try
                    {
                        // 行の「+」は日付、wordの文書名、空行、あとは念のため。
                        // 列の「+」は最初の空列と出力の最初の列（Length = 0 が最大の可能性あり）、あと念のため
                        var colMax = tplInfoList.Max(x => x.cNum.Length);
                        var setValue = new object[tplInfoList.Count + 10, colMax + 10];
                        setValue[0, 0] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        setValue[1, 0] = doc.FullName;

                        for (int row = 0; row < tplInfoList.Count; row++)
                        {
                            var tplInfo = tplInfoList[row];
                            for (int col = 0; col < colMax; col++)
                            {
                                var colIndent = tplInfo.cNum.Count(c => c == '.');
                                var value = $"{tplInfo.cNum} {tplInfo.cTitle} (P.{tplInfo.cPage})";
                                setValue[row + 4, col + 2 + colIndent] = value;
                            }
                        }

                        // Excelの貼り付け開始位置（Indentが1始まりなので注意）
                        var pasteRow = 1;
                        var pasteCol = 1;
                        var startCell = (Excel.Range)sheet.Cells[pasteRow, pasteCol];
                        try
                        {
                            var endCell = (Excel.Range)sheet.Cells[pasteRow + tplInfoList.Count + 4, pasteCol + colMax + 3];
                            try
                            {
                                var targetRange = sheet.Range[startCell, endCell];
                                try
                                {
                                    targetRange.Value = setValue;
                                }
                                finally { Marshal.ReleaseComObject(targetRange); }
                            }
                            finally { Marshal.ReleaseComObject(endCell); }
                        }
                        finally { Marshal.ReleaseComObject(startCell); }
                    }
                    finally { Marshal.ReleaseComObject(sheet); }
                }
                finally { Marshal.ReleaseComObject(wb); }
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                excelApp.Visible = true;
            }
            #endregion
        }
    }
}
